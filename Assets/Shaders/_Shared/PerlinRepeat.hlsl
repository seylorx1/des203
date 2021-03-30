// https://www.shadertoy.com/view/4dlGW2

// Tileable noise, for creating useful textures. By David Hoskins, Sept. 2013.
// It can be extrapolated to other types of randomised texture.

//Converted to GLSL - Crabertay.

//----------------------------------------------------------------------------------------
// GLSL to HLSL HELPER FUNCTIONS
float fract(float foo) {
	return foo - floor(foo);
}

float2 fract(float2 foo) {
	return float2(fract(foo.x), fract(foo.y));
}

//----------------------------------------------------------------------------------------
float Hash(in float2 p, in float scale)
{
	// This is tiling part, adjusts with the scale...
	p = fmod(p, scale);
	return fract(sin(dot(p, float2(27.16898, 38.90563))) * 5151.5473453);
}

//----------------------------------------------------------------------------------------
float Noise(in float2 p, in float scale)
{
	float2 f;

	p *= scale;


	f = fract(p);		// Separate integer from fractional
	p = floor(p);

	f = f * f * (3.0 - 2.0 * f);	// Cosine interpolation approximation

	float res =
		lerp(
			lerp(
				Hash(p, scale),
				Hash(p + float2(1.0, 0.0), scale),
				f.x),
			lerp(
				Hash(p + float2(0.0, 1.0), scale),
				Hash(p + float2(1.0, 1.0), scale),
				f.x),
			f.y);

	return res;
}

//----------------------------------------------------------------------------------------
float fBm(in float2 p)
{
	float f = 0.0;
	// Change starting scale to any integer value...
	float scale = 10.;
	p = fmod(p, scale);
	float amp = 0.6;

	for (int i = 0; i < 5; i++)
	{
		f += Noise(p, scale) * amp;
		amp *= 0.5;
		// Scale must be multiplied by an integer value...
		scale *= 2.0;
	}
	// Clamp it just in case....
	return min(f, 1.0);
}

#ifdef VORONOI
float voronoiHash(in float2 p) {
	return
		fract(
			sin(
				float2(
					dot(p, float2(127.1, 311.7)),
					dot(p,float2(269.5, 183.3))
				)
			) * 43758.5453
		);
}

float voronoiDistance(in float2 x) {
	int2 p = int2(floor(x).xx);
	float2 f = fract(x);

	int2 mb;
	float2 mr;

	float res = 8.0;
	for (int vj = -1; vj <= 1; vj++) {
		for (int vi = -1; vi <= 1; vi++) {
			int2 b = int2(vi, vj);

			float2 r = float2(b) + voronoiHash(float2(p + b)) - f;
			float d = dot(r, r);

			if (d < res) {
				res = d;
				mr = r;
				mb = b;
			}
		}
	}

	res = 8.0;
	for (int vj = -2; vj <= 2; vj++) {
		for (int vi = -2; vi <= 2; vi++) {
			int2 b = mb + int2(vi, vj);

			float2 r = float2(b)+ voronoiHash(float2(p + b)) - f;
			float d = dot(0.5 * (mr + r), normalize(r - mr));

			res = min(res, d);
		}
	}

	return res;
}

float voronoiBorder(in float2 p) {
	float d = voronoiDistance(p);
	return 1.0 - smoothstep(0.0, 0.05, d);
}
#endif