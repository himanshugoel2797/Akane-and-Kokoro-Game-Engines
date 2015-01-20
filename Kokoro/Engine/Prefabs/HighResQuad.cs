﻿using System;
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
    public class HighResQuad : Model
    {
        public HighResQuad(float x0, float y0, int terrainWidth, int terrainHeight, Texture tex = null)
        {
            filepath = "";
            vbufs = new VertexBufferLL[1];
            vbufs[0] = new VertexBufferLL();

            Materials = new Material[1];

            Vector3[] vertices = new Vector3[terrainWidth * terrainHeight];
            Vector2[] uvs = new Vector2[terrainWidth * terrainHeight];
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    //vertices.AddRange(new float[] { x, 0, -y });
                    vertices[x + y * terrainWidth] = new Vector3(x, 0, -y);

                    uvs[(x + y * terrainWidth)] = new Vector2((float)x / (float)terrainWidth, (float)y / (float)terrainHeight);
                }
            }


            uint[] indices = new uint[(terrainWidth - 1) * (terrainHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < terrainHeight - 1; y++)
            {
                for (int x = 0; x < terrainWidth - 1; x++)
                {
                    int lowerLeft = x + y * terrainWidth;
                    int lowerRight = (x + 1) + y * terrainWidth;
                    int topLeft = x + (y + 1) * terrainWidth;
                    int topRight = (x + 1) + (y + 1) * terrainWidth;

                    indices[counter++] = (uint)topLeft;
                    indices[counter++] = (uint)lowerRight;
                    indices[counter++] = (uint)lowerLeft;

                    indices[counter++] = (uint)topLeft;
                    indices[counter++] = (uint)topRight;
                    indices[counter++] = (uint)lowerRight;
                }
            }

            List<float> verts = new List<float>();
            List<float> uv = new List<float>();
            List<float> norms = new List<float>();
            for (int i = 0; i < vertices.Length; i++)
            {
                verts.AddRange(new float[] { vertices[i].X, vertices[i].Y, vertices[i].Z });
                uv.AddRange(new float[] { uvs[i].X, uvs[i].Y });
                norms.AddRange(new float[] { 0, 1, 0 });
            }

            vbufs[0].SetIndices(indices);
            vbufs[0].SetUVs(uv.ToArray());
            vbufs[0].SetVertices(verts.ToArray());
            vbufs[0].SetNormals(norms.ToArray());
            vbufs[0].DrawMode = DrawMode.Triangles;
            Materials[0] = new Material { Diffuse = tex };


            World = Matrix4.Identity;
        }

    }
}