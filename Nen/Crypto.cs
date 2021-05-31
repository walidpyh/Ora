using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using System.IO;

namespace Nen
{

	class Crypto
	{
		public static string md5(string input)
		{
			MD5 md5 = MD5.Create();
			byte[] inputBytes = Encoding.ASCII.GetBytes(input);
			byte[] hashBytes = md5.ComputeHash(inputBytes);
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hashBytes.Length; i++)
			{
				sb.Append(hashBytes[i].ToString("X2"));
			}
			return sb.ToString();

		}

		public static string sha256(string input)
		{
			return Sha256.ArrayToString(Sha256.HashFile(input));
		}

		public class Sha256
		{
			public static string ArrayToString(ReadOnlyCollection<byte> arr)
			{
				StringBuilder stringBuilder = new StringBuilder(arr.Count * 2);
				for (int i = 0; i < arr.Count; i++)
				{
					stringBuilder.AppendFormat("{0:x2}", arr[i]);
				}
				return stringBuilder.ToString();
			}

			private static uint ROTR(uint x, byte n)
			{
				return x >> (int)n | x << (int)(32 - n);
			}

			private static uint Ch(uint x, uint y, uint z)
			{
				return (x & y) ^ (~x & z);
			}

			private static uint Maj(uint x, uint y, uint z)
			{
				return (x & y) ^ (x & z) ^ (y & z);
			}

			private static uint Sigma0(uint x)
			{
				return Sha256.ROTR(x, 2) ^ Sha256.ROTR(x, 13) ^ Sha256.ROTR(x, 22);
			}

			private static uint Sigma1(uint x)
			{
				return Sha256.ROTR(x, 6) ^ Sha256.ROTR(x, 11) ^ Sha256.ROTR(x, 25);
			}

			private static uint sigma0(uint x)
			{
				return Sha256.ROTR(x, 7) ^ Sha256.ROTR(x, 18) ^ x >> 3;
			}

			private static uint sigma1(uint x)
			{
				return Sha256.ROTR(x, 17) ^ Sha256.ROTR(x, 19) ^ x >> 10;
			}

			private void processBlock(uint[] M)
			{
				uint[] array = new uint[64];
				for (int i = 0; i < 16; i++)
				{
					array[i] = M[i];
				}
				for (int j = 16; j < 64; j++)
				{
					array[j] = Sha256.sigma1(array[j - 2]) + array[j - 7] + Sha256.sigma0(array[j - 15]) + array[j - 16];
				}
				uint num = this.H[0];
				uint num2 = this.H[1];
				uint num3 = this.H[2];
				uint num4 = this.H[3];
				uint num5 = this.H[4];
				uint num6 = this.H[5];
				uint num7 = this.H[6];
				uint num8 = this.H[7];
				for (int k = 0; k < 64; k++)
				{
					uint num9 = num8 + Sha256.Sigma1(num5) + Sha256.Ch(num5, num6, num7) + Sha256.K[k] + array[k];
					uint num10 = Sha256.Sigma0(num) + Sha256.Maj(num, num2, num3);
					num8 = num7;
					num7 = num6;
					num6 = num5;
					num5 = num4 + num9;
					num4 = num3;
					num3 = num2;
					num2 = num;
					num = num9 + num10;
				}
				this.H[0] = num + this.H[0];
				this.H[1] = num2 + this.H[1];
				this.H[2] = num3 + this.H[2];
				this.H[3] = num4 + this.H[3];
				this.H[4] = num5 + this.H[4];
				this.H[5] = num6 + this.H[5];
				this.H[6] = num7 + this.H[6];
				this.H[7] = num8 + this.H[7];
			}

			public void AddData(byte[] data, uint offset, uint len)
			{
				if (this.closed)
				{
					throw new InvalidOperationException("Adding data to a closed hasher.");
				}
				if (len == 0U)
				{
					return;
				}
				this.bits_processed += (ulong)(len * 8U);
				while (len > 0U)
				{
					uint num;
					if (len < 64U)
					{
						if (this.pending_block_off + len > 64U)
						{
							num = 64U - this.pending_block_off;
						}
						else
						{
							num = len;
						}
					}
					else
					{
						num = 64U - this.pending_block_off;
					}
					Array.Copy(data, (long)((ulong)offset), this.pending_block, (long)((ulong)this.pending_block_off), (long)((ulong)num));
					len -= num;
					offset += num;
					this.pending_block_off += num;
					if (this.pending_block_off == 64U)
					{
						Sha256.toUintArray(this.pending_block, this.uint_buffer);
						this.processBlock(this.uint_buffer);
						this.pending_block_off = 0U;
					}
				}
			}

			public ReadOnlyCollection<byte> GetHash()
			{
				return Sha256.toByteArray(this.GetHashUInt32());
			}

			public ReadOnlyCollection<uint> GetHashUInt32()
			{
				if (!this.closed)
				{
					ulong num = this.bits_processed;
					this.AddData(new byte[]
					{
					128
					}, 0U, 1U);
					uint num2 = 64U - this.pending_block_off;
					if (num2 < 8U)
					{
						num2 += 64U;
					}
					byte[] array = new byte[num2];
					for (uint num3 = 1U; num3 <= 8U; num3 += 1U)
					{
						array[(int)((IntPtr)((long)array.Length - (long)((ulong)num3)))] = (byte)num;
						num >>= 8;
					}
					this.AddData(array, 0U, (uint)array.Length);
					this.closed = true;
				}
				return Array.AsReadOnly<uint>(this.H);
			}

			private static void toUintArray(byte[] src, uint[] dest)
			{
				uint num = 0U;
				uint num2 = 0U;
				while ((ulong)num < (ulong)((long)dest.Length))
				{
					dest[(int)num] = (uint)((int)src[(int)num2] << 24 | (int)src[(int)(num2 + 1U)] << 16 | (int)src[(int)(num2 + 2U)] << 8 | (int)src[(int)(num2 + 3U)]);
					num += 1U;
					num2 += 4U;
				}
			}

			private static ReadOnlyCollection<byte> toByteArray(ReadOnlyCollection<uint> src)
			{
				byte[] array = new byte[src.Count * 4];
				int num = 0;
				for (int i = 0; i < src.Count; i++)
				{
					array[num++] = (byte)(src[i] >> 24);
					array[num++] = (byte)(src[i] >> 16);
					array[num++] = (byte)(src[i] >> 8);
					array[num++] = (byte)src[i];
				}
				return Array.AsReadOnly<byte>(array);
			}

			public static ReadOnlyCollection<byte> HashFile(string text)
			{
				Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(text));
				Sha256 sha = new Sha256();
				byte[] array = new byte[8196];
				uint num;
				do
				{
					num = (uint)stream.Read(array, 0, array.Length);
					if (num == 0U)
					{
						break;
					}
					sha.AddData(array, 0U, num);
				}
				while (num == 8196U);
				return sha.GetHash();
			}

			private static readonly uint[] K = new uint[]
			{
				1116352408U,
				1899447441U,
				3049323471U,
				3921009573U,
				961987163U,
				1508970993U,
				2453635748U,
				2870763221U,
				3624381080U,
				310598401U,
				607225278U,
				1426881987U,
				1925078388U,
				2162078206U,
				2614888103U,
				3248222580U,
				3835390401U,
				4022224774U,
				264347078U,
				604807628U,
				770255983U,
				1249150122U,
				1555081692U,
				1996064986U,
				2554220882U,
				2821834349U,
				2952996808U,
				3210313671U,
				3336571891U,
				3584528711U,
				113926993U,
				338241895U,
				666307205U,
				773529912U,
				1294757372U,
				1396182291U,
				1695183700U,
				1986661051U,
				2177026350U,
				2456956037U,
				2730485921U,
				2820302411U,
				3259730800U,
				3345764771U,
				3516065817U,
				3600352804U,
				4094571909U,
				275423344U,
				430227734U,
				506948616U,
				659060556U,
				883997877U,
				958139571U,
				1322822218U,
				1537002063U,
				1747873779U,
				1955562222U,
				2024104815U,
				2227730452U,
				2361852424U,
				2428436474U,
				2756734187U,
				3204031479U,
				3329325298U
			};

			private uint[] H = new uint[]
			{
			1779033703U,
			3144134277U,
			1013904242U,
			2773480762U,
			1359893119U,
			2600822924U,
			528734635U,
			1541459225U
			};

			private byte[] pending_block = new byte[64];

			private uint pending_block_off;

			private uint[] uint_buffer = new uint[16];

			private ulong bits_processed;

			private bool closed;
		}
	}
}
