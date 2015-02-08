﻿

// Interpolated values from the vertex shaders
in vec2 UV;

// Ouput data
out vec4 color;

// Values that stay constant for the whole mesh.
uniform sampler2D ColorMap;

uniform vec3 TransparentColor;

void main(){

	// Output color = color of the texture at the specified UV
	color = texture2D( ColorMap, UV );
	if( all(equal(color.rgb, TransparentColor)) )color.a = 0;
}