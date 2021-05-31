using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Ora
{
    class Program
    {
        public static string fileDir = string.Empty;
        public static string agilePath = string.Empty;


        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.ASCII;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine("Drag n Drop the Exe: ");
            string path = Console.ReadLine().Replace("\"", "");

            fileDir = System.IO.Path.GetDirectoryName(path).Replace("\"", "");

            agilePath = fileDir + "\\AgileDotNet.VMRuntime.dll";
            
            string obf = "";

            Console.Clear();
            ModuleDefMD module = ModuleDefMD.Load(path);

            Console.WriteLine("Ora v2.0");
            Console.WriteLine("By Pyhoma69 [https://github.com/Pyhoma69]\n");
            info($"Processing module '{module.Name}'...");
            info($"Resolving dependencies...");

            foreach (var data in module.GetAssemblyRefs())
            {
                if (data.ToString().Contains("AgileDotNet.VMRuntime"))
                    obf = "Agile.NET";
                dbg($"Resolved  '{data.ToString()}'");
            }
            success("Resolved all dependencies");

            Colorful.Console.Write("\n[DEBUG]    ", Color.LightSkyBlue);
            Colorful.Console.Write("Pick an option [1: Auto Detection |2: Manual Detection] ");
            stopwatch.Stop();
            int detection = int.Parse(Console.ReadLine());
            stopwatch.Start();

            if (detection == 1)
            {
                if (obf == "Agile.NET")
                {
                    info("Detected Agile.NET as Obfuscator");
                    obf = "AgileVM";
                }
                else if ((module.Assembly.Modules[0].CustomAttributes[0].AttributeType.ToString().Contains("ConfusedBy") || module.Assembly.Modules[0].CustomAttributes[1].AttributeType.ToString().Contains("ConfusedBy")) || module.FullName.Contains("вє∂ѕ ρяσтє¢тσя"))
                {
                    info("Detected ConfuserEx as Obfuscator");
                    KoiData.engine(module, path);
                }
                else
                {
                    info("Detected EazFuscator as Obfuscator");
                    process("eaz", path);
                }

            }
            else
            {
                Colorful.Console.Write("[DEBUG]    ", Color.LightSkyBlue);
                Colorful.Console.Write("Pick a virtualizer [1: Koi |2: Eaz |3: Agile] ");
                stopwatch.Stop();
                int vmOption = int.Parse(Console.ReadLine());
                stopwatch.Start();
                if (vmOption == 1)
                {
                    KoiData.engine(module, path);
                }
                else if (vmOption == 2)
                {
                    process("eaz", path);
                }
                else
                {
                    process("agile", path);
                }
            }
            
            stopwatch.Stop();
            Console.WriteLine("\n\n\nElapsed time: " + stopwatch.Elapsed.TotalSeconds.ToString() + " seconds.");
            Console.ReadKey();

        }
        
        public static void process(string Type, string path, byte[] bytes = null)
        {
            try
            {
                dbg("Hooking Nen...");
                File.Copy("Nen.dll", fileDir + "\\Nen.dll");
                switch (Type)
                {
                    case "eaz":
                        Injections.eazInj(path);
                        break;

                    case "koi":
                        Injections.koiInj(path, bytes);
                        break;

                    case "vmp":

                        break;

                    case "agile":
                        Injections.agileInj(agilePath);
                        File.Move(agilePath, agilePath + ".bak");
                        File.Move(agilePath + ".temp", agilePath);
                        success("Hooked Succesfully!");
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex) { File.WriteAllText("failure.txt", ex.Message.ToString()); }
        }   

        public static void dbg(string str)
        {
            Colorful.Console.Write("\n[DEBUG]    ", Color.LightSkyBlue);
            Colorful.Console.Write(str);
        }
        public static void info(string str)
        {
            Colorful.Console.Write("\n[INFO]     ", Color.Purple);
            Colorful.Console.Write(str);
        }
        public static void success(string str)
        {
            Colorful.Console.Write("\n[SUCCESS]  ", Color.Green);
            Colorful.Console.Write(str);
        }
        public static void error(string str)
        {
            Colorful.Console.Write("\n[ERROR]    ", Color.Gray);
            Colorful.Console.Write(str);
        }
        public static void fail(string str)
        {
            Colorful.Console.Write("\n[FAILURE]  ", Color.Red);
            Colorful.Console.Write(str);
        }
        public static string ToHex(int value)
        {
            return String.Format("0x{0:X}", value);
        }
    }
}
