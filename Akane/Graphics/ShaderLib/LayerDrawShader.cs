using System;
using Kokoro.KSL;
using Kokoro.KSL.Lib;
using Kokoro.KSL.Lib.General;
using Kokoro.KSL.Lib.Math;
using Kokoro.KSL.Lib.Texture;

namespace Akane
{
	public class LayerDrawShader : IKShaderProgram
	{
		public LayerDrawShader ()
		{
		}

		#region IKShaderProgram implementation

		public void Fragment ()
		{
			throw new NotImplementedException ();
		}

		public void Vertex ()
		{
			var Vars = Manager.ShaderStart();

			Manager.ShaderEnd();
		}

		#endregion
	}
}

