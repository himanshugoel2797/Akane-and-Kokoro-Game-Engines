

// Interpolated values from the vertex shaders
in vec2 UV;

// Ouput data
layout(location = 0) out vec4 color;

// Values that stay constant for the whole mesh.
uniform sampler2D diffuse;
uniform sampler2D specular;
uniform sampler2D ambient;
uniform vec4 col;
uniform float mode; 


void main(){

	color = vec4(1);

	// Output color = color of the texture at the specified UV
	if(mode == 1)color = col;
	else {
	color = texture2D( diffuse, UV ) + (vec4(1) * texture2D(specular, UV) );
	}

	if(mode == 2){
	color *= texture2D(ambient, UV);
	}
}