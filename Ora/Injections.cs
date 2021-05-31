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
using System.Reflection;

namespace Ora
{
    class Injections
    {
        public static void vmpInj(string path)
        {
            ModuleDefMD moduleDef = ModuleDefMD.Load(path);
            int num = 0;
            foreach (TypeDef typeDef in moduleDef.Types)
            {
                foreach (MethodDef methodDef in typeDef.Methods)
                {
                    if (methodDef.Parameters.Count == 3 && methodDef.Parameters[1].Type.TypeName == "MethodBase" && methodDef.Parameters[2].Type.TypeName == "Boolean")
                    {
                        for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
                        {
                            if (methodDef.Body.Instructions[i].Operand != null && methodDef.Body.Instructions[i].Operand.ToString().Contains("Invoke"))
                            {
                                if (num++ == 1)
                                {
                                    int num2 = methodDef.Body.Instructions.IndexOf(methodDef.Body.Instructions[i]);
                                    Instruction target = methodDef.Body.Instructions[num2 + 2];

                                    Local local;
                                    if (methodDef.Body.Instructions[num2 + 1].IsStloc())
                                    {
                                        MDToken var = methodDef.MDToken;
                                        methodDef.Body.Instructions[num2].OpCode = OpCodes.Nop;
                                        Importer importer = new Importer(moduleDef);
                                        IMethod myMethod = importer.Import(typeof(Nen.Nen).GetMethod("Queue"));
                                        methodDef.Body.Instructions.Insert(num2 + 1, new Instruction(OpCodes.Call, moduleDef.Import(myMethod)));
                                        Program.success("Hooked Succesfully!");

                                        if (methodDef.Body.Instructions[num2 + 1].OpCode == OpCodes.Stloc_0)
                                        {
                                            local = methodDef.Body.Variables.Locals[0];
                                        }
                                        else
                                        {
                                            if (methodDef.Body.Instructions[num2 + 1].OpCode == OpCodes.Stloc_1)
                                            {
                                                local = methodDef.Body.Variables.Locals[1];
                                            }
                                            else
                                            {
                                                if (methodDef.Body.Instructions[num2 + 1].OpCode == OpCodes.Stloc_2)
                                                {
                                                    local = methodDef.Body.Variables.Locals[2];
                                                }
                                                else
                                                {
                                                    if (methodDef.Body.Instructions[num2 + 1].OpCode == OpCodes.Stloc_3)
                                                    {
                                                        local = methodDef.Body.Variables.Locals[3];
                                                    }
                                                    else
                                                    {
                                                        local = methodDef.Body.Instructions[num2 + 1].GetLocal(null);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (methodDef.Body.Instructions[num2 + 3].OpCode == OpCodes.Stloc_S)
                                        {
                                            local = methodDef.Body.Instructions[num2 + 3].GetLocal(null);
                                            num2 += 2;
                                        }
                                        else
                                        {
                                            local = methodDef.Body.Instructions[num2 + 5].GetLocal(null);
                                            num2 += 4;
                                        }
                                    }
                                    Program.success("CRC-check bypassing...");
                                    string text = methodDef.Body.Instructions[num2 - 1].OpCode.ToString();
                                    int index;
                                    bool flag10 = int.TryParse(text.Substring(text.Length - 1), out index);
                                    bool flag11 = !flag10;
                                    Local local2;
                                    if (flag11)
                                    {
                                        local2 = methodDef.Body.Instructions[num2 - 1].GetLocal(null);
                                    }
                                    else
                                    {
                                        local2 = methodDef.Body.Variables[index];
                                    }
                                    methodDef.Body.Instructions.Insert(num2 - 2, Instruction.Create(OpCodes.Callvirt, moduleDef.Import(typeof(MemberInfo).GetMethod("get_Name", new Type[0]))));
                                    methodDef.Body.Instructions.Insert(num2 - 1, Instruction.Create(OpCodes.Ldstr, "CreateFile"));
                                    methodDef.Body.Instructions.Insert(num2, Instruction.Create(OpCodes.Call, moduleDef.Import(typeof(string).GetMethod("op_Equality", new Type[]
                                    {
                                        typeof(string),
                                        typeof(string)
                                    }))));
                                    methodDef.Body.Instructions.Insert(num2 + 1, Instruction.Create(OpCodes.Brfalse_S, target));
                                    methodDef.Body.Instructions.Insert(num2 + 2, Instruction.Create(OpCodes.Ldloc_S, local2));
                                    methodDef.Body.Instructions.Insert(num2 + 3, Instruction.Create(OpCodes.Ldc_I4_0));
                                    methodDef.Body.Instructions.Insert(num2 + 4, Instruction.Create(OpCodes.Ldstr, ""));
                                    methodDef.Body.Instructions.Insert(num2 + 5, Instruction.Create(OpCodes.Stelem_Ref));
                                    methodDef.Body.Instructions.Insert(num2 + 6, Instruction.Create(OpCodes.Ldarg_1));
                                    Program.success("Anti-debug bypassing...");
                                    methodDef.Body.Instructions.Insert(num2 + 11, Instruction.Create(OpCodes.Ldarg_1));
                                    methodDef.Body.Instructions.Insert(num2 + 12, Instruction.Create(OpCodes.Callvirt, moduleDef.Import(typeof(MemberInfo).GetMethod("get_Name", new Type[0]))));
                                    methodDef.Body.Instructions.Insert(num2 + 13, Instruction.Create(OpCodes.Ldstr, "NtQueryInformationProcess"));
                                    methodDef.Body.Instructions.Insert(num2 + 14, Instruction.Create(OpCodes.Call, moduleDef.Import(typeof(string).GetMethod("op_Equality", new Type[]
                                    {
                                        typeof(string),
                                        typeof(string)
                                    }))));
                                    methodDef.Body.Instructions.Insert(num2 + 15, Instruction.Create(OpCodes.Brfalse_S, target));
                                    methodDef.Body.Instructions.Insert(num2 + 16, Instruction.Create(OpCodes.Ldc_I4_1));
                                    methodDef.Body.Instructions.Insert(num2 + 17, Instruction.Create(OpCodes.Box, moduleDef.Import(typeof(int))));
                                    methodDef.Body.Instructions.Insert(num2 + 18, Instruction.Create(OpCodes.Stloc_S, local));
                                    methodDef.Body.Instructions.Insert(num2 + 19, Instruction.Create(OpCodes.Ldarg_1));
                                    methodDef.Body.Instructions.Insert(num2 + 20, Instruction.Create(OpCodes.Callvirt, moduleDef.Import(typeof(MemberInfo).GetMethod("get_Name", new Type[0]))));
                                    methodDef.Body.Instructions.Insert(num2 + 21, Instruction.Create(OpCodes.Ldstr, "get_IsAttached"));
                                    methodDef.Body.Instructions.Insert(num2 + 22, Instruction.Create(OpCodes.Call, moduleDef.Import(typeof(string).GetMethod("op_Equality", new Type[]
                                    {
                                        typeof(string),
                                        typeof(string)
                                    }))));
                                    methodDef.Body.Instructions.Insert(num2 + 23, Instruction.Create(OpCodes.Brfalse_S, target));
                                    methodDef.Body.Instructions.Insert(num2 + 24, Instruction.Create(OpCodes.Ldc_I4_0));
                                    methodDef.Body.Instructions.Insert(num2 + 25, Instruction.Create(OpCodes.Box, moduleDef.Import(typeof(bool))));
                                    methodDef.Body.Instructions.Insert(num2 + 26, Instruction.Create(OpCodes.Stloc_S, local));
                                    methodDef.Body.Instructions.Insert(num2 + 27, Instruction.Create(OpCodes.Ldarg_1));
                                    methodDef.Body.Instructions.Insert(num2 + 28, Instruction.Create(OpCodes.Callvirt, moduleDef.Import(typeof(MemberInfo).GetMethod("get_Name", new Type[0]))));
                                    methodDef.Body.Instructions.Insert(num2 + 29, Instruction.Create(OpCodes.Ldstr, "IsLogging"));
                                    methodDef.Body.Instructions.Insert(num2 + 30, Instruction.Create(OpCodes.Call, moduleDef.Import(typeof(string).GetMethod("op_Equality", new Type[]
                                    {
                                        typeof(string),
                                        typeof(string)
                                    }))));
                                    methodDef.Body.Instructions.Insert(num2 + 31, Instruction.Create(OpCodes.Brfalse_S, target));
                                    methodDef.Body.Instructions.Insert(num2 + 32, Instruction.Create(OpCodes.Ldc_I4_0));
                                    methodDef.Body.Instructions.Insert(num2 + 33, Instruction.Create(OpCodes.Box, moduleDef.Import(typeof(bool))));
                                    methodDef.Body.Instructions.Insert(num2 + 34, Instruction.Create(OpCodes.Stloc_S, local));
                                    methodDef.Body.Instructions.Insert(num2 + 35, Instruction.Create(OpCodes.Ldarg_1));
                                    methodDef.Body.Instructions.Insert(num2 + 36, Instruction.Create(OpCodes.Callvirt, moduleDef.Import(typeof(MemberInfo).GetMethod("get_Name", new Type[0]))));
                                    methodDef.Body.Instructions.Insert(num2 + 37, Instruction.Create(OpCodes.Ldstr, "IsDebuggerPresent"));
                                    methodDef.Body.Instructions.Insert(num2 + 38, Instruction.Create(OpCodes.Call, moduleDef.Import(typeof(string).GetMethod("op_Equality", new Type[]
                                    {
                                        typeof(string),
                                        typeof(string)
                                    }))));
                                    methodDef.Body.Instructions.Insert(num2 + 39, Instruction.Create(OpCodes.Brfalse_S, target));
                                    methodDef.Body.Instructions.Insert(num2 + 40, Instruction.Create(OpCodes.Ldc_I4_0));
                                    methodDef.Body.Instructions.Insert(num2 + 41, Instruction.Create(OpCodes.Box, moduleDef.Import(typeof(bool))));
                                    methodDef.Body.Instructions.Insert(num2 + 42, Instruction.Create(OpCodes.Stloc_S, local));
                                    methodDef.Body.Instructions.Insert(num2 + 43, Instruction.Create(OpCodes.Ldarg_1));
                                    methodDef.Body.Instructions.Insert(num2 + 44, Instruction.Create(OpCodes.Callvirt, moduleDef.Import(typeof(MemberInfo).GetMethod("get_Name", new Type[0]))));
                                    methodDef.Body.Instructions.Insert(num2 + 45, Instruction.Create(OpCodes.Ldstr, "CheckRemoteDebuggerPresent"));
                                    methodDef.Body.Instructions.Insert(num2 + 46, Instruction.Create(OpCodes.Call, moduleDef.Import(typeof(string).GetMethod("op_Equality", new Type[]
                                    {
                                        typeof(string),
                                        typeof(string)
                                    }))));
                                    methodDef.Body.Instructions.Insert(num2 + 47, Instruction.Create(OpCodes.Brfalse_S, target));
                                    methodDef.Body.Instructions.Insert(num2 + 48, Instruction.Create(OpCodes.Ldc_I4_0));
                                    methodDef.Body.Instructions.Insert(num2 + 49, Instruction.Create(OpCodes.Box, moduleDef.Import(typeof(bool))));
                                    methodDef.Body.Instructions.Insert(num2 + 50, Instruction.Create(OpCodes.Stloc_S, local));
                                    methodDef.Body.UpdateInstructionOffsets();
                                    target = methodDef.Body.Instructions[num2 + 6];
                                    methodDef.Body.Instructions[num2 + 1] = Instruction.Create(OpCodes.Brfalse_S, target);
                                    target = methodDef.Body.Instructions[num2 + 19];
                                    methodDef.Body.Instructions[num2 + 15] = Instruction.Create(OpCodes.Brfalse_S, target);
                                    target = methodDef.Body.Instructions[num2 + 27];
                                    methodDef.Body.Instructions[num2 + 23] = Instruction.Create(OpCodes.Brfalse_S, target);
                                    target = methodDef.Body.Instructions[num2 + 35];
                                    methodDef.Body.Instructions[num2 + 31] = Instruction.Create(OpCodes.Brfalse_S, target);
                                    target = methodDef.Body.Instructions[num2 + 43];
                                    methodDef.Body.Instructions[num2 + 39] = Instruction.Create(OpCodes.Brfalse_S, target);
                                    target = methodDef.Body.Instructions[num2 + 51];
                                    methodDef.Body.Instructions[num2 + 47] = Instruction.Create(OpCodes.Brfalse_S, target);
                                    i += 20;
                                }
                            }
                        }
                    }
                }
            }

            var nativeModuleWriter = new dnlib.DotNet.Writer.NativeModuleWriterOptions(moduleDef, false);
            nativeModuleWriter.Logger = DummyLogger.NoThrowInstance;
            nativeModuleWriter.MetadataOptions.Flags = MetadataFlags.PreserveAll |
                                                       MetadataFlags.KeepOldMaxStack |
                                                       MetadataFlags.PreserveExtraSignatureData |
                                                       MetadataFlags.PreserveBlobOffsets |
                                                       MetadataFlags.PreserveUSOffsets |
                                                       MetadataFlags.PreserveStringsOffsets;
            nativeModuleWriter.Cor20HeaderOptions.Flags = ComImageFlags.ILOnly;
            moduleDef.NativeWrite(path + "_Hooked_.exe", nativeModuleWriter);
        }
        public static void vmpInj3(string path)
        {
            ModuleDefMD moduleDef = ModuleDefMD.Load(path);
            foreach (var type in moduleDef.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Parameters.Count == 3 && method.Parameters[1].Type.TypeName == "MethodBase" && method.Parameters[2].Type.TypeName == "Boolean")
                    {
                        for (int i = 0; i < method.Body.Instructions.Count; i++)
                        {
                            if (method.Body.Instructions[i].Operand?.ToString().Contains("Invoke") ?? false)
                            {

                                var indexInvoke = method.Body.Instructions.IndexOf(method.Body.Instructions[i]);
                                var lastInstruction = method.Body.Instructions[indexInvoke + 2];

                                if (method.Body.Instructions[indexInvoke + 1].IsStloc())
                                {
                                    MDToken var = method.MDToken;
                                    method.Body.Instructions[indexInvoke].OpCode = OpCodes.Nop;
                                    Importer importer = new Importer(moduleDef);
                                    IMethod myMethod = importer.Import(typeof(Nen.Nen).GetMethod("Queue"));
                                    method.Body.Instructions.Insert(indexInvoke + 1, new Instruction(OpCodes.Call, moduleDef.Import(myMethod)));
                                    Program.success("Hooked Succesfully!");
                                }

                            }
                        }

                    }
                }
            }

            var nativeModuleWriter = new dnlib.DotNet.Writer.NativeModuleWriterOptions(moduleDef, false);
            nativeModuleWriter.Logger = DummyLogger.NoThrowInstance;
            nativeModuleWriter.MetadataOptions.Flags = MetadataFlags.PreserveAll |
                                                       MetadataFlags.KeepOldMaxStack |
                                                       MetadataFlags.PreserveExtraSignatureData |
                                                       MetadataFlags.PreserveBlobOffsets |
                                                       MetadataFlags.PreserveUSOffsets |
                                                       MetadataFlags.PreserveStringsOffsets;
            nativeModuleWriter.Cor20HeaderOptions.Flags = ComImageFlags.ILOnly;
            moduleDef.NativeWrite(path + "_Hooked_.exe", nativeModuleWriter);
        }
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
                                    Program.success("Hooked Succesfully!");
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
                                    Colorful.Console.Write("\n[INFO]     ", Color.Purple);
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
                                    Colorful.Console.Write("\n[INFO]     ", Color.Purple);
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
