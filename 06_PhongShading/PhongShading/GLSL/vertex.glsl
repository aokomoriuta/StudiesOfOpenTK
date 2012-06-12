#version 330

//! input: position of cube
in vec3 cubePosition;

//! input: length of a edge of cube
in float cubeSize;

//! input: color of cube
in vec4 cubeColor;

//! output: cube data
out Cube
{
	//! center position
	vec4 Position;

	//! length of a edge
	float Size;

	//! color
	vec4 Color;
} cubeVertex;

//! entry point
void main()
{
	//! set position of cube
	cubeVertex.Position = vec4(cubePosition, 1.0);

	//! set size of cube
	cubeVertex.Size = cubeSize;

	//! set color of cube
	cubeVertex.Color = cubeColor;
}