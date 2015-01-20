

// Input vertex data, different for all executions of this shader.
layout(location = 0) in vec3 vertexPosition_modelspace;
layout(location = 1) in vec2 vertexUV;
layout(location = 2) in vec3 normal;

// Output data ; will be interpolated for each fragment.
out vec2 UV;

// Values that stay constant for the whole mesh.
uniform mat4 Model;
uniform mat4 View;
uniform mat4 Proj;
uniform mat4 Normal;

uniform float zNear;
uniform float zFar;

smooth out vec3 vertexPos;
smooth out vec3 normalPos;
smooth out float depth;

void main(){

	// Output position of the vertex, in clip space : MVP * position
	mat4 MVP = Proj * View * Model;
	gl_Position =  MVP * vec4(vertexPosition_modelspace,1);
	
	vertexPos = (MVP * vec4(vertexPosition_modelspace, 1)).xyz;
	normalPos = (Model * vec4(normal, 0)).xyz;

	depth = (gl_Position.z * gl_Position.w - zNear)/(zFar - zNear);

	// UV of the vertex. No special space for this one.
	UV = vertexUV;
}