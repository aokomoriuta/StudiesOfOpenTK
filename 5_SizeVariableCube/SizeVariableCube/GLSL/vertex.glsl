#version 330

//! input: position of cube
in vec3 cubePosition;

//! input: length of a edge of cube
in float cubeSize;

//! output: cube data
out Cube
{
	//! center position of cube
	vec4 position;

	//! length of a edge
	float size;
} CubeOut;

//! entry point
void main()
{
	//! set position of cube
	CubeOut.position = vec4(cubePosition, 1.0);

	//! set size of cube
	CubeOut.size = cubeSize;
}