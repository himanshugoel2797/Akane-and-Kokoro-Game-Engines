using System;
using Kokoro.KSL;
using Kokoro.KSL.Lib.General;
using Kokoro.KSL.Lib.Math;
using Kokoro.KSL.Lib.Texture;
using Kokoro.KSL.Lib;

namespace Akane
{
	public class DefaultShader : IKShaderProgram
	{
		public DefaultShader ()
		{
		}

		#region IKShaderProgram implementation

		public void Fragment ()
		{
			var Vars = Manager.ShaderStart();
			Manager.SharedIn<Vec2>("UV");
			Manager.StreamOut<Vec4>("Color", 0);

			Vars.Color = Texture.Read2D(Vars.ColorMap, Vars.UV);

			Manager.ShaderEnd();
		}

		public void Vertex ()
		{
			dynamic Variables = Manager.ShaderStart();
			Manager.StreamIn<Vec3>("VertexPos", 0);
			Manager.StreamIn<Vec2>("UV0", 1);

			Manager.SharedOut<Vec2>("UV");
			Manager.Create<Mat4>("MVP");

			Variables.MVP = Variables.Projection * Variables.View * Variables.World;
			Variables.VertexPosition.Construct(Variables.VertexPos, 1);
			Variables.VertexPosition *= Variables.MVP;
			Variables.UV = Variables.UV0;


			Manager.ShaderEnd();
		}

		public static HLShader Create(ShaderTypes t)
		{
			return new HLShader()
			{
				Shader = new DefaultShader(),
				ShaderType = t
			};
		}
		#endregion
	}
}

