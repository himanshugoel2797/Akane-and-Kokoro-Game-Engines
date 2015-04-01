using Kokoro.KSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.Shaders
{
    public class Ubershader
    {
        public List<IKShaderProgram> Subroutines { get; set; }
        public IKUbershader UberMain { get; set; }

        public Ubershader(IKUbershader uber)
        {
            this.UberMain = uber;
            this.Subroutines = new List<IKShaderProgram>();
        }

        public Ubershader(object vertex, object fragment)
        {
            this.UberMain = new DynUbershader((Action)vertex, (Action<int>)fragment);
            this.Subroutines = new List<IKShaderProgram>();
        }

        public static implicit operator ShaderProgram(Ubershader s)
        {
            return new ShaderProgram(s);
        }


        //Allows for the creation of Ubershaders from multiple pieces
        class DynUbershader : IKUbershader
        {
            Action vert;
            Action<int> frag;
            public DynUbershader(Action vertex, Action<int> fragment)
            {
                vert = vertex;
                frag = fragment;
            }

            public void Vertex()
            {
                vert();
            }

            public void Fragment(int num)
            {
                frag(num);
            }
        }
    }
}
