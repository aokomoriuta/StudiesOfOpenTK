#version 330

layout(points) in; 
layout(line_strip, max_vertices=6) out; 

//! input: view (camera)
uniform mat4 view;

//! input: projection matrix
uniform mat4 projection;

//! input: length of line
uniform float size;

//! input: position where I look at
uniform vec3 lookat;

//! output: color of line
out vec4 linecolor;


//! projection and model matrix
mat4 projectionView;

//! Draw a line
/*!
	\param p0 stard position of vertex of this line
	\param p1 end position of vertex of this line
	\param color color of this line
*/
void DrawLine(vec3 p0, vec3 p1, vec3 color)
{
	//
	gl_Position = projectionView*vec4(p0, 1);
	gl_PointSize = 5;
	linecolor = vec4(color, 1);
	EmitVertex();

	gl_Position = projectionView*vec4(p1, 1);
	gl_PointSize = 5;
	linecolor = vec4(color, 1);
	EmitVertex();

	EndPrimitive();
}

//! entry point
void main(void)
{
	// calculate projection and model view matrix
	projectionView = projection*view;

	vec3 position[6];
	position[0] = lookat + vec3(-1,  0,  0) * size;
	position[1] = lookat + vec3( 1,  0,  0) * size;
	position[2] = lookat + vec3( 0, -1,  0) * size;
	position[3] = lookat + vec3( 0,  1,  0) * size;
	position[4] = lookat + vec3( 0,  0, -1) * size;
	position[5] = lookat + vec3( 0,  0,  1) * size;

	DrawLine(position[0], position[1], vec3(1.0f, 0.0f, 0.0f));
	DrawLine(position[2], position[3], vec3(0.0f, 1.0f, 0.0f));
	DrawLine(position[4], position[5], vec3(0.0f, 0.0f, 1.0f));
}