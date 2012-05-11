#version 330
#extension GL_EXT_geometry_shader4 : enable

layout(points) in; 
layout(triangle_strip, max_vertices=4) out;

//! input: view (camera)
uniform mat4 view;

//! input: projection matrix
uniform mat4 projection;

//! entry point
void main(void)
{
	// calculate projection and model view matrix
	mat4 projectionView = projection*view;

	// original point
	gl_Position = projectionView* gl_in[0].gl_Position;
	EmitVertex();

	// other points1
	gl_Position = projectionView* (gl_in[0].gl_Position + vec4(1, 0, 0, 0));
	EmitVertex();

	// other points2
	gl_Position = projectionView* (gl_in[0].gl_Position + vec4(0, 1, 0, 0));
	EmitVertex();

	// other points3
	gl_Position = projectionView* (gl_in[0].gl_Position + vec4(1, 1, 0, 0));
	EmitVertex();

	EndPrimitive();
}