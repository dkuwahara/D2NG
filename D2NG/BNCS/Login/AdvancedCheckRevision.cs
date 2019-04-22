using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Reflection;

namespace D2NG.BNCS.Login
{
    public class AdvancedCheckRevision
    {
        protected delegate uint Operator(uint var1, uint var2);
        protected static Dictionary<char, Operator> Operations = new Dictionary<char, Operator>() {
            {'+', Add}, {'-', Subtract}, {'*', Multiply}, {'/', Divide}, {'|', Or}, {'&', And}, {'^', Xor}
        };

        protected static Dictionary<int, OpCode> Ldargs = new Dictionary<int, OpCode>() {
            {0, OpCodes.Ldarg_0}, {1, OpCodes.Ldarg_1}, {2, OpCodes.Ldarg_2}, {3, OpCodes.Ldarg_3}
        };

        protected static Dictionary<char, OpCode> Operators = new Dictionary<char, OpCode>() {
            {'+', OpCodes.Add}, {'-', OpCodes.Sub}, {'*', OpCodes.Mul}, {'/', OpCodes.Div},
            {'|', OpCodes.Or},  {'&', OpCodes.And}, {'^', OpCodes.Xor}
        };

        protected delegate void FileHasher(ref uint a, ref uint b, ref uint c, ref uint s, byte[] f);

        protected static uint[] hashes = new uint[] { 0xE7F4CB62, 0xF6A14FFC, 0xAA5504AF, 0x871FCDC2, 0x11BF6A18, 0xC57292E6, 0x7927D27E, 0x2FEC8733 };

        public static uint FastComputeHash(string formula, string mpqFile, String gameExe, String bnclientDll, String d2clientDll)
        {
            byte[] game = File.ReadAllBytes(gameExe);
            byte[] bnclient = File.ReadAllBytes(bnclientDll);
            byte[] d2client = File.ReadAllBytes(d2clientDll);

            int mpq = Convert.ToInt32(mpqFile[mpqFile.LastIndexOf('.') - 1].ToString(), 10);
            uint[] values = new uint[4];
            IEnumerable<FormulaOp> ops = BuildFormula(formula, ref values);
            values[0] ^= hashes[mpq];

            FileHasher HashFile = BuildFileHasher(ops);

            HashFile(ref values[0], ref values[1], ref values[2], ref values[3], game);
            HashFile(ref values[0], ref values[1], ref values[2], ref values[3], bnclient);
            HashFile(ref values[0], ref values[1], ref values[2], ref values[3], d2client);

            return values[2];
        }

        protected static FileHasher BuildFileHasher(IEnumerable<FormulaOp> ops)
        {
            Type uint_t = typeof(uint).MakeByRefType();
            DynamicMethod method = new DynamicMethod("HashFile", typeof(void),
                                                     new Type[] { uint_t, uint_t, uint_t, uint_t, typeof(byte[]) });
            MethodInfo touint32 = typeof(BitConverter).GetMethod("ToUInt32");

            ILGenerator gen = method.GetILGenerator();

            Label start = gen.DefineLabel();
            LocalBuilder index = gen.DeclareLocal(typeof(int));
            LocalBuilder len = gen.DeclareLocal(typeof(int));

            // initialize the loop counter
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Stloc, index);

            // load the length of the array into a local
            gen.Emit(OpCodes.Ldarg, (short)4);
            gen.Emit(OpCodes.Ldlen);
            gen.Emit(OpCodes.Conv_I4);
            gen.Emit(OpCodes.Stloc, len);

            // start of loop across the file
            gen.MarkLabel(start);

            // load the value of arg4 at index into the address of arg3
            gen.Emit(OpCodes.Ldarg_3);
            gen.Emit(OpCodes.Ldarg_S, (byte)4);
            gen.Emit(OpCodes.Ldloc, index);
            gen.EmitCall(OpCodes.Call, touint32, null);
            gen.Emit(OpCodes.Stind_I4);

            // for each op in the formula...
            foreach (var op in ops)
            {
                // load the result address
                gen.Emit(Ldargs[op.Result]);

                // load the first value
                gen.Emit(Ldargs[op.Variable1]);
                gen.Emit(OpCodes.Ldind_U4);

                // load the second value
                gen.Emit(Ldargs[op.Variable2]);
                gen.Emit(OpCodes.Ldind_U4);

                // execute the operator
                gen.Emit(Operators[op.Operation]);

                // store the result in the result address
                gen.Emit(OpCodes.Stind_I4);
            }

            // increment the loop counter
            gen.Emit(OpCodes.Ldloc, index);
            gen.Emit(OpCodes.Ldc_I4_4);
            gen.Emit(OpCodes.Add);
            gen.Emit(OpCodes.Stloc, index);

            // jump back to the top of the label if the loop counter is less arg4's length
            gen.Emit(OpCodes.Ldloc, index);
            gen.Emit(OpCodes.Ldloc, len);
            gen.Emit(OpCodes.Blt, start);
            gen.Emit(OpCodes.Ret);

            FileHasher del = (FileHasher)method.CreateDelegate(typeof(FileHasher));
            return del;
        }

        public static uint ComputeHash(string formula, string mpqFile, FileStream gameExe, FileStream bnclientDll, FileStream d2clientDll)
        {
            byte[] game = File.ReadAllBytes(gameExe.Name);
            byte[] bnclient = File.ReadAllBytes(bnclientDll.Name);
            byte[] d2client = File.ReadAllBytes(d2clientDll.Name);

            uint[] values = new uint[4];
            IEnumerable<FormulaOp> ops = BuildFormula(formula, ref values);

            // TODO: figure out the real mpq hashing code
            //computeFileHash(ops, mpqBytes, ref values);
            // UGLY HACK: use the hardcoded mpq hash
            int mpq = Convert.ToInt32("" + mpqFile[mpqFile.LastIndexOf('.') - 1], 10);
            values[0] ^= hashes[mpq];

            ComputeFileHash(ops, game, ref values);
            ComputeFileHash(ops, bnclient, ref values);
            ComputeFileHash(ops, d2client, ref values);

            return values[2];
        }

        protected static IEnumerable<FormulaOp> BuildFormula(string formula, ref uint[] values)
        {
            List<FormulaOp> ops = new List<FormulaOp>();
            string[] tokens = formula.Split(' ');
            foreach (string token in tokens)
            {
                string[] param = token.Split('=');

                if (param.Length == 1)
                    continue;

                int res = WhichVariable(param[0][0]);
                if (char.IsDigit(param[1][0]))
                    values[res] = Convert.ToUInt32(param[1]);
                else
                {
                    string method = param[1];
                    ops.Add(new FormulaOp(method[1], res, WhichVariable(method[0]), WhichVariable(method[2])));
                }
            }
            return ops;
        }
        protected static int WhichVariable(char param)
        {
            int res = (param) - 'A';
            if (res > 2) res = 3;
            return res;
        }

        protected static void ComputeFileHash(IEnumerable<FormulaOp> formula, byte[] file, ref uint[] values)
        {
            int len = file.Length;
            for (int i = 0; i < len; i += 4)
            {
                values[3] = BitConverter.ToUInt32(file, i);
                foreach (FormulaOp op in formula)
                    values[op.Result] = Operations[op.Operation](values[op.Variable1], values[op.Variable2]);
            }
        }

        protected static uint Add(uint var1, uint var2) { return var1 + var2; }
        protected static uint Subtract(uint var1, uint var2) { return var1 - var2; }
        protected static uint Multiply(uint var1, uint var2) { return var1 * var2; }
        protected static uint Divide(uint var1, uint var2) { return var1 / var2; }
        protected static uint Or(uint var1, uint var2) { return var1 | var2; }
        protected static uint Xor(uint var1, uint var2) { return var1 ^ var2; }
        protected static uint And(uint var1, uint var2) { return var1 & var2; }

        protected class FormulaOp
        {
            public int Variable1 { get; protected set; }
            public int Variable2 { get; protected set; }
            public int Result { get; protected set; }
            public char Operation { get; protected set; }
            public FormulaOp(char op, int result, int variable1, int variable2)
            {
                Result = result; Variable1 = variable1; Variable2 = variable2; Operation = op;
            }
        }
    }
}