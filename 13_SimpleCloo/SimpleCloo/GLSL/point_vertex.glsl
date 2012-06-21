#version 330

//! input: position of particle
in vec3 particleX;

//! input: color of particle
in vec4 particleColor;

//! input: length of a edge of particle
in float particleD;

//! output: particle data
out Particle
{
	//! center position
	vec4 X;

	//! length of a edge
	float D;

	//! color
	vec4 Color;
} particleVertex;

//! entry point
void main()
{
	//! set position of particle
	particleVertex.X = vec4(particleX, 1.0);

	//! set size of particle
	particleVertex.D = particleD;

	//! set color of particle
	particleVertex.Color = particleColor;
}