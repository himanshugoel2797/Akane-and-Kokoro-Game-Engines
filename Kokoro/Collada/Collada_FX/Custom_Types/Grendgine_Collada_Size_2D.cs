using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace Kokoro.ColladaLL
{

	[System.SerializableAttribute()]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
	public partial class Grendgine_Collada_Size_2D
	{

		[XmlAttribute("width")]
		public int Width;	

		[XmlAttribute("height")]
		public int Height;	
	}
}

