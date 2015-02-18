using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL.Lib.Math
{
    public class KInt : Obj
    {
        public void Assign(KInt k)
        {
            SyntaxTree.Instructions.Enqueue(new SyntaxTree.Instruction()
            {
                instructionType = SyntaxTree.InstructionType.Assign,
                Parameters = new string[] { this.ObjName, k.ObjName }
            });
        }

        public override object GetDefaultValue()
        {
            return 0;
        }

        #region Operators
        public static KInt operator *(KInt a, KInt b)
        {
            SyntaxTree.AssignmentBuffer.Enqueue(new SyntaxTree.Instruction()
            {
                instructionType = SyntaxTree.InstructionType.Math,
                Parameters = new string[] { a.ObjName, "*", b.ObjName }
            });


            return (KInt)new Obj()
            {
                ObjName = "(" + a.ObjName + "*" + b.ObjName + ")"
            };
        }

        public static KInt operator /(KInt a, KInt b)
        {
            SyntaxTree.AssignmentBuffer.Enqueue(new SyntaxTree.Instruction()
            {
                instructionType = SyntaxTree.InstructionType.Math,
                Parameters = new string[] { a.ObjName, "/", b.ObjName }
            });


            return (KInt)new Obj()
            {
                ObjName = "(" + a.ObjName + "/" + b.ObjName + ")"
            };
        }

        public static KInt operator +(KInt a, KInt b)
        {
            SyntaxTree.AssignmentBuffer.Enqueue(new SyntaxTree.Instruction()
            {
                instructionType = SyntaxTree.InstructionType.Math,
                Parameters = new string[] { a.ObjName, "+", b.ObjName }
            });


            return new KInt()
            {
                ObjName = "(" + a.ObjName + "+" + b.ObjName + ")"
            };
        }

        public static KInt operator -(KInt a, KInt b)
        {
            SyntaxTree.AssignmentBuffer.Enqueue(new SyntaxTree.Instruction()
            {
                instructionType = SyntaxTree.InstructionType.Math,
                Parameters = new string[] { a.ObjName, "-", b.ObjName }
            });

            return (KInt)new Obj()
            {
                ObjName = "(" + a.ObjName + "-" + b.ObjName + ")"
            };
        }
        #endregion

        #region Converters
        public static implicit operator KInt(int i)
        {
            SyntaxTree.AssignmentBuffer.Enqueue(new SyntaxTree.Instruction()
            {
                instructionType = SyntaxTree.InstructionType.Math,
                Parameters = new string[] { i.ToString() }
            });

            return new KInt() {
                ObjName = i.ToString()
            };
        }
        #endregion
    }
}
