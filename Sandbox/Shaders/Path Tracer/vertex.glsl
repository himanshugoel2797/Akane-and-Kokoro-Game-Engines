#version 330 core
layout(location = 0) in vec3 VertexPos;
layout(location = 2) in vec2 UV0;

uniform sampler2D ColorMap;
uniform sampler2D LightingMap;
uniform sampler2D NormalMap;
uniform mat4 World;
uniform mat4 View;
uniform mat4 Projection;
uniform float ZNear;
uniform float ZFar;

out vec2 UV;


void main(){
gl_Position.xyz = VertexPos;
gl_Position.w = 1;
UV = UV0;
}
