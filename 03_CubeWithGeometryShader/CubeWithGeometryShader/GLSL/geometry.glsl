#version 330
#extension GL_EXT_geometry_shader4 : enable

layout(points) in; 
layout(triangle_strip, max_vertices=24) out;

//! input: view (camera)
uniform mat4 view;

//! input: projection matrix
uniform mat4 projection;

//! input: size of cube
uniform float cubeSize;

//! output: planeColor;
out vec3 planeColor;


//! projection and model matrix
mat4 projectionView;

void DrawPlane(vec4 p0, vec4 p1, vec4 p2, vec4 p3, vec3 color)
{
	// point 1
	gl_Position = projectionView* p0;
	planeColor = color;
	EmitVertex();

	// point 2
	gl_Position = projectionView* p1;
	planeColor = color;
	EmitVertex();

	// point 3
	gl_Position = projectionView* p2;
	planeColor = color;
	EmitVertex();

	// point 4
	gl_Position = projectionView* p3;
	planeColor = color;
	EmitVertex();

	EndPrimitive();
}

//! entry point
void main(void)
{
	// calculate projection and model view matrix
	projectionView = projection*view;

	// get center position of cube
	vec4 center = gl_in[0].gl_Position;

	// half size of cube
	float size = cubeSize/2;

	// calculate each position of vertex of cube
	vec4 position[8];
	position[0] = center + vec4(-1, -1, -1, 0) * size;
	position[1] = center + vec4( 1, -1, -1, 0) * size;
	position[2] = center + vec4(-1,  1, -1, 0) * size;
	position[3] = center + vec4( 1,  1, -1, 0) * size;
	position[4] = center + vec4(-1, -1,  1, 0) * size;
	position[5] = center + vec4( 1, -1,  1, 0) * size;
	position[6] = center + vec4(-1,  1,  1, 0) * size;
	position[7] = center + vec4( 1,  1,  1, 0) * size;

	// -z plane
	DrawPlane(
		position[0],
		position[1],
		position[2],
		position[3],
		vec3(1, 0, 0)
		);

	// +z plane
	DrawPlane(
		position[4],
		position[5],
		position[6],
		position[7],
		vec3(0.5, 0, 0)
		);

	// -x plane
	DrawPlane(
		position[0],
		position[2],
		position[4],
		position[6],
		vec3(0, 1, 0)
		);

	// +x plane
	DrawPlane(
		position[1],
		position[3],
		position[5],
		position[7],
		vec3(0, 0.5, 0)
		);

	// -y plane
	DrawPlane(
		position[0],
		position[1],
		position[4],
		position[5],
		vec3(0, 0, 1)
		);

	// +y plane
	DrawPlane(
		position[2],
		position[3],
		position[6],
		position[7],
		vec3(0, 0, 0.5)
		);
}