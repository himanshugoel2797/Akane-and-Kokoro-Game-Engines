using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Kokoro.Math;
using Kokoro.Engine.Shaders;

namespace Kokoro.Engine
{
    public struct Material
    {
        public string Name { get; set; }
        public Texture Diffuse { get; set; }
        public Texture Specular { get; set; }
        public Texture NormalMap { get; set; }
        public ShaderProgram Shader { get; set; }

        public float Lit { get; set; }
        public float FresnelTerm { get; set; }
        public float DiffuseReflectivity { get; set; }

        public bool Bloom { get; set; }
        public float Reflectivity { get; set; }

        /// <summary>
        /// Parse shader parameters from the material file
        /// </summary>
        /// <param name="s">The Shader whose parameters need to be set</param>
        /// <param name="line">The string to parse</param>
        /// <returns></returns>
        private static ShaderProgram ParseParams(ShaderProgram s, string line)
        {
            //Handle all vectors
            if (line.StartsWith("Vec", StringComparison.InvariantCultureIgnoreCase))
            {
                string varName = line.Replace("Vec", "").Split('=')[0].Trim().Substring(1);

                string[] rep = line.Split('{')[1].Replace("}", "").Split(',');
                Vector4 vec = new Vector4();
                for (int i = 0; i < rep.Length; i++)
                {
                    if (i == 0) vec.X = float.Parse(rep[i]);
                    else if (i == 1) vec.Y = float.Parse(rep[i]);
                    else if (i == 2) vec.Z = float.Parse(rep[i]);
                    else if (i == 3) vec.W = float.Parse(rep[i]);
                }

                if (rep.Length == 1) s.SetShaderVector(varName, vec.Xy);
                else if (rep.Length == 2) s.SetShaderVector(varName, vec.Xyz);
                else if (rep.Length == 3) s.SetShaderVector(varName, vec);
            }
            else if (line.StartsWith("float", StringComparison.InvariantCultureIgnoreCase))
            {
                string varName = line.Replace("float", "").Split('=')[0].Trim();
                s.SetShaderFloat(varName, float.Parse(line.Split('=')[1].Trim()));
            }

            return s;
        }

        /// <summary>
        /// Load a Material from a file
        /// </summary>
        /// <param name="filename">The file to load the material from</param>
        /// <returns></returns>
        public static Material Load(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            List<string> parameters = new List<string>();

            string name = lines[0].Replace("[Name]", "");
            string diffuse = "", specular = "", ambient = "", vshader = "", fshader = "";
            VertexShader vshdr = null;
            FragmentShader fshdr = null;
            ShaderProgram s = null;

            for (int i = 1; i < lines.Length; i++)
            {
                switch (lines[i].Split(']')[0])
                {
                    case "[Diffuse":
                        diffuse = lines[i].Split(']')[1].Trim();
                        break;
                    case "[Specular":
                        specular = lines[i].Split(']')[1].Trim();
                        break;
                    case "[NormalMap":
                        ambient = lines[i].Split(']')[1].Trim();
                        break;
                    case "[Vertex Shader":
                        vshader = lines[i].Split(']')[1].Trim();
                        vshdr = new VertexShader(vshader);
                        break;
                    case "[Fragment Shader":
                        fshader = lines[i].Split(']')[1].Trim();
                        fshdr = new FragmentShader(fshader);
                        break;
                    case "[Vertex Shader Params":
                        parameters.Add(lines[i].Split(']')[1].Trim());
                        //vshdr = (VertexShader)ParseParams(vshdr, lines[i].Split(']')[1].Trim());
                        break;
                    case "[Fragment Shader Params":
                        parameters.Add(lines[i].Split(']')[1].Trim());
                        //fshdr = (FragmentShader)ParseParams(fshdr, lines[i].Split(']')[1].Trim());
                        break;
                    default:
                        break;
                }
            }

            if (s == null) s = new ShaderProgram(vshdr, fshdr);

            for (int i = 0; i < parameters.Count; i++)
            {
                s = ParseParams(s, parameters[i]);
            }

            Material mat = new Material()
            {
                Shader = s,
                Specular = new Texture(specular),
                NormalMap = new Texture(ambient),
                Diffuse = new Texture(diffuse),
                Name = name
            };
            return mat;
        }

    }
}
