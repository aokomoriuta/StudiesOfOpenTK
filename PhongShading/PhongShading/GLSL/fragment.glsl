#version 330

//! output: vertex data
in Vertex
{
	// normal vector of this plane
	vec3 Normal;

	// color of this plane
	vec3 Color;
} vertexGeometry;

//! output: color
out vec4 outColor;


//! input: strength of ambient 
uniform float ambient;

//! input: direction vector of light
uniform vec3 light;

//! entry point
void main()
{
	// calculate strength of dissusing
	float diffuse = max(0, dot(vertexGeometry.Normal, -normalize(light)) );

	// set diffusing color
	outColor = vec4((ambient + (1-ambient)*diffuse)* vertexGeometry.Color, 1);
}