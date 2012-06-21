#version 330

layout(points) in; 
layout(line_strip, max_vertices=30) out; 

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
	// first
	gl_Position = projectionView*vec4(p0, 1);
	linecolor = vec4(color, 1);
	EmitVertex();

	// last
	gl_Position = projectionView*vec4(p1, 1);
	linecolor = vec4(color, 1);
	EmitVertex();

	EndPrimitive();
}

//! entry point
void main(void)
{
	// calculate projection and model view matrix
	projectionView = projection*view;

	// color of each axis
	vec3 color[3];
	color[0] = vec3(1.0f, 0.0f, 0.0f);
	color[1] = vec3(0.0f, 1.0f, 0.0f);
	color[2] = vec3(0.0f, 0.0f, 1.0f);

	// points of each axis
	vec3 position[2*3];
	position[0] = lookat + vec3(-1,  0,  0) * size;
	position[1] = lookat + vec3( 1,  0,  0) * size;
	position[2] = lookat + vec3( 0, -1,  0) * size;
	position[3] = lookat + vec3( 0,  1,  0) * size;
	position[4] = lookat + vec3( 0,  0, -1) * size;
	position[5] = lookat + vec3( 0,  0,  1) * size;

	// size of tip
	float tipSize = size*0.1;

	// location of points of tip
	vec3 tipVertex[4*3];
	tipVertex[ 0] = position[1] + vec3(-1,  1,  1) * tipSize;
	tipVertex[ 1] = position[1] + vec3(-1, -1,  1) * tipSize;
	tipVertex[ 2] = position[1] + vec3(-1, -1, -1) * tipSize;
	tipVertex[ 3] = position[1] + vec3(-1,  1, -1) * tipSize;
	tipVertex[ 4] = position[3] + vec3( 1, -1,  1) * tipSize;
	tipVertex[ 5] = position[3] + vec3(-1, -1,  1) * tipSize;
	tipVertex[ 6] = position[3] + vec3(-1, -1, -1) * tipSize;
	tipVertex[ 7] = position[3] + vec3( 1, -1, -1) * tipSize;
	tipVertex[ 8] = position[5] + vec3( 1,  1, -1) * tipSize;
	tipVertex[ 9] = position[5] + vec3(-1,  1, -1) * tipSize;
	tipVertex[10] = position[5] + vec3(-1, -1, -1) * tipSize;
	tipVertex[11] = position[5] + vec3( 1, -1, -1) * tipSize;

	// draw each axis line
	DrawLine(position[0], position[1], color[0]);
	DrawLine(position[2], position[3], color[1]);
	DrawLine(position[4], position[5], color[2]);

	// draw each tips of axis
	DrawLine(tipVertex[ 0], position[1], color[0]);
	DrawLine(tipVertex[ 1], position[1], color[0]);
	DrawLine(tipVertex[ 2], position[1], color[0]);
	DrawLine(tipVertex[ 3], position[1], color[0]);
	DrawLine(tipVertex[ 4], position[3], color[1]);
	DrawLine(tipVertex[ 5], position[3], color[1]);
	DrawLine(tipVertex[ 6], position[3], color[1]);
	DrawLine(tipVertex[ 7], position[3], color[1]);
	DrawLine(tipVertex[ 8], position[5], color[2]);
	DrawLine(tipVertex[ 9], position[5], color[2]);
	DrawLine(tipVertex[10], position[5], color[2]);
	DrawLine(tipVertex[11], position[5], color[2]);
	
}