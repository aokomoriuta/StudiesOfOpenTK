#version 330

//! input: normal vector of vertex
in vec3 vertexNormal;

//! input: direction of light
uniform vec3 lightDirection;

//! output: color
out vec4 outColor;


//! entry point
void main()
{
	//! calulate diffusing color by normal vector of vertex and direction of light
	outColor = vec4(max(0.0,dot(vertexNormal,-lightDirection)));
	
	//! full opacity
	outColor.a = 1.0;
}