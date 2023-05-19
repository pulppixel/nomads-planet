// Draws an explicit function of x. I was focusing on getting decent looking output
// with constant line size and "disconnected discontinuities".
// Remarks:
// The naive sampling approach has its problems.
// The drawing code and the function itself are coupled. At least sampling could be separated.
// A good sampling step size depends on local function gradient and graph range.
// Performance is proportional to line width, aa falloff and sample step; easy to
// make it too heavy.
// Connected line segments would be more performant, but would mask the discontinuity.
// Aliasing on this looks funky. :)

// The values below control the rendering, knock yourself out.
#define AA_FALLOFF 1.0			// AA falloff in pixels, must be > 0, affects all drawing
#define GRID_WIDTH 0.1			// grid line width in pixels, must be >= 0
#define CURVE_WIDTH	8.0			// curve line width in pixels, must be >= 0

#define FUNC_SAMPLE_STEP 0.2	// function sample step size in pixels

float pp; 			// pixel pitch in graph units

// The function to be drawn
float func(float x)
{
    //return invXEase(x, x + 0.6 + sin(x * x * 3.0 * 6.2832 + iTime * 5.0) * 0.6); // https://www.shadertoy.com/view/Xd2yRd
    //return 0.5 + sin(x * x * 3.0 + iTime * 2.0) * 0.5;
    float p = (x + iTime * 0.1) * 6.2832;
    float f = 1.0 * sin(p);
    f += 0.5 * sin(p * 3.0);
    f += 0.25 * sin(p * 5.0 + iTime * 2.5);
    f *= 0.5;
    return 0.5 + step(-0.5, x) * step(x, 1.5) * f;
}

// AA falloff function, trying lerp instead of smoothstep.
float aaStep(float a, float b, float x)
{
    // lerp step, make sure that a != b
    x = clamp(x, a, b);
    return (x - a) / (b - a);
}

// Alphablends color
void blend(inout vec4 baseCol, vec4 color, float alpha)
{
    baseCol = vec4(mix(baseCol.rgb, color.rgb, alpha * color.a), 1.0);
}

// Draws a gridline every stepSize
void drawGrid(inout vec4 baseCol, vec2 xy, float stepSize, vec4 gridCol)
{
	float hlw = GRID_WIDTH * pp * 0.5;
    float mul = 1.0 / stepSize;
	vec2 gf = abs(vec2(-0.5) + fract((xy + vec2(stepSize) * 0.5) * mul));
	float g = 1.0 - aaStep(hlw * mul, (hlw + pp * AA_FALLOFF) * mul, min(gf.x, gf.y));
    blend(baseCol, gridCol, g);
}

// Draws a circle
void drawCircle(inout vec4 baseCol, vec2 xy, vec2 center, float radius, vec4 color)
{
    float r = length(xy - center);
    float c = 1.0 - aaStep(radius, radius + pp * AA_FALLOFF, r);
    blend(baseCol, color, c);
}

// Draws explicit function of x defined in func(x)
void drawFunc(inout vec4 baseCol, vec2 xy, vec4 curveCol)
{
    // samples the function around x neighborhood to get distance to curve
    float hlw = CURVE_WIDTH * pp * 0.5;
    
    // cover line width and aa
    float left = xy.x - hlw - pp * AA_FALLOFF;
    float right = xy.x + hlw + pp * AA_FALLOFF;
    float closest = 100000.0;
    for (float x = left; x <= right; x+= pp * FUNC_SAMPLE_STEP)
    {
        vec2 diff = vec2(x, func(x)) - xy;
        float dSqr = dot(diff, diff);
        closest = min(dSqr, closest);
    }
    
	float c = 1.0 - aaStep(hlw, hlw + pp * AA_FALLOFF, sqrt(closest));
	blend(baseCol, curveCol, c);    
}

mat2 rotate2d(float angle)
{
    float sina = sin(angle);
    float cosa = cos(angle);
    return mat2(cosa, -sina,
                sina, cosa);
}

// Finds the next smaller power of 10
float findMagnitude(float range)
{
    float l10 = log(range) / log(10.0);
    return pow(10.0, floor(l10));
}

void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
    vec2 uv = fragCoord.xy / iResolution.xy;
    
    // graph setup
	float aspect = iResolution.x / iResolution.y;
    float z = 0.0;
	// comment out disable zoom:
    z = sin(iTime * 0.3) * 1.1;
    
    float graphRange = 0.4 + pow(1.5, z * z * z);
	vec2 graphSize = vec2(aspect * graphRange, graphRange);
	vec2 graphPos = 0.5 - graphSize * 0.5;	// center at (0.5, 0.5)

    vec2 xy = graphPos + uv * graphSize;	// xy = current graph coords
    pp = graphSize.y / iResolution.y;		// pp = pixel pitch in graph units
    
    // comment out to disable rotation:
   	xy = rotate2d(sin(iTime * 0.2) * 0.2) * (xy - 0.5) + 0.5;

    // background
    float t = length(0.5 - uv) * 1.414;
    t = t * t * t;
	vec4 col = mix(vec4(0.1, 0.25, 0.35, 1.0), vec4(0.0, 0.0, 0.0, 1.0), t);
    
	// grid
    float range = graphSize.y * 2.0;
    //float mag = findMagnitude(range);
    drawGrid(col, xy, 0.1, vec4(1.0, 1.0, 1.0, 0.1));
	drawGrid(col, xy, 0.5, vec4(1.0, 1.0, 1.0, 0.1));
	drawGrid(col, xy, 1.0, vec4(1.0, 1.0, 1.0, 0.4));

    // curve
    vec4 cCol = vec4(0.0, 1.0, 0.7, 0.8);
    drawFunc(col, xy, cCol);
    
	fragColor = col;
}