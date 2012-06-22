//! Accelerate particle by sin/cos and move
/*!
	\param count number of particles
	\param particlesX array of postion vector
	\param particlesU array of velocity vector
	\param particlesA array of acceleration vector
	\param particlesD array of diameter
	\param A amplitude
	\param omega angular velocity
	\param t current time
	\param dt time increment
*/
__kernel void SinAcceleration(
	long count,
	__global float4* particlesX,
	__global float4* particlesU,
	__global float4* particlesA,
	const __global float* particlesD,
	float A,
	float omega,
	float t,
	float dt)
{	
	const int gi = get_global_id(0);

	if(gi > count)
	{
		return;
	}

	const float4 a = (float4)(A * cos(omega * t), 0, A * cos(1/omega * t), 0);
	
	const float4 u0 = particlesU[gi];

	particlesX[gi] += u0*dt + a * dt*dt / 2;
	particlesU[gi] += a * dt;
	particlesA[gi] = a;
}