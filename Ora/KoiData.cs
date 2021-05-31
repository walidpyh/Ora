using System;
using System.IO;
using System.Linq;
using System.Reflection;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Ora
{
    class KoiData
    {
        public static void engine(ModuleDefMD module, string path)
        {
            Program.dbg("Looking for KoiVM data");
            bool isFound = false;
            foreach (var stream in module.Metadata.AllStreams)
            {
                if (stream.Name == "#Koi" || stream.Name == "#Bed" || stream.Name == "Eddy^CZ")
                {
                    isFound = true;
                    Program.info($"Found KoiVM data '{stream.Name}'");
                    Program.dbg($"Heap offset: {Program.ToHex((int)stream.StreamHeader.StartOffset)}");
                    Program.dbg($"Heap size: {Program.ToHex((int)stream.StreamHeader.StreamSize)}");
                    var read = stream.CreateReader();
                    byte[] bytes = read.ToArray();
                    System.IO.File.WriteAllText(path + "_" + stream.Name.Replace("#", "") + "_unicode.bin", System.Text.Encoding.Unicode.GetString(bytes));
                    System.IO.File.WriteAllBytes(path + "_" + stream.Name.Replace("#", "") + ".bin", bytes);
                    Program.success("Exported KoiVM data");
                    Program.dbg("Hooking Nen...");

                    Program.process("koi", path, bytes);

                    int VMmethods = KoiData.VMedMethods(path);
                    Program.success($"Parsed {VMmethods} virtualized methods");


                    //VMData koi = new VMData(stream.StreamHeader);
                    //System.Reflection.Module m = System.Reflection.Assembly
                    //System.Reflection.Module m = Type.GetTypeFromHandle(typeof(Fox).TypeHandle).Module;
                }
            }
            if (!isFound)
                Program.fail("Couldn't find KoiVM data");
        }

        public static MethodDef GetMethod(ModuleDefMD md)
        {
            TypeDef[] array = md.Types.ToArray<TypeDef>();
            for (int i = 0; i < array.Length; i++)
            {
                foreach (MethodDef methodDef in array[i].Methods.ToArray<MethodDef>())
                {
                    bool flag = methodDef.HasBody && methodDef.Body.HasInstructions;
                    if (flag)
                    {
                        bool flag2 = methodDef.Body.Instructions.Count<Instruction>() > 205;
                        if (flag2)
                        {
                            bool flag3 = methodDef.Body.Instructions[204].OpCode == OpCodes.Ldstr;
                            if (flag3)
                            {
                                bool flag4 = methodDef.Body.Instructions[205].OpCode == OpCodes.Call;
                                if (flag4)
                                {
                                    bool flag5 = methodDef.Body.Instructions[203].OpCode == OpCodes.Callvirt;
                                    if (flag5)
                                    {
                                        return methodDef;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static int VMedMethods(string path)
        {
            int result = 0;
            ModuleDefMD md = ModuleDefMD.Load(path);
            foreach (TypeDef type in md.Types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.HasBody && method.Body.HasInstructions && method.Body.Instructions.Count() < 100)
                    {
                        for (int i = 0; i < method.Body.Instructions.Count(); i++)
                        {
                            if (method.Body.Instructions[i].OpCode == OpCodes.Call && method.Body.Instructions[i].Operand.ToString().Contains("System.Object KoiVM.Runtime.VMEntry::Run(System.RuntimeTypeHandle,System.UInt32,System.Object[])") && method.Body.Instructions[i - 1].OpCode.ToString().Contains("newarr"))
                            {
                                result++;
                                Program.dbg($"Found virtualized method: {method.FullName} Id: {result}");
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static MethodDef GetTestMethod(ModuleDefMD mod)
        {
            TypeDef[] array = mod.Types.ToArray<TypeDef>();
            for (int i = 0; i < array.Length; i++)
            {
                foreach (MethodDef methodDef in array[i].Methods.ToArray<MethodDef>())
                {
                    bool flag = methodDef.FullName.Contains("Test123");
                    if (flag)
                    {
                        return methodDef;
                    }
                }
            }
            return null;
        
        }

        private unsafe static void* Test123(byte* A_0)
        {
            UnmanagedMemoryStream unmanagedMemoryStream = (UnmanagedMemoryStream)Assembly.GetExecutingAssembly().GetManifestResourceStream("VM");
            return Test2((void*)unmanagedMemoryStream.PositionPointer, (uint)unmanagedMemoryStream.Length);
        }

        private unsafe static void* Test2(void* A_0, uint A_1)
        {
            return null;
        }

        public static MethodDef GetMethod2(ModuleDefMD md)
        {
            TypeDef[] array = md.Types.ToArray<TypeDef>();
            for (int i = 0; i < array.Length; i++)
            {
                foreach (MethodDef methodDef in array[i].Methods.ToArray<MethodDef>())
                {
                    bool flag = methodDef.HasBody && methodDef.Body.HasInstructions;
                    if (flag)
                    {
                        bool flag2 = methodDef.Body.Instructions.Count<Instruction>() > 12;
                        if (flag2)
                        {
                            bool flag3 = methodDef.Body.Instructions[2].OpCode == OpCodes.Call;
                            if (flag3)
                            {
                                bool flag4 = methodDef.Body.Instructions[2].ToString().ToUpper().Contains("ALLOC");
                                if (flag4)
                                {
                                    bool flag5 = methodDef.Body.Instructions[8].OpCode == OpCodes.Call;
                                    if (flag5)
                                    {
                                        return methodDef;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
