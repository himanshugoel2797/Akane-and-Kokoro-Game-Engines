using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace Kokoro.ColladaLL
{

	[System.SerializableAttribute()]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
	public partial class Grendgine_Collada_Common_Param_Type
	{
		[XmlElement(ElementName = "param")]
		public Grendgine_Collada_Param Param;	
	}
}
