#version 330

//! input: plane color
in vec3 planeColor;

//! output: color
out vec4 outColor;

//! entry point
void main()
{
	//! set diffusing color
	outColor = vec4(planeColor, 1);
}