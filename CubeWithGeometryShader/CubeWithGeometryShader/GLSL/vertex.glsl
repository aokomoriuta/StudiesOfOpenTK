#version 330

//! input: position of vertex
in vec3 position;

//! entry point
void main()
{
	//! set position of vertex
	gl_Position = vec4(position, 1.0);
}