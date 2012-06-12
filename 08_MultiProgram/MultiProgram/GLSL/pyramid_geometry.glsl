#version 330

layout(points) in; 
layout(triangle_strip, max_vertices=24) out;

//! input: view (camera)
uniform mat4 view;

//! input: projection matrix
uniform mat4 projection;

//! input: cube data
in Cube
{
	//! center position
	vec4 Position;

	//! length of a edge
	float Size;

	//! color
	vec4 Color;
} cubeVertex[];

//! output: vertex data
out Vertex
{
	// normal vector of this plane
	vec3 Normal;

	// color of this plane
	vec4 Color;
} vertexGeometry;

//! projection and model matrix
mat4 projectionView;

//! Draw a plane
/*!
	\param p0 first position of vertex of this plane
	\param p1 secont position of vertex of this plane
	\param p2 third position of vertex of this plane
	\param p3 forth position of vertex of this plane
	\param color color of this plane
*/
void DrawPlane(vec4 p0, vec4 p1, vec4 p2, vec4 p3, vec4 color)
{
	vec3 normal1 = cross((p0 - p2).xyz, (p1 - p2).xyz);
	vec3 normal2 = cross((p1 - p2).xyz, (p3 - p2).xyz);
	vec3 normal = normalize(normal1 + normal2);
	normal = normalize(normal1);

	// point 1
	gl_Position = projectionView* p0;
	vertexGeometry.Normal = normal;
	vertexGeometry.Color = color;

	EmitVertex();

	// point 2
	gl_Position = projectionView* p1;
	vertexGeometry.Normal = normal;
	vertexGeometry.Color = color;
	EmitVertex();

	// point 3
	gl_Position = projectionView* p2;
	vertexGeometry.Normal = normal;
	vertexGeometry.Color = color;
	EmitVertex();

	// point 4
	gl_Position = projectionView* p3;
	vertexGeometry.Normal = normal;
	vertexGeometry.Color = color;
	EmitVertex();

	EndPrimitive();
}

//! entry point
void main(void)
{
	// calculate projection and model view matrix
	projectionView = projection*view;

	// get center position of cube
	vec4 center = cubeVertex[0].Position;

	// half size of cube
	float size = cubeVertex[0].Size/2;

	// get color of cube
	vec4 color = cubeVertex[0].Color;

	// calculate each position of vertex of cube
	vec4 position[5];
	position[0] = center + vec4(-1, -1, -1, 0) * size;
	position[1] = center + vec4( 1, -1, -1, 0) * size;
	position[2] = center + vec4(-1,  1, -1, 0) * size;
	position[3] = center + vec4( 1,  1, -1, 0) * size;
	position[4] = center + vec4( 0,  0,  1, 0) * size;

	// -z plane
	DrawPlane(
		position[0],
		position[2],
		position[1],
		position[3],
		color
		);

	// -x plane
	DrawPlane(
		position[0],
		position[4],
		position[2],
		position[4],
		color
		);

	// +x plane
	DrawPlane(
		position[1],
		position[3],
		position[4],
		position[4],
		color
		);

	// -y plane
	DrawPlane(
		position[0],
		position[1],
		position[4],
		position[4],
		color
		);

	// +y plane
	DrawPlane(
		position[2],
		position[4],
		position[3],
		position[4],
		color
		);
}