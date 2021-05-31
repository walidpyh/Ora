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
    class Injections
    {
        public static void agileInj(string path)
        {
            ModuleDefMD md = ModuleDefMD.Load(path);
            foreach (TypeDef type in md.Types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.HasBody && method.Body.HasInstructions && method.Body.Instructions.Count() < 300)
                    {
                        for (int i = 0; i < method.Body.Instructions.Count(); i++)
                        {
                            if (method.Body.Instructions[i].OpCode == OpCodes.Callvirt)
                            {
                                string operand = method.Body.Instructions[i].Operand.ToString();
                                if (operand.Contains("System.Object System.Reflection.MethodBase::Invoke(System.Object,System.Object[])") && method.Body.Instructions[i + 1].IsStloc() && method.Body.Instructions[i - 1].IsLdarg() && method.Body.Instructions[i - 3].IsLdarg())
                                {
                                    MDToken var = method.MDToken;
                                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                                    Importer importer = new Importer(md);
                                    IMethod myMethod = importer.Import(typeof(Nen.Nen).GetMethod("Queue"));
                                    method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, md.Import(myMethod)));
                                    i += 1;

                                }
                            }
                        }
                    }
                }
            }

            ModuleWriterOptions moduleWriterOptions = new ModuleWriterOptions(md);
            moduleWriterOptions.MetadataOptions.Flags = MetadataFlags.KeepOldMaxStack;
            moduleWriterOptions.Logger = DummyLogger.NoThrowInstance;
            moduleWriterOptions.MetadataOptions.Flags = MetadataFlags.PreserveAll;
            moduleWriterOptions.MetadataOptions.PreserveHeapOrder(md, true);
            moduleWriterOptions.Cor20HeaderOptions.Flags = new ComImageFlags?(ComImageFlags.ILOnly | ComImageFlags.Bit32Required);
            md.Write(path + ".temp", moduleWriterOptions);
        }

        public static void eazInj(string path)
        {
            ModuleDefMD md = ModuleDefMD.Load(path);
            foreach (TypeDef type in md.Types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.HasBody && method.Body.HasInstructions && method.Body.Instructions.Count() < 200)
                    {
                        for (int i = 0; i < method.Body.Instructions.Count(); i++)
                        {
                            if (method.Body.Instructions[i].OpCode == OpCodes.Callvirt)
                            {
                                string operand = method.Body.Instructions[i].Operand.ToString();
                                if (operand.Contains("System.Object System.Reflection.MethodBase::Invoke(System.Object,System.Object[])") && method.Body.Instructions[i - 1].IsLdarg() && method.Body.Instructions[i - 2].IsLdarg() && method.Body.Instructions[i - 3].IsLdarg())
                                {
                                    MDToken var = method.MDToken;
                                    Colorful.Console.Write("[INFO]     ", Color.Purple);
                                    Colorful.Console.Write("Invoke MDToken: " + var.Raw.ToString());
                                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                                    Importer importer = new Importer(md);
                                    IMethod myMethod = importer.Import(typeof(Nen.Nen).GetMethod("Queue"));
                                    method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, md.Import(myMethod)));
                                    i += 1;
                                    Program.success("Forlaxer Hooked Succesfully!");
                                }
                            }
                        }
                    }
                }
            }

            ModuleWriterOptions moduleWriterOptions = new ModuleWriterOptions(md);
            moduleWriterOptions.MetadataOptions.Flags = MetadataFlags.KeepOldMaxStack;
            moduleWriterOptions.Logger = DummyLogger.NoThrowInstance;
            moduleWriterOptions.MetadataOptions.Flags = MetadataFlags.PreserveAll;
            moduleWriterOptions.MetadataOptions.PreserveHeapOrder(md, true);
            moduleWriterOptions.Cor20HeaderOptions.Flags = new ComImageFlags?(ComImageFlags.ILOnly | ComImageFlags.Bit32Required);
            md.Write(path + "_Hooked_.exe", moduleWriterOptions);
        }
        public static void koiInj(string path, byte[] koi)
        {
            ModuleDefMD md = ModuleDefMD.Load(path);
            foreach (TypeDef type in md.Types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.HasBody && method.Body.HasInstructions && method.Body.Instructions.Count() > 150)
                    {
                        for (int i = 0; i < method.Body.Instructions.Count(); i++)
                        {
                            if (method.Body.Instructions[i].OpCode == OpCodes.Callvirt)
                            {
                                string operand = method.Body.Instructions[i].Operand.ToString();
                                if (operand.Contains("System.Object System.Reflection.MethodBase::Invoke(System.Object,System.Object[])") && method.Body.Instructions[i + 1].IsStloc() && method.Body.Instructions[i - 2].IsLdloc())
                                {
                                    MDToken var = method.MDToken;
                                    Colorful.Console.Write("[INFO]     ", Color.Purple);
                                    Colorful.Console.Write("Invoke MDToken: " + var.Raw.ToString());
                                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                                    Importer importer = new Importer(md);
                                    IMethod myMethod = importer.Import(typeof(Nen.Nen).GetMethod("Queue"));

                                    method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, md.Import(myMethod)));
                                    i += 1;
                                    Program.success("Hooked Succesfully!");
                                }
                            }
                        }
                    }
                }
            }

            Program.dbg("Preparing to move KoiVM Data...");
            MethodDef methodDef = KoiData.GetMethod(md);
            MethodDef methodDef2 = KoiData.GetMethod2(md);
            try
            {
                methodDef.Body.Instructions.Clear();
            }
            catch { }

            ModuleDefMD mod = ModuleDefMD.Load("Ora.exe");
            MethodDef testMethod = KoiData.GetTestMethod(mod);
            foreach (Instruction item in testMethod.Body.Instructions)
            {
                methodDef.Body.Instructions.Add(item);
            }
            TypeRef type2 = new TypeRefUser(md, "System.IO", "UnmanagedMemoryStream", md.CorLibTypes.AssemblyRef);
            methodDef.Body.Variables[0].Type = type2.ToTypeSig(true);
            if (methodDef.Body.Instructions[11].OpCode == OpCodes.Call)
            {
                methodDef.Body.Instructions[11].Operand = methodDef2.ResolveMethodDef();
            }
            Program.dbg("Adding VM to Resources");
            md.Resources.Add(new EmbeddedResource("VM", koi, ManifestResourceAttributes.Private));
            ModuleWriterOptions moduleWriterOptions = new ModuleWriterOptions(md);
            moduleWriterOptions.MetadataOptions.Flags = MetadataFlags.KeepOldMaxStack;
            moduleWriterOptions.Logger = DummyLogger.NoThrowInstance;
            moduleWriterOptions.MetadataOptions.Flags = MetadataFlags.PreserveAll;
            moduleWriterOptions.MetadataOptions.PreserveHeapOrder(md, true);
            moduleWriterOptions.Cor20HeaderOptions.Flags = new ComImageFlags?(ComImageFlags.ILOnly | ComImageFlags.Bit32Required);
            Program.success("VM Resource injected");
            md.Write(path + "_Hooked_.exe", moduleWriterOptions);
        }
    }
}
