//! input: position of vertex
attribute vec3 position;

//! input: normal vector of vertex
attribute vec3 normal;

//! input: view (camera)
uniform mat4 view;

//! input: projection matrix
uniform mat4 projection;

//! input: direction of light
uniform vec3 lightDirection;

//! output: diffusing color
varying vec4 diffuseColor;

//! entry point
void main()
{
	//! calculate position by view, projection and position of vertex
	gl_Position = projection*view * vec4(position, 1.0);

	//! calulate diffusing color by normal vector of vertex and direction of light
	diffuseColor = vec4(max(0,dot(normal,-lightDirection)));
	
	//! full opacity
	diffuseColor.a = 1.0;
}
