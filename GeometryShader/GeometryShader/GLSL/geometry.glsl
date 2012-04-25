#version 330
#extension GL_EXT_geometry_shader4 : enable


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
	gl_Position = projectionView*gl_PositionIn[0];
	gl_PointSize = 10;
	EmitVertex();
	EndPrimitive();

	// copied point
	gl_Position = projectionView*(gl_PositionIn[0] + vec4(2, 0, 0, 0));
	gl_PointSize = 3;
	EmitVertex();
	EndPrimitive();
}