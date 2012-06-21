#version 330

//! input: particle data
in Particle
{
	//! center position
	vec4 X;

	//! color
	vec4 Color;
} particleGeometry;

//! output: color
out vec4 outColor;


//! input: projection matrix
uniform mat4 projection;

//! input: view maxrix (camera)
uniform mat4 view;

//! input: strength of ambient 
uniform float ambient;

//! input: direction vector of light
uniform vec3 sunLight;

//! input: strength of light from camera
uniform float cameraLight;

//! input: position of camera
uniform vec3 cameraPosition;

//! entry point
void main()
{
	// calcluate relative unit vector from particle's center in screen
	vec2 coord = gl_PointCoord * 2.0 - 1.0;
	float r2 = dot(coord.xy, coord.xy);

	// if this point is out of the circle
	if (r2 > 1)
	{
		// disable this point
		discard;
	}

	// calculate normal vector in screen
	vec3 normal = vec3(-coord.x, coord.y, -sqrt(1 - r2));

	// calculate normal vector in computational space
	normal = normalize((inverse(view)*vec4(normal, 0)).xyz);
	
	// calculate strength of dissusing light by the Sun
	float sun = max(0, dot(normal, sunLight) );

	float camera = max(0, dot(normal, normalize(particleGeometry.X.xyz - cameraPosition))*cameraLight );

	// set diffusing color
	outColor = vec4((ambient + sun + camera)* particleGeometry.Color.xyz, particleGeometry.Color.w);
}