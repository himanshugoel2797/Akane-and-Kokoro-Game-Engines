using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Math;

#if OPENGL
#if PC
using Kokoro.OpenGL.PC;
#endif
#endif

namespace Kokoro.Engine.Prefabs
{
    public class Sphere : Model
    {
        public Sphere(float radius, int step = 10) : base()
        {

            this.DrawMode = DrawMode.Triangles;

            List<float> verts = new List<float>();
            List<float> uvs = new List<float>();
            List<float> normals = new List<float>();
            List<uint> indices = new List<uint>();
            World = Matrix4.Identity;

            float angleStep = 360f / (float)step;
            double toRad = MathHelper.Pi / 180;

            uint n = 0;
            for (float aY = 0; aY < 180; aY += angleStep)
            {
                for (float aX = 0; aX < 360; aX += angleStep)
                {
                    float x = (float)(radius * System.Math.Cos(aX * toRad) * System.Math.Sin(aY * toRad));
                    float y = (float)(radius * System.Math.Sin(aX * toRad) * System.Math.Sin(aY * toRad));
                    float z = (float)(radius * System.Math.Cos(aY * toRad));

                    verts.Add(x);
                    verts.Add(y);
                    verts.Add(z);

                    float uvX = aX / 360;
                    float uvY = (2 * aY) / 360;
                    uvs.Add(uvX);
                    uvs.Add(uvY);

                    var normal = new Vector3(x, y, z);
                    normal.Normalize();
                    normals.Add(normal.X);
                    normals.Add(normal.Y);
                    normals.Add(normal.Z);

                    indices.Add(n);
                    n++;


                    x = (float)(radius * System.Math.Cos(aX * toRad) * System.Math.Sin((aY + angleStep) * toRad));
                    y = (float)(radius * System.Math.Sin(aX * toRad) * System.Math.Sin((aY + angleStep) * toRad));
                    z = (float)(radius * System.Math.Cos((aY + angleStep) * toRad));

                    verts.Add(x);
                    verts.Add(y);
                    verts.Add(z);

                    normal = new Vector3(x, y, z);
                    normal.Normalize();
                    normals.Add(normal.X);
                    normals.Add(normal.Y);
                    normals.Add(normal.Z);

                    uvX = aX / 360;
                    uvY = (2 * (aY + angleStep)) / 360;
                    uvs.Add(uvX);
                    uvs.Add(uvY);

                    indices.Add(n);
                    n++;


                    x = (float)(radius * System.Math.Cos((aX + angleStep) * toRad) * System.Math.Sin(aY * toRad));
                    y = (float)(radius * System.Math.Sin((aX + angleStep) * toRad) * System.Math.Sin(aY * toRad));
                    z = (float)(radius * System.Math.Cos(aY * toRad));

                    verts.Add(x);
                    verts.Add(y);
                    verts.Add(z);

                    normal = new Vector3(x, y, z);
                    normal.Normalize();
                    normals.Add(normal.X);
                    normals.Add(normal.Y);
                    normals.Add(normal.Z);

                    uvX = (aX + angleStep) / 360;
                    uvY = (2 * aY) / 360;
                    uvs.Add(uvX);
                    uvs.Add(uvY);

                    indices.Add(n);
                    n++;

                    x = (float)(radius * System.Math.Cos((aX + angleStep) * toRad) * System.Math.Sin((aY + angleStep) * toRad));
                    y = (float)(radius * System.Math.Sin((aX + angleStep) * toRad) * System.Math.Sin((aY + angleStep) * toRad));
                    z = (float)(radius * System.Math.Cos((aY + angleStep) * toRad));

                    verts.Add(x);
                    verts.Add(y);
                    verts.Add(z);

                    normal = new Vector3(x, y, z);
                    normal.Normalize();
                    normals.Add(normal.X);
                    normals.Add(normal.Y);
                    normals.Add(normal.Z);

                    uvX = (aX + angleStep) / 360;
                    uvY = (2 * (aY + angleStep)) / 360;
                    uvs.Add(uvX);
                    uvs.Add(uvY);

                    indices.Add(n);
                    indices.Add((uint)(n - 1));
                    indices.Add((uint)(n - 2));
                    n++;

                }
            }

            vbufs = new VertexBufferLL[1];
            vbufs[0] = new VertexBufferLL();

            vbufs[0].SetIndices(indices.ToArray());
            vbufs[0].SetUVs(uvs.ToArray());
            vbufs[0].SetVertices(verts.ToArray());
            vbufs[0].SetNormals(normals.ToArray());
        }
    }
}
