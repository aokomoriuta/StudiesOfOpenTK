#version 330

//! output: color of line
in vec4 linecolor;

//! output: color
out vec4 outColor;

//! entry point
void main()
{
	// set color
	outColor = linecolor;
}