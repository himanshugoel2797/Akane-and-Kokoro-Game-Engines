in vec2 UV;
in float depth;
in vec3 normPos;

layout(location = 0) out vec4 NormalE0;
layout(location = 1) out vec4 RGBA0;
layout(location = 2) out vec4 Depth0;

layout(location = 3) out vec4 NormalE1;
layout(location = 4) out vec4 RGBA1;
layout(location = 5) out vec4 Depth1;

//layout(location = 6) out vec4 NormalE2;
//layout(location = 7) out vec4 RGBA2;
//layout(location = 8) out vec4 Depth2;

//layout(location = 9) out vec4 NormalE3;
//layout(location = 10) out vec4 RGBA3;
//layout(location = 11) out vec4 Depth3;

//layout(location = 12) out vec4 NormalE4;
//layout(location = 13) out vec4 RGBA4;
//layout(location = 14) out vec4 Depth4;

uniform sampler2D ColorMap;
uniform sampler2D LightingMap;
uniform sampler2D NormalMap;
uniform float Emissivity;

vec4 EncodeFloatRGBA( float v ) {
  vec4 enc = vec4(1.0, 255.0, 65025.0, 16581375.0) * v;
  enc = fract(enc);
  enc -= enc.yzww * vec4(1.0/255.0,1.0/255.0,1.0/255.0,0.0);
  return enc;
}

float DecodeFloatRGBA( vec4 rgba ) {
  return dot( rgba, vec4(1.0, 1/255.0, 1/65025.0, 1/160581375.0) );
}

void main()
{
	if(depth <= 0.2f)
	{
		NormalE0 = vec4(normPos, 1);
		RGBA0 = texture2D(ColorMap, UV);
		Depth0 = EncodeFloatRGBA(depth);
		//Depth0 = vec4(vec3(depth), 1);
	}
	else if(depth > 0.2f)
	{
		NormalE1 = vec4(normPos, 1);
		RGBA1 = texture2D(ColorMap, UV);
		Depth1 = EncodeFloatRGBA(depth - 0.2f);
		//Depth1 = vec4(vec3(depth - 0.3f), 1);
	}
	//else if(depth <= 0.9f)
	//{
	//	NormalE2 = vec4(normPos, Emissivity);
	//	RGBA2 = texture2D(ColorMap, UV);
	//	Depth2 = EncodeFloatRGBA(depth);
	//}
	//else if(depth <= 1.0f)
	//{
	//	NormalE3 = vec4(normPos, Emissivity);
	//	RGBA3 = texture2D(ColorMap, UV);
	//	Depth3 = EncodeFloatRGBA(depth);
	//}
	//else if(depth >= 1.0f)
	//{
	//	NormalE4 = vec4(normPos, Emissivity);
	//	RGBA4 = texture2D(ColorMap, UV);
	//	Depth4 = EncodeFloatRGBA(depth - 1.0f);
	//}
}