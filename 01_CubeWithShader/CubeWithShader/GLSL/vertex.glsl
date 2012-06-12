#version 330

//! input: position of vertex
in vec3 position;

//! input: normal vector of vertex
in vec3 normal;


//! input: view (camera)
uniform mat4 view;

//! input: projection matrix
uniform mat4 projection;


//! output: normal vector after vertex shader
out vec3 vertexNormal;


//! entry point
void main()
{
	//! calculate position by view, projection and position of vertex
	gl_Position = projection*view * vec4(position, 1.0);

	//! set normal vector same as input
	vertexNormal = normal;
}