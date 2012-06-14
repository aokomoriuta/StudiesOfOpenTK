#version 330

layout(points) in; 
layout(points, max_vertices=1) out;

//! input: view matrix (camera)
uniform mat4 view;

//! input: projection matrix
uniform mat4 projection;

uniform float size;

//! input: particle data
in Particle
{
	//! center position
	vec4 X;

	//! length of a edge
	float D;

	//! color
	vec4 Color;
} particleVertex[];

//! output: particle data
out Particle
{
	//! center position
	vec4 X;

	//! color
	vec4 Color;
} particleGeometry;

//! projection and model matrix
mat4 projectionView;

//! entry point
void main(void)
{
	// calculate projection and model view matrix
	projectionView = projection*view;

	// set position and size of this point
	gl_Position = projectionView* particleVertex[0].X;
	gl_PointSize = particleVertex[0].D*size;

	// set output data
	particleGeometry.X     = particleVertex[0].X;
	particleGeometry.Color = particleVertex[0].Color;

	// end of point
	EmitVertex();
	EndPrimitive();
}