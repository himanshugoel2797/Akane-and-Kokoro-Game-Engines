using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL.Lib.Math
{
    public class Vec4 : Obj
    {
        public void Assign(KInt k)
        {
            SyntaxTree.Instructions.Enqueue(new SyntaxTree.Instruction()
            {
                instructionType = SyntaxTree.InstructionType.Assign,
                Parameters = new string[] { this.ObjName, k.ObjName }
            });
        }

        public void Assign(Vec4 k)
        {
            SyntaxTree.Instructions.Enqueue(new SyntaxTree.Instruction()
            {
                instructionType = SyntaxTree.InstructionType.Assign,
                Parameters = new string[] { this.ObjName, k.ObjName }
            });
        }

    }
}
