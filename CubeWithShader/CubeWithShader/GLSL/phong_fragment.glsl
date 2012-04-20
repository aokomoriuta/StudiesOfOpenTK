//! input: diffusing color
varying vec4 diffuseColor;

//! entry point
void main()
{
	// set color with diffusing color
	gl_FragColor = diffuseColor;
}