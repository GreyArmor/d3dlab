//--------------------------------------------------------------------------------------
// File: Default Effect for HelixToolkitDX
// Author: Przemyslaw Musialski
// Date: 11/11/12
// References & Sources: 
// light equations from OpenGL Spec: http://www.opengl.org/documentation/specs/version1.2/OpenGL_spec_1.2.1.pdf
// spotlight equation from DX9: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
// parts of the code based on: http://takinginitiative.net/2010/08/30/directx-10-tutorial-8-lighting-theory-and-hlsl/
//--------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------
// pre-processor includes
//--------------------------------------------------------------------------------------
#include "./Shaders/Common.fx"
#include "./Shaders/Material.fx"
#include "./Shaders/Lines.fx"

   

bool bDisableBack;
bool bDisableBlandDif;

//illumination settings
float4 illumDiffuse;
float4 illumSpecular;
float illumShine;

//--------------------------------------------------------------------------------------
// VERTEX AND PIXEL SHADER INPUTS
//-------------------------------------------------------------------------------------- 
struct VSInput
{
	float4 p			: POSITION;
	float4 c			: COLOR;
	float2 t			: TEXCOORD;
	float3 n			: NORMAL;
	//float3 t1			: TANGENT;
	//float3 t2			: BINORMAL;

	float4 mr0			: TEXCOORD1;
	float4 mr1			: TEXCOORD2;
	float4 mr2			: TEXCOORD3;
	float4 mr3			: TEXCOORD4; 
};     

//--------------------------------------------------------------------------------------
struct PSInput
{
	float4 p			: SV_POSITION;
	float4 wp			: POSITION0;
	float4 sp			: TEXCOORD1;
	float3 n			: NORMAL;	    // normal
	float2 t			: TEXCOORD0;	// tex coord	
	//float3 t1			: TANGENT;		// tangent
	//float3 t2			: BINORMAL;		// bi-tangent	
	float4 c			: COLOR;		// solid color (for debug)
};

//--------------------------------------------------------------------------------------
void GSAddPoint(float4 p, PSInput input, inout TriangleStream<PSInput> outStream)
{
	PSInput output;
	output.p = p;
	output.c = input.c;
	output.wp = input.wp;
	output.sp = input.sp;
	output.n = input.n;
	output.t = input.t;
	//output.t1 = input.t1;
	//output.t2 = input.t2;
	outStream.Append(output);
}

[maxvertexcount(18)]
void GShaderWires(triangle PSInput input[3], inout TriangleStream<PSInput> outStream)
{
	PSInput output;

	for (int i0 = 0; i0 < 3; i0++)
	{
		int i1 = i0 + 1;
		if (i1 == 3)
			i1 = 0;

		float thikness = vLineParams.x;

		float4 lineCorners[4];
		makeLine(lineCorners, input[i0].p, input[i1].p, thikness);

		GSAddPoint(lineCorners[0], input[i0], outStream);
		GSAddPoint(lineCorners[1], input[i0], outStream);
		GSAddPoint(lineCorners[2], input[i1], outStream);
		GSAddPoint(lineCorners[0], input[i0], outStream);
		GSAddPoint(lineCorners[2], input[i1], outStream);
		GSAddPoint(lineCorners[3], input[i1], outStream);
	}
	outStream.RestartStrip();
}

//-------------------------------------------------------------------------------------------
// Vertex
//-------------------------------------------------------------------------------------------
void GSAddPointV(float4 p, PSInput input, inout TriangleStream<PSInput> outStream)
{
	PSInput output;
	output.p = p;
	output.c = input.c;
	output.wp = input.wp;
	output.sp = input.sp;
	output.n = input.n;
	output.t = input.t;
	//output.t1 = input.t1;
	//output.t2 = input.t2;
	outStream.Append(output);
}

[maxvertexcount(12)]
void GShaderVertex_P1(triangle PSInput _input[3], inout TriangleStream<PSInput> outStream)
{
	float w = vLineParams.x * 0.5;
	PSInput output;

	for (int i = 0; i < 3; i++)
	{
		PSInput input = _input[i];
		float2 p0 = projToWindow(input.p);

			float2 p1 = p0 + float2(-w, -w);
			float2 p2 = p0 + float2(-w, w);
			float2 p3 = p0 + float2(w, -w);
			float2 p4 = p0 + float2(w, w);
			GSAddPointV(windowToProj(p1, input.p.z, input.p.w), input, outStream);
		GSAddPointV(windowToProj(p2, input.p.z, input.p.w), input, outStream);
		GSAddPointV(windowToProj(p3, input.p.z, input.p.w), input, outStream);
		GSAddPointV(windowToProj(p4, input.p.z, input.p.w), input, outStream);
		outStream.RestartStrip();
	}
}



//[maxvertexcount(24)]
//void GShaderVertex_P2(point PSInput _input[1], inout TriangleStream<PSInput> outStream)
//{
//	PSInput input = _input[0];
//	PSInput output;

//	float r = 3;

//	float pi = 3.14159f;
//	float step = pi / 4;
//	float start = pi / 8;

//	float2 p0 = projToWindow(input.p);

//	GSAddPointV(input.p, input, outStream);

//	for (float i = 0; i <= 8; i += 1.0)
//	{
//		float a = start + step * i;

//		float2 pr = float2(p0.x + sin(a) * r, p0.y + cos(a) * r);
//		float4 p = windowToProj(pr, input.p.z, input.p.w);

//		GSAddPointV(p, input, outStream);
//	}
//	outStream.RestartStrip();
//}

/*
struct GSInputVertex
{
float4 p	: SV_POSITION;
float4 wp	: POSITION0;
float4 c	: COLOR;
float3 n	: NORMAL;
};

struct PSInputVertex
{
float4 p	: SV_POSITION;
float4 wp	: POSITION0;
float4 c	: COLOR;
float2 t	: TEXCOORD0;
float3 n	: NORMAL;
};

struct PSOutputVertex
{
float4 n	: SV_Target0;
float4 dif	: SV_Target1;
float4 spec	: SV_Target2;
float4 p	: SV_Target3;
};

GSInputVertex VShaderVertex(VSInput input)
{
GSInputVertex output = (GSInputVertex)0;

output.p = mul(input.p, mWorld);
output.wp = output.p;
output.p = mul(output.p, mView);
output.p = mul(output.p, mProjection);

output.c = input.c;
output.n = input.n;

return output;
}

//void makePoint(out float4 points[4], in float4 pos, in float width)
//{
//	width /= 2.0;

//	float2 p0 = projToWindow(pos);

//	float2 p1 = p0 + float2(-3, -3);
//	float2 p2 = p0 + float2(-3, 3);
//	float2 p3 = p0 + float2(3, -3);
//	float2 p4 = p0 + float2(3, 3);

//	// bring back corners in projection frame
//	points[0] = windowToProj(p1, pos.z, pos.w);
//	points[1] = windowToProj(p2, pos.z, pos.w);
//	points[2] = windowToProj(p3, pos.z, pos.w);
//	points[3] = windowToProj(p4, pos.z, pos.w);
//}

void GSAddPointV(float dx, float dy, GSInputVertex input, inout TriangleStream<PSInputVertex> outStream)
{
float2 p0 = projToWindow(input.p);
p0 += float2(dx, dy);

PSInputVertex output;
output.p = windowToProj(p0, input.p.z, input.p.w);;
output.c = input.c;
output.wp = input.wp;
//output.sp = input.sp;
output.n = input.n;
output.t = float2(dx / abs(dx), dy / abs(dy));
//output.t1 = input.t1;
//output.t2 = input.t2;
outStream.Append(output);
}

[maxvertexcount(9)]
void GShaderVertex3(triangle GSInputVertex input[3], inout TriangleStream<PSInputVertex> outStream)
{
PSInput output;
for (int i = 0; i < 3; i++)
{
//float4 points[4];
//makePoint(points, input[i].p, vLineParams.x);

float w = 6;

GSAddPointV(-w, -w, input[i], outStream);
GSAddPointV(-w, w, input[i], outStream);
GSAddPointV(w, -w, input[i], outStream);
GSAddPointV(w, w, input[i], outStream);
outStream.RestartStrip();
}
}

float4 PShaderVertex(PSInputVertex input) : SV_Target
{
float l = length(float2(input.t.x, input.t.y));
float k1 = 0.9;
float k2 = 1.1;
float a = l < k1 ? 1 :
l > k2 ? 0 :
(1 - (l - k1) * (1 / (k2 - k1)));

if (a > 0)
return float4(1 - l * 0.5, 0, 0, a);// normalize(float4(input.t.x, input.t.y, 1 - l, 1));

return float4(0, 0, 0, 0);
}
*/

//--------------------------------------------------------------------------------------
// Phong Lighting Reflection Model
//--------------------------------------------------------------------------------------
// Returns the sum of the diffuse and specular terms in the Phong reflection model
// The specular and diffuse reflection constants for the currently loaded material (k_d and k_s) as well
// as other material properties are defined in Material.fx.
//float4 calcPhongLighting(float4 LColor, float4 diffuseColor, float3 N, float3 L, float3 V, float3 R)
//{
//	float4 Id = diffuseColor * illumDiffuse * saturate(dot(N, L));
//		float4 Is = vMaterialSpecular * illumSpecular * pow(saturate(dot(R, V)), sMaterialShininess * illumShine);
//		return (Id + Is) * LColor;
//}

float4 calcPhongLightingWithColor(float4 LColor, float4 diffuseColor, float3 N, float3 L, float3 V, float3 R, float4 vertexColor)
{
	float4 c = 1.0f;
		if (diffuseColor.a > 0)
			c *= diffuseColor;
	if (vertexColor.a > 0)
	{
		if (bDisableBlandDif)
			c = vertexColor;
		else
			c *= vertexColor;
	}
	float4 Id = c * illumDiffuse * saturate(dot(N, L));
		float4 Is = vMaterialSpecular * illumSpecular * pow(saturate(dot(R, V)), sMaterialShininess * illumShine);
		return (Id + Is) * LColor;
}

//--------------------------------------------------------------------------------------
// Blinn-Phong Lighting Reflection Model
//--------------------------------------------------------------------------------------
// Returns the sum of the diffuse and specular terms in the Blinn-Phong reflection model.
float4 calcBlinnPhongLighting(float4 LColor, float4 diffuseColor, float3 N, float3 L, float3 H)
{
	float4 Id = diffuseColor * illumDiffuse * saturate(dot(N, L));
		float4 Is = vMaterialSpecular * illumSpecular * pow(saturate(dot(N, H)), sMaterialShininess * illumShine);
		return (Id + Is) * LColor;
}

//--------------------------------------------------------------------------------------
// normal mapping
//--------------------------------------------------------------------------------------
// This function returns the normal in world coordinates.
// The input struct contains tangent (t1), bitangent (t2) and normal (n) of the
// unperturbed surface in world coordinates. The perturbed normal in tangent space
// can be read from texNormalMap.
// The RGB values in this texture need to be normalized from (0, +1) to (-1, +1).
float3 calcNormal(PSInput input)
{
	if (bHasNormalMap)
	{
		// Normalize the per-pixel interpolated tangent-space
		input.n = normalize(input.n);
		//input.t1 = normalize(input.t1);
		//input.t2 = normalize(input.t2);

		// Sample the texel in the bump map.
		float4 bumpMap = texNormalMap.Sample(NormalSampler, input.t);
			// Expand the range of the normal value from (0, +1) to (-1, +1).
			bumpMap = (bumpMap * 2.0f) - 1.0f;
		// Calculate the normal from the data in the bump map.
		input.n = input.n /*+ bumpMap.x * input.t1 + bumpMap.y * input.t2*/;
	}
	return normalize(input.n);
}

//--------------------------------------------------------------------------------------
// reflectance mapping
//--------------------------------------------------------------------------------------
float4 cubeMapReflection(PSInput input, float4 I)
{
	float3 v = normalize((float3)input.wp - vEyePos);
		float3 r = reflect(v, input.n);
		return (1.0f - vMaterialReflect)*I + vMaterialReflect*texCubeMap.Sample(LinearSampler, r);
}

//--------------------------------------------------------------------------------------
// get shadow color
//--------------------------------------------------------------------------------------
float2 texOffset(int u, int v)
{
	return float2(u * 1.0f / vShadowMapSize.x, v * 1.0f / vShadowMapSize.y);
}

//--------------------------------------------------------------------------------------
// get shadow color
//--------------------------------------------------------------------------------------
float shadowStrength(float4 sp)
{
	sp = sp / sp.w;
	if (sp.x < -1.0f || sp.x > 1.0f || sp.y < -1.0f || sp.y > 1.0f || sp.z < 0.0f || sp.z > 1.0f)
	{
		return 1;
	}
	sp.x = sp.x / +2.0 + 0.5;
	sp.y = sp.y / -2.0 + 0.5;

	//apply shadow map bias
	sp.z -= vShadowMapInfo.z;

	//// --- not in shadow, hard cut
	//float shadowMapDepth = texShadowMap.Sample(PointSampler, sp.xy).r;
	//if ( shadowMapDepth < sp.z) 
	//{
	//	return 0;
	//}

	//// --- basic hardware PCF - single texel
	//float shadowFactor = texShadowMap.SampleCmpLevelZero( CmpSampler, sp.xy, sp.z ).r;

	//// --- PCF sampling for shadow map
	float sum = 0;
	float x = 0, y = 0;
	float range = vShadowMapInfo.y;
	float div = 0.0000001;

	// ---perform PCF filtering on a 4 x 4 texel neighborhood
	for (y = -range; y <= range; y += 1.0)
	{
		for (x = -range; x <= range; x += 1.0)
		{
			sum += texShadowMap.SampleCmpLevelZero(CmpSampler, sp.xy + texOffset(x, y), sp.z);
			div++;
		}
	}

	float shadowFactor = sum / (float)div;
	float fixTeil = vShadowMapInfo.x;
	float nonTeil = 1 - vShadowMapInfo.x;
	// now, put the shadow-strengh into the 0-nonTeil range
	nonTeil = shadowFactor*nonTeil;
	return (fixTeil + nonTeil);
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - Vertex Shader
//--------------------------------------------------------------------------------------

PSInput VShaderSimple2(VSInput input)
{
	PSInput result = (PSInput)0;
	result.p = input.p;
	//result.wp = mul(input.p, mWorld);
	result.wp = input.p;
	result.c = input.c;
	result.t = input.t;
	return result;
}

float4 PShaderSimple2(PSInput input) : SV_Target
{
	return texDiffuseMap.Sample(LinearSampler, input.t);
}
PSInput VShaderDefault(VSInput input)
{
	PSInput output = (PSInput)0;
	float4 inputp = input.p;

		// compose instance matrix
	if (bHasInstances)
	{
		matrix mInstance =
		{
			input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
			input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
			input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
			input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
		};
		inputp = mul(mInstance, input.p);
	}

		//set position into camera clip space	
		output.p = mul(inputp, mWorld);
	output.wp = output.p;
	output.p = mul(output.p, mView);
	output.p = mul(output.p, mProjection);

	//set position into light-clip space
	if (bHasShadowMap)
	{
		//for (int i = 0; i < 1; i++)
		{
			output.sp = mul(inputp, mWorld);
			output.sp = mul(output.sp, mLightView[0]);
			output.sp = mul(output.sp, mLightProj[0]);
		}
	}

	//set texture coords and color
	output.t = input.t;
	output.c = input.c;

	//set normal for interpolation	
	output.n = normalize(mul(input.n, (float3x3)mWorld));

	/*
	if (bHasNormalMap)
	{
	// transform the tangents by the world matrix and normalize
	output.t1 = normalize(mul(input.t1, (float3x3)mWorld));
	output.t2 = normalize(mul(input.t2, (float3x3)mWorld));
	}
	else
	{
	output.t1 = 0.0f;
	output.t2 = 0.0f;
	}
	*/

	return output;
}

int CalcPlanes(PSInput input)
{
	int result = -1;
	float4 normal;
	float d;
	float res;
	for (int i = 0; i < nPlaneCount; i++)
	{
		normal = vPlaneNormals[i];
		d = vPlaneDs[i];
		res = normal.x * input.wp.x + normal.y * input.wp.y + normal.z * input.wp.z + d;
		if (res < 0)
		{
			discard;
		}
		if (res < fPlaneLineWidth)
		{
			result = i;
		}
	}
	return result;
}


//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - PHONG
//--------------------------------------------------------------------------------------
float4 PShaderPhongCore(PSInput input, int discardKind)
{
	float4 I;
	//calculate lighting vectors - renormalize vectors	
	input.n = calcNormal(input);

	// get per pixel vector to eye-position
	//float3 eye = normalize(vEyeLook);
	float3 eye = normalize(vEyePos - input.wp.xyz);

		float angle = dot(input.n, eye);
	if ((discardKind == DISCARD_FRONT && angle >= 0) || (discardKind == DISCARD_BACK && angle < 0))
		discard;

	if (!bDisableBack && angle < 0)
		input.n = input.n * -1;

	// light emissive and ambient intensity
	// this variable can be used for light accumulation
	I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;

	// get shadow color
	float s = 1;
	if (bHasShadowMap)
	{
		s = shadowStrength(input.sp);
	}

	// add diffuse sampling
	float4 vDiffuseColor = vMaterialDiffuse;
		if (bHasDiffuseMap)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
				vDiffuseColor = texDiffuseMap.Sample(LinearSampler, input.t);
			else
				vDiffuseColor *= texDiffuseMap.Sample(LinearSampler, input.t);
		}
	if (!bDisableBack && angle < 0)
	{
		vDiffuseColor = vMaterialDiffuseBack;
		if (bHasDiffuseMapBack)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
				vDiffuseColor = texDiffuseMapBack.Sample(LinearSampler, input.t);
			else
				vDiffuseColor *= texDiffuseMapBack.Sample(LinearSampler, input.t);
		}
	}

	// loop over lights
	for (int i = 0; i < iLightCount; i++)
	{
		// This framework calculates lighting in world space.
		// For every light type, you should calculate the input values to the
		// calcPhongLighting function, namely light direction and the reflection vector.
		// For computuation of attenuation and the spot light factor, use the
		// model from the DirectX documentation:
		// http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx

		if (iLightType[i] == 1) // directional
		{
			float3 d = normalize(vLightDir[i].xyz);
				float3 r = reflect(-d, input.n);
				I += s * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else if (iLightType[i] == 2)  // point
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir	
				float dl = length(d);
			d = normalize(d);
			float3 r = reflect(-d, input.n);
				float att = 1.0f / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else if (iLightType[i] == 3)  // spot
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir
				float dl = length(d);
			d = normalize(d);
			float3 r = reflect(-d, input.n);
				float3 sd = normalize((float3)vLightDir[i]);	// missuse the vLightDir variable for spot-dir

				/* --- this is the  DirectX9 version (better) --- */
				float rho = dot(-d, sd);
			float spot = pow(saturate((rho - vLightSpot[i].x) / (vLightSpot[i].y - vLightSpot[i].x)), vLightSpot[i].z);
			float att = spot / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else
		{
			//I += 0;
		}
	}
	//I = I + vLightAmbient;
	/// set diffuse alpha
	I.a = vDiffuseColor.a;

	int pRes = CalcPlanes(input);
	if (pRes >= 0)
	{
		I = vPlaneColors[pRes];
	}

	return I;
}

float4 PShaderPhongF(PSInput input) : SV_Target
{
	return PShaderPhongCore(input, DISCARD_BACK);
}

float4 PShaderPhongB(PSInput input) : SV_Target
{
	return PShaderPhongCore(input, DISCARD_FRONT);
}

float4 PShaderPhong(PSInput input) : SV_Target
{
	return PShaderPhongCore(input, DISCARD_NONE);
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - PHONG with ambient
//--------------------------------------------------------------------------------------
float4 PShaderPhongWithAmbient(PSInput input, int discardKind)
{
	float4 I;
	//calculate lighting vectors - renormalize vectors	
	input.n = calcNormal(input);

	// get per pixel vector to eye-position
	//float3 eye = normalize(vEyeLook);
	float3 eye = normalize(vEyePos - input.wp.xyz);

		float angle = dot(input.n, eye);
	if ((discardKind == DISCARD_FRONT && angle >= 0) || (discardKind == DISCARD_BACK && angle < 0))
		discard;

	if (!bDisableBack && angle < 0)
		input.n = input.n * -1;

	// light emissive and ambient intensity
	// this variable can be used for light accumulation
	I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;

	// get shadow color
	float s = 1;
	if (bHasShadowMap)
	{
		s = shadowStrength(input.sp);
	}

	// add diffuse sampling
	float4 vDiffuseColor = vMaterialDiffuse;
		if (bHasDiffuseMap)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
				vDiffuseColor = texDiffuseMap.Sample(LinearSampler, input.t);
			else
				vDiffuseColor *= texDiffuseMap.Sample(LinearSampler, input.t);
		}
	if (!bDisableBack && angle < 0)
	{
		vDiffuseColor = vMaterialDiffuseBack;
		if (bHasDiffuseMapBack)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
				vDiffuseColor = texDiffuseMapBack.Sample(LinearSampler, input.t);
			else
				vDiffuseColor *= texDiffuseMapBack.Sample(LinearSampler, input.t);
		}
	}

	// loop over lights
	for (int i = 0; i < iLightCount; i++)
	{
		// This framework calculates lighting in world space.
		// For every light type, you should calculate the input values to the
		// calcPhongLighting function, namely light direction and the reflection vector.
		// For computuation of attenuation and the spot light factor, use the
		// model from the DirectX documentation:
		// http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx

		if (iLightType[i] == 1) // directional
		{
			float3 d = normalize((float3)vLightDir[i]);
				float3 r = reflect(-d, input.n);
				I += s * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else if (iLightType[i] == 2)  // point
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir	
				float dl = length(d);
			d = normalize(d);
			float3 r = reflect(-d, input.n);
				float att = 1.0f / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else if (iLightType[i] == 3)  // spot
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir
				float dl = length(d);
			d = normalize(d);
			float3 r = reflect(-d, input.n);
				float3 sd = normalize((float3)vLightDir[i]);	// missuse the vLightDir variable for spot-dir

				/* --- this is the  DirectX9 version (better) --- */
				float rho = dot(-d, sd);
			float spot = pow(saturate((rho - vLightSpot[i].x) / (vLightSpot[i].y - vLightSpot[i].x)), vLightSpot[i].z);
			float att = spot / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else
		{
			//I += 0;
		}
	}
	I = I + vLightAmbient;
	/// set diffuse alpha
	I.a = vDiffuseColor.a;

	int pRes = CalcPlanes(input);
	if (pRes >= 0)
	{
		I = vPlaneColors[pRes];
	}

	return I;
}

float4 PShaderPhongWithAmbientF(PSInput input) : SV_Target
{
	return PShaderPhongWithAmbient(input, DISCARD_BACK);
}

float4 PShaderPhongWithAmbientB(PSInput input) : SV_Target
{
	return PShaderPhongWithAmbient(input, DISCARD_FRONT);
}

float4 PShaderPhongWithAmbient(PSInput input) : SV_Target
{
	return PShaderPhongWithAmbient(input, DISCARD_NONE);
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - PHONG with sphere coloring
//--------------------------------------------------------------------------------------


int PointInsideSphere(PSInput input,out float distanceToCentre)
{
	//float vSphereColoringRadius;
	//float vSphereColoringOffset;

	//float4 vSphereColoringPosition;
	float distanceToCentreOfBrush = distance(input.wp, vSphereColoringPosition);
	distanceToCentre = distanceToCentreOfBrush;
	if ((distanceToCentreOfBrush<vSphereColoringRadius))
	{
		if ((vSphereColoringRadius - vSphereColoringOffset)<distanceToCentreOfBrush)
		{
			return 0;
		}
		else
		{
			return 1;
		}
	}
	return -1;;
}
float4 PShaderPhongWithSphereColoring(PSInput input, int discardKind)
{
	float4 I;
	//calculate lighting vectors - renormalize vectors	
	input.n = calcNormal(input);

	// get per pixel vector to eye-position
	//float3 eye = normalize(vEyeLook);
	float3 eye = normalize(vEyePos - input.wp.xyz);

		float angle = dot(input.n, eye);
	if ((discardKind == DISCARD_FRONT && angle >= 0) || (discardKind == DISCARD_BACK && angle < 0))
		discard;

	if (!bDisableBack && angle < 0)
		input.n = input.n * -1;

	// light emissive and ambient intensity
	// this variable can be used for light accumulation
	I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;

	// get shadow color
	float s = 1;
	if (bHasShadowMap)
	{
		s = shadowStrength(input.sp);
	}

	// add diffuse sampling
	float4 vDiffuseColor = vMaterialDiffuse;
		if (bHasDiffuseMap)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
				vDiffuseColor = texDiffuseMap.Sample(LinearSampler, input.t);
			else
				vDiffuseColor *= texDiffuseMap.Sample(LinearSampler, input.t);
		}
	if (!bDisableBack && angle < 0)
	{
		vDiffuseColor = vMaterialDiffuseBack;
		if (bHasDiffuseMapBack)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
				vDiffuseColor = texDiffuseMapBack.Sample(LinearSampler, input.t);
			else
				vDiffuseColor *= texDiffuseMapBack.Sample(LinearSampler, input.t);
		}
	}
	if (vSphereEnabled)
	{
		//float vSphereColoringRadius;
		//float vSphereColoringOffset;

		//float4 vSphereColoringPosition;
		//0 - inside offset, 1 - inside sphere no offset, -1 - outside sphere
		float distanceToCentre;
		int yesNoInsideOffset = PointInsideSphere(input, distanceToCentre);
		if (yesNoInsideOffset == 1)
		{
			//float4 vSphereColoringColor;
			vDiffuseColor = vSphereColoringColor;
		}
		if (yesNoInsideOffset == 0)
		{
			float howMuchOffset = vSphereColoringRadius - distanceToCentre;
			float percentOffset = howMuchOffset / vSphereColoringOffset;
			vDiffuseColor = lerp(vDiffuseColor, vSphereColoringColor, percentOffset);
		}
	}
	// loop over lights
	for (int i = 0; i < iLightCount; i++)
	{
		// This framework calculates lighting in world space.
		// For every light type, you should calculate the input values to the
		// calcPhongLighting function, namely light direction and the reflection vector.
		// For computuation of attenuation and the spot light factor, use the
		// model from the DirectX documentation:
		// http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx

		if (iLightType[i] == 1) // directional
		{
			float3 d = normalize((float3)vLightDir[i]);
				float3 r = reflect(-d, input.n);
				I += s * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else if (iLightType[i] == 2)  // point
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir	
				float dl = length(d);
			d = normalize(d);
			float3 r = reflect(-d, input.n);
				float att = 1.0f / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else if (iLightType[i] == 3)  // spot
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir
				float dl = length(d);
			d = normalize(d);
			float3 r = reflect(-d, input.n);
				float3 sd = normalize((float3)vLightDir[i]);	// missuse the vLightDir variable for spot-dir

				/* --- this is the  DirectX9 version (better) --- */
				float rho = dot(-d, sd);
			float spot = pow(saturate((rho - vLightSpot[i].x) / (vLightSpot[i].y - vLightSpot[i].x)), vLightSpot[i].z);
			float att = spot / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else
		{
			//I += 0;
		}
	}
	/// set diffuse alpha
	I.a = vDiffuseColor.a;

	int pRes = CalcPlanes(input);
	if (pRes >= 0)
	{
		I = vPlaneColors[pRes];
	}

	return I;
}

float4 PShaderPhongWithSphereColoringF(PSInput input) : SV_Target
{
	return PShaderPhongWithSphereColoring(input, DISCARD_BACK);
}

float4 PShaderPhongWithSphereColoringB(PSInput input) : SV_Target
{
	return PShaderPhongWithSphereColoring(input, DISCARD_FRONT);
}

float4 PShaderPhongWithSphereColoring(PSInput input) : SV_Target
{
	return PShaderPhongWithSphereColoring(input, DISCARD_NONE);
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - PHONG PointSampler
//--------------------------------------------------------------------------------------

float4 PShaderPhongCorePointSampler(PSInput input, int discardKind)
{
	float4 I;// = texDiffuseMap.Sample(PointSampler, input.t);
	//calculate lighting vectors - renormalize vectors	
	input.n = calcNormal(input);

	// get per pixel vector to eye-position
	//float3 eye = normalize(vEyeLook);
	float3 eye = normalize(vEyePos - input.wp.xyz);

		float angle = dot(input.n, eye);
	if ((discardKind == DISCARD_FRONT && angle >= 0) || (discardKind == DISCARD_BACK && angle < 0))
		discard;

	bool drawBack = !bDisableBack && angle < 0;
	if (drawBack)
		input.n = input.n * -1;

	// light emissive and ambient intensity
	// this variable can be used for light accumulation
	I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;

	// get shadow color
	float s = 1;
	if (bHasShadowMap)
	{
		s = shadowStrength(input.sp);
	}

	// add diffuse sampling
	float4 vDiffuseColor = vMaterialDiffuse;

	if (bHasDiffuseMap && (!drawBack || !bHasDiffuseMapBack))
		{
		drawBack = false;
			// SamplerState is defined in Common.fx.
		float4 tColor = texDiffuseMap.Sample(PointSampler, input.t);
			if (bDisableBlandDif)
			vDiffuseColor = tColor;
			else
			vDiffuseColor *= tColor;
		}
	if (drawBack)
	{
		vDiffuseColor = vMaterialDiffuseBack;
		if (bHasDiffuseMapBack)
		{
			float4 tColor = texDiffuseMapBack.Sample(PointSampler, input.t);
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
				vDiffuseColor = tColor;
			else
				vDiffuseColor *= tColor;
		}
	}

	I = vDiffuseColor;

	int pRes = CalcPlanes(input);
	if (pRes >= 0)
	{
		I = vPlaneColors[pRes];
	}

	return I;
}

float4 PShaderPhongFPointSampler(PSInput input) : SV_Target
{
	return PShaderPhongCorePointSampler(input, DISCARD_BACK);
}

float4 PShaderPhongBPointSampler(PSInput input) : SV_Target
{
	return PShaderPhongCorePointSampler(input, DISCARD_FRONT);
}

float4 PShaderPhongPointSampler(PSInput input) : SV_Target
{
	return PShaderPhongCorePointSampler(input, DISCARD_NONE);
}
// axis of coloring (when checking radius it will check in axis plane): 0 = None, 1 = X, 2 = Y, 3 = Z
bool CheckRadius(PSInput input,int i)
{
	if (iCilidricalColoringAxis[i] == 0)
	{
		return false;
	}
	float3 _2DWorldPoint = { 0, 0, 0 };
	float4 maximum = vBoxesMaximums[i];
	float4 minimum = vBoxesMinimums[i];
	float centerX = (minimum.x + maximum.x) / 2;
	float centerY = (minimum.y + maximum.y) / 2;
	float centerZ = (minimum.z + maximum.z) / 2;
	if (iCilidricalColoringAxis[i] == 1)
	{
		centerX = 0;
		_2DWorldPoint = float3(0, input.wp.y, input.wp.z);
	}
	if (iCilidricalColoringAxis[i] == 2)
	{
		centerY = 0;
		_2DWorldPoint = float3(input.wp.x, 0, input.wp.z);
	}
	if (iCilidricalColoringAxis[i] == 3)
	{
		centerZ = 0;
		_2DWorldPoint = float3(input.wp.x, input.wp.y, 0);
	}


	float radiusOfBlock = vBoxBlockRadius[i];
	float3 centre = { centerX, centerY, centerZ };

	return distance(_2DWorldPoint, centre) > radiusOfBlock;

}

int PointInsideBoxes(PSInput input)
{
	if (nBoxCount == 0)
	{
		return -1;
	}
	for (int i = 0; i < nBoxCount; i++)
	{
		float4 maximum = vBoxesMaximums[i];
		float4 minimum = vBoxesMinimums[i];
		if (!(input.wp.x > maximum.x || input.wp.y > maximum.y || input.wp.z > maximum.z || input.wp.x < minimum.x || input.wp.y < minimum.y || input.wp.z < minimum.z || CheckRadius(input,i)))
		{
			return -1;
		}
	}
	return 0;
}

float4 PShaderPhongCoreColorByBox(PSInput input, int discardKind)
{
	float4 I;

	//calculate lighting vectors - renormalize vectors	
	input.n = calcNormal(input);

	// get per pixel vector to eye-position
	//float3 eye = normalize(vEyeLook);
	float3 eye = normalize(vEyePos - input.wp.xyz);

		float angle = dot(input.n, eye);
	if ((discardKind == DISCARD_FRONT && angle >= 0) || (discardKind == DISCARD_BACK && angle < 0))
		discard;

	if (!bDisableBack && angle < 0)
		input.n = input.n * -1;

	// light emissive and ambient intensity
	// this variable can be used for light accumulation
	I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;

	// get shadow color
	float s = 1;
	if (bHasShadowMap)
	{
		s = shadowStrength(input.sp);
	}

	// add diffuse sampling
	float4 vDiffuseColor = vMaterialDiffuse;
		if (bHasDiffuseMap)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
			{
				vDiffuseColor = texDiffuseMap.Sample(LinearSampler, input.t);
			}
			else
			{
				vDiffuseColor *= texDiffuseMap.Sample(LinearSampler, input.t);
			}
		}

	if (!bDisableBack && angle < 0)
	{
		vDiffuseColor = vMaterialDiffuseBack;
		if (bHasDiffuseMapBack)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
			{
				vDiffuseColor = texDiffuseMapBack.Sample(LinearSampler, input.t);
			}
			else
			{
				vDiffuseColor *= texDiffuseMapBack.Sample(LinearSampler, input.t);
			}
		}
	}

	int boxIndex = PointInsideBoxes(input);
	if (boxIndex > -1)
	{
		vDiffuseColor = vBoxesColors[boxIndex];
	}
	// loop over lights
	for (int i = 0; i < iLightCount; i++)
	{
		// This framework calculates lighting in world space.
		// For every light type, you should calculate the input values to the
		// calcPhongLighting function, namely light direction and the reflection vector.
		// For computuation of attenuation and the spot light factor, use the
		// model from the DirectX documentation:
		// http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx

		if (iLightType[i] == 1) // directional
		{
			float3 d = normalize((float3)vLightDir[i]);
				float3 r = reflect(-d, input.n);
				I += s * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else if (iLightType[i] == 2)  // point
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir	
				float dl = length(d);
			d = normalize(d);
			float3 r = reflect(-d, input.n);
				float att = 1.0f / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else if (iLightType[i] == 3)  // spot
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir
				float dl = length(d);
			d = normalize(d);
			float3 r = reflect(-d, input.n);
				float3 sd = normalize((float3)vLightDir[i]);	// missuse the vLightDir variable for spot-dir

				/* --- this is the  DirectX9 version (better) --- */
				float rho = dot(-d, sd);
			float spot = pow(saturate((rho - vLightSpot[i].x) / (vLightSpot[i].y - vLightSpot[i].x)), vLightSpot[i].z);
			float att = spot / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else
		{
			//I += 0;
		}
	}

	/// set diffuse alpha
	I.a = vDiffuseColor.a;

	int pRes = CalcPlanes(input);
	if (pRes >= 0)
	{
		I = vPlaneColors[pRes];
	}


	return I;
}

float4 PShaderPhongFColorByBox(PSInput input) : SV_Target
{
	return PShaderPhongCoreColorByBox(input, DISCARD_BACK);
}

float4 PShaderPhongBColorByBox(PSInput input) : SV_Target
{
	return PShaderPhongCoreColorByBox(input, DISCARD_FRONT);
}

float4 PShaderPhongColorByBox(PSInput input) : SV_Target
{
	return PShaderPhongCoreColorByBox(input, DISCARD_NONE);
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - PHONG COLOR FOR OUT OF BOX OBJECTS
// DETECT OBJECT THAT OUT OF SETTING BOX
//--------------------------------------------------------------------------------------

int PointOutOfBoxes(PSInput input)
{
	float4 maximum = vOutOfBoxesMaximums;
	float4 minimum = vOutOfBoxesMinimums;

	float centerX = (minimum.x + maximum.x) / 2;
	float centerY = (minimum.y + maximum.y) / 2;
	float centerZ = 0;
	float3 centre = { centerX, centerY, centerZ };


	float3 _2DWorldPoint = { input.wp.x, input.wp.y, 0 };
	if (distance(_2DWorldPoint, centre) > vOutOfBoxesRadius){
		return 1;
	}
	return 0;
}

float4 PShaderPhongCoreColorByOutOfBox(PSInput input, int discardKind)
{
	float4 I;

	//calculate lighting vectors - renormalize vectors	
	input.n = calcNormal(input);

	// get per pixel vector to eye-position
	//float3 eye = normalize(vEyeLook);
	float3 eye = normalize(vEyePos - input.wp.xyz);

		float angle = dot(input.n, eye);
	if ((discardKind == DISCARD_FRONT && angle >= 0) || (discardKind == DISCARD_BACK && angle < 0))
		discard;

	if (!bDisableBack && angle < 0)
		input.n = input.n * -1;

	// light emissive and ambient intensity
	// this variable can be used for light accumulation
	I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;

	// get shadow color
	float s = 1;
	if (bHasShadowMap)
	{
		s = shadowStrength(input.sp);
	}

	// add diffuse sampling
	float4 vDiffuseColor = vMaterialDiffuse;
		if (bHasDiffuseMap)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
			{
				vDiffuseColor = texDiffuseMap.Sample(LinearSampler, input.t);
			}
			else
			{
				vDiffuseColor *= texDiffuseMap.Sample(LinearSampler, input.t);
			}
		}

	if (!bDisableBack && angle < 0)
	{
		vDiffuseColor = vMaterialDiffuseBack;
		if (bHasDiffuseMapBack)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
			{
				vDiffuseColor = texDiffuseMapBack.Sample(LinearSampler, input.t);
			}
			else
			{
				vDiffuseColor *= texDiffuseMapBack.Sample(LinearSampler, input.t);
			}
		}
	}

	int boxIndex = PointOutOfBoxes(input);
	if (boxIndex > 0)
	{
		vDiffuseColor = vOutOfBoxesColors;
	}
	// loop over lights
	for (int i = 0; i < iLightCount; i++)
	{
		// This framework calculates lighting in world space.
		// For every light type, you should calculate the input values to the
		// calcPhongLighting function, namely light direction and the reflection vector.
		// For computuation of attenuation and the spot light factor, use the
		// model from the DirectX documentation:
		// http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx

		if (iLightType[i] == 1) // directional
		{
			float3 d = normalize((float3)vLightDir[i]);
				float3 r = reflect(-d, input.n);
				I += s * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else if (iLightType[i] == 2)  // point
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir	
				float dl = length(d);
			d = normalize(d);
			float3 r = reflect(-d, input.n);
				float att = 1.0f / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else if (iLightType[i] == 3)  // spot
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir
				float dl = length(d);
			d = normalize(d);
			float3 r = reflect(-d, input.n);
				float3 sd = normalize((float3)vLightDir[i]);	// missuse the vLightDir variable for spot-dir

				/* --- this is the  DirectX9 version (better) --- */
				float rho = dot(-d, sd);
			float spot = pow(saturate((rho - vLightSpot[i].x) / (vLightSpot[i].y - vLightSpot[i].x)), vLightSpot[i].z);
			float att = spot / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else
		{
			//I += 0;
		}
	}

	/// set diffuse alpha
	I.a = vDiffuseColor.a;

	int pRes = CalcPlanes(input);
	if (pRes >= 0)
	{
		I = vPlaneColors[pRes];
	}


	return I;
}

float4 PShaderPhongFColorByOutOfBox(PSInput input) : SV_Target
{
	return PShaderPhongCoreColorByOutOfBox(input, DISCARD_BACK);
}

float4 PShaderPhongBColorByOutOfBox(PSInput input) : SV_Target
{
	return PShaderPhongCoreColorByOutOfBox(input, DISCARD_FRONT);
}

float4 PShaderPhongColorByOutOfBox(PSInput input) : SV_Target
{
	return PShaderPhongCoreColorByOutOfBox(input, DISCARD_NONE);
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG
//--------------------------------------------------------------------------------------
float4 PSShaderBlinnPhongCore(PSInput input, int discardKind)
{

	float4 I;

	// renormalize interpolated vectors
	input.n = calcNormal(input);

	// get per pixel vector to eye-position
	//float3 eye = normalize(vEyeLook);
	float3 eye = normalize(vEyePos - input.wp.xyz);

		float angle = dot(input.n, eye);
	if (discardKind == DISCARD_FRONT && angle >= 0)
		discard;
	if (discardKind == DISCARD_BACK && angle < 0)
		discard;

	if (!bDisableBack && angle < 0)
		input.n = input.n * -1;

	// light emissive intensity and add ambient light
	I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;

	// get shadow color
	float s = 1;
	if (bHasShadowMap)
	{
		s = shadowStrength(input.sp);
	}

	// add diffuse sampling
	float4 vDiffuseColor = vMaterialDiffuse;
		if (bHasDiffuseMap)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
				vDiffuseColor = texDiffuseMap.Sample(LinearSampler, input.t);
			else
				vDiffuseColor *= texDiffuseMap.Sample(LinearSampler, input.t);
		}
	if (!bDisableBack && angle < 0)
	{
		vDiffuseColor = vMaterialDiffuseBack;
		if (bHasDiffuseMapBack)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
				vDiffuseColor = texDiffuseMapBack.Sample(LinearSampler, input.t);
			else
				vDiffuseColor *= texDiffuseMapBack.Sample(LinearSampler, input.t);
		}
	}

	// compute lighting
	for (int i = 0; i < iLightCount; i++)
	{
		// Same as for the Phong PixelShader, but use
		// calcBlinnPhongLighting instead.
		if (iLightType[i] == 1) // directional
		{
			float3 d = normalize((float3)vLightDir[i]);  // light dir	
				float3 h = normalize(eye + d);
				I += s * calcBlinnPhongLighting(vLightColor[i], vDiffuseColor, input.n, d, h);
		}
		else if (iLightType[i] == 2)  // point
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	// light dir
				float dl = length(d);							// light distance
			d = d / dl;										// normalized light dir						
			float3 h = normalize(eye + d);				// half direction for specular
				float att = 1.0f / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcBlinnPhongLighting(vLightColor[i], vDiffuseColor, input.n, d, h);
		}
		else if (iLightType[i] == 3)  // spot
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	// light dir
				float  dl = length(d);							// light distance
			d = d / dl;										// normalized light dir					
			float3 h = normalize(eye + d);				// half direction for specular
				float3 sd = normalize((float3)vLightDir[i]);	// missuse the vLightDir variable for spot-dir

				/* --- this is the OpenGL 1.2 version (not so nice) --- */
				//float spot = (dot(-d, sd));
				//if(spot > cos(vLightSpot[i].x))
				//	spot = pow( spot, vLightSpot[i].y );
				//else
				//	spot = 0.0f;	
				/* --- */

				/* --- this is the  DirectX9 version (better) --- */
				float rho = dot(-d, sd);
			float spot = pow(saturate((rho - vLightSpot[i].x) / (vLightSpot[i].y - vLightSpot[i].x)), vLightSpot[i].z);
			float att = spot / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att*calcBlinnPhongLighting(vLightColor[i], vDiffuseColor, input.n, d, h);
		}
		else
		{
			//I += 0;
		}
	}
	I.a = vDiffuseColor.a;

	// get reflection-color
	if (bHasCubeMap)
	{
		I = cubeMapReflection(input, I);
	}

	clip(I.a < 0.1f ? -1 : 1);

	int pRes = CalcPlanes(input);
	if (pRes >= 0)
	{
		I = vPlaneColors[pRes];
	}

	return I;
}

float4 PSShaderBlinnPhongF(PSInput input) : SV_Target
{
	return PSShaderBlinnPhongCore(input, DISCARD_BACK);
}

float4 PSShaderBlinnPhongB(PSInput input) : SV_Target
{
	return PSShaderBlinnPhongCore(input, DISCARD_FRONT);
}

//--------------------------------------------------------------------------------------
// Given Per-Vertex Color
//--------------------------------------------------------------------------------------
float4 PShaderColor(PSInput input) : SV_Target
{
	float4 I;

	//calculate lighting vectors - renormalize vectors	
	input.n = calcNormal(input);

	// get per pixel vector to eye-position
	//float3 eye = normalize(vEyeLook);
	float3 eye = normalize(vEyePos - input.wp.xyz);

		float angle = dot(input.n, eye);
	//if (discardKind == DISCARD_FRONT && angle >= 0)
	//	discard;
	//if (discardKind == DISCARD_BACK && angle < 0)
	//	discard;

	if (!bDisableBack && angle < 0)
		input.n = input.n * -1;

	// light emissive and ambient intensity
	// this variable can be used for light accumulation
	I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;
	//	I *= -1;
	// get shadow color
	float s = 1;
	if (bHasShadowMap)
	{
		s = shadowStrength(10);
	}

	// add diffuse sampling
	float4 vDiffuseColor = vMaterialDiffuse;
		if (bHasDiffuseMap)
		{
			// SamplerState is defined in Common.fx.
			vDiffuseColor *= texDiffuseMap.Sample(LinearSampler, input.t);
		}
	if (!bDisableBack && angle < 0)
	{
		vDiffuseColor = vMaterialDiffuseBack;
		if (bHasDiffuseMapBack)
		{
			// SamplerState is defined in Common.fx.
			if (bDisableBlandDif)
				vDiffuseColor = texDiffuseMapBack.Sample(LinearSampler, input.t);
			else
				vDiffuseColor *= texDiffuseMapBack.Sample(LinearSampler, input.t);
		}
	}

	// loop over lights
	for (int i = 0; i < iLightCount; i++)
	{
		// This framework calculates lighting in world space.
		// For every light type, you should calculate the input values to the
		// calcPhongLighting function, namely light direction and the reflection vector.
		// For computuation of attenuation and the spot light factor, use the
		// model from the DirectX documentation:
		// http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx

		if (iLightType[i] == 1) // directional
		{
			float3 d = normalize((float3)vLightDir[i]);
				float3 r = reflect(-d, input.n);
				I += s * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else if (iLightType[i] == 2)  // point
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir	
				float dl = length(d);
			d = normalize(d);
			float3 r = reflect(-d, input.n);
				float att = 1.0f / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else if (iLightType[i] == 3)  // spot
		{
			float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir
				float dl = length(d);
			d = normalize(d);
			float3 r = reflect(-d, input.n);
				float3 sd = normalize((float3)vLightDir[i]);	// missuse the vLightDir variable for spot-dir

				/* --- this is the  DirectX9 version (better) --- */
				float rho = dot(-d, sd);
			float spot = pow(saturate((rho - vLightSpot[i].x) / (vLightSpot[i].y - vLightSpot[i].x)), vLightSpot[i].z);
			float att = spot / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
			I += att * calcPhongLightingWithColor(vLightColor[i], vDiffuseColor, input.n, d, eye, r, input.c);
		}
		else
		{
			//I += 0;
		}
	}

	/// set diffuse alpha
	I.a = vDiffuseColor.a;

	int pRes = CalcPlanes(input);
	if (pRes >= 0)
	{
		I = vPlaneColors[pRes];
	}

	return I;
	//return input.c;
}

//--------------------------------------------------------------------------------------
//  Render Positions as Color
//--------------------------------------------------------------------------------------
//float4 PShaderPositions(PSInput input) : SV_Target
//{
//	return float4(input.wp.xyz, 1);
//}

//--------------------------------------------------------------------------------------
//  Render Normals as Color
//--------------------------------------------------------------------------------------
//float4 PShaderNormals(PSInput input) : SV_Target
//{
//	return float4(input.n*0.5 + 0.5, 1);
//}

//--------------------------------------------------------------------------------------
//  Render Perturbed normals as Color
//--------------------------------------------------------------------------------------
//float4 PShaderPerturbedNormals(PSInput input) : SV_Target
//{
//	return float4(calcNormal(input)*0.5 + 0.5, 1.0f);
//}

//--------------------------------------------------------------------------------------
//  Render Tangents as Color
//--------------------------------------------------------------------------------------
//float4 PShaderTangents(PSInput input) : SV_Target
//{
//	return float4(input.t1*0.5 + 0.5, 1);
//}

//--------------------------------------------------------------------------------------
//  Render TexCoords as Color
//--------------------------------------------------------------------------------------
//float4 PShaderTexCoords(PSInput input) : SV_Target
//{
//	return float4(input.t, 1, 1);
//}

//--------------------------------------------------------------------------------------
// diffuse map pixel shader
//--------------------------------------------------------------------------------------
//float4 PShaderDiffuseMap(PSInput input) : SV_Target
//{
//	// SamplerState is defined in Common.fx.
//	return texDiffuseMap.Sample(LinearSampler, input.t);
//}

//--------------------------------------------------------------------------------------
// empty pixel shader
//--------------------------------------------------------------------------------------
//void PShaderEmpty(PSInput input)
//{
//}


//--------------------------------------------------------------------------------------
// CUBE-MAP funcs
//--------------------------------------------------------------------------------------
struct PSInputCube
{
	float4 p  : SV_POSITION;
	float3 t  : TEXCOORD;
	float4 c  : COLOR;
};

PSInputCube VShaderCubeMap(float4 p : POSITION)
{
	PSInputCube output = (PSInputCube)0;

	//set position into clip space		
	output.p = mul(p, mWorld);
	output.p = mul(output.p, mView);
	output.p = mul(output.p, mProjection).xyww;

	//set texture coords and color
	//output.t = input.t;	
	output.c = p;

	//Set Pos to xyww instead of xyzw, so that z will always be 1 (furthest from camera)	
	output.t = p.xyz;

	return output;
}

float4 PShaderCubeMap(PSInputCube input) : SV_Target
{
	return texCubeMap.Sample(LinearSampler, input.t);
	//return float4(input.t,1);
	return float4(1, 0, 0, 1);
}




//--------------------------------------------------------------------------------------
// Techniques
//--------------------------------------------------------------------------------------
technique11 RenderPhong
{
	pass P0
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhongB()));
	}
	pass P1
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhongF()));
	}
}

technique11 RenderPhongPointSampler
{
	pass P0
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhongBPointSampler()));
	}
	pass P1
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhongFPointSampler()));
	}
}

technique11 RenderBackground
{
	pass P0
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderSimple2()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderSimple2()));
		//SetPixelShader(CompileShader(ps_4_0, PShaderSimple2()));//PShaderPhongBPointSampler()));
	}
	//pass P1
	//{
	//	SetRasterizerState(RSSolid);
	//	SetDepthStencilState(DSSDepthLess, 0);
	//	SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
	//	SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
	//	SetHullShader(NULL);
	//	SetDomainShader(NULL);
	//	SetPixelShader(CompileShader(ps_4_0, PShaderPhongFPointSampler()));
	//}
}

technique11 RenderPhongColorByOutOfBox
{
	pass P0
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhongBColorByOutOfBox()));
	}
	pass P1
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhongFColorByOutOfBox()));
	}
}

technique11 RenderPhongColorByBox
{
	pass P0
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhongBColorByBox()));
	}
	pass P1
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhongFColorByBox()));
	}
}

technique11 RenderPhongWithAmbient
{
	pass P0
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhongWithAmbientB()));
	}
	pass P1
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhongWithAmbientF()));
	}
}

technique11 RenderPhongWithSphereColoring
{
	pass P0
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhongWithSphereColoringB()));
	}
	pass P1
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhongWithSphereColoringF()));
	}
}

technique11 RenderBlinn
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);

		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PSShaderBlinnPhongB()));
	}
	pass P1
	{
		//SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);

		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PSShaderBlinnPhongF()));
	}
}

//technique11 RenderDiffuse
//{
//	pass P0
//	{
//		//SetRasterizerState	( RSSolid );
//		SetDepthStencilState(DSSDepthLess, 0);
//		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
//		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
//		SetHullShader(NULL);
//		SetDomainShader(NULL);
//		SetGeometryShader(NULL);
//		SetPixelShader(CompileShader(ps_4_0, PShaderDiffuseMap()));
//	}
//	pass P1
//	{
//		SetRasterizerState(RSWire);
//		SetDepthStencilState(DSSDepthLess, 0);
//		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
//		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
//		SetHullShader(NULL);
//		SetDomainShader(NULL);
//		SetGeometryShader(NULL);
//		SetPixelShader(CompileShader(ps_4_0, PShaderDiffuseMap()));
//	}
//}

technique11 RenderColors
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderColor()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderColor()));
	}
}

//technique11 RenderPositions
//{
//	pass P0
//	{
//		//SetRasterizerState	( RSSolid );
//		SetDepthStencilState(DSSDepthLess, 0);
//		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
//		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
//		SetHullShader(NULL);
//		SetDomainShader(NULL);
//		SetGeometryShader(NULL);
//		SetPixelShader(CompileShader(ps_4_0, PShaderPositions()));
//	}
//	pass P1
//	{
//		SetRasterizerState(RSWire);
//		SetDepthStencilState(DSSDepthLess, 0);
//		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
//		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
//		SetHullShader(NULL);
//		SetDomainShader(NULL);
//		SetGeometryShader(NULL);
//		SetPixelShader(CompileShader(ps_4_0, PShaderPositions()));
//	}
//}

//technique11 RenderNormals
//{
//	pass P0
//	{
//		//SetRasterizerState	( RSSolid );
//		SetDepthStencilState(DSSDepthLess, 0);
//		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
//		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
//		SetHullShader(NULL);
//		SetDomainShader(NULL);
//		SetGeometryShader(NULL);
//		SetPixelShader(CompileShader(ps_4_0, PShaderNormals()));
//	}
//	pass P1
//	{
//		SetRasterizerState(RSWire);
//		SetDepthStencilState(DSSDepthLess, 0);
//		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
//		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
//		SetHullShader(NULL);
//		SetDomainShader(NULL);
//		SetGeometryShader(NULL);
//		SetPixelShader(CompileShader(ps_4_0, PShaderNormals()));
//	}
//}

//technique11 RenderPerturbedNormals
//{
//	pass P0
//	{
//		//SetRasterizerState	( RSSolid );
//		SetDepthStencilState(DSSDepthLess, 0);
//		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
//		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
//		SetHullShader(NULL);
//		SetDomainShader(NULL);
//		SetGeometryShader(NULL);
//		SetPixelShader(CompileShader(ps_4_0, PShaderPerturbedNormals()));
//	}
//	pass P1
//	{
//		SetRasterizerState(RSWire);
//		SetDepthStencilState(DSSDepthLess, 0);
//		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
//		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
//		SetHullShader(NULL);
//		SetDomainShader(NULL);
//		SetGeometryShader(NULL);
//		SetPixelShader(CompileShader(ps_4_0, PShaderPerturbedNormals()));
//	}
//}

//technique11 RenderTangents
//{
//	pass P0
//	{
//		//SetRasterizerState	( RSSolid );
//		SetDepthStencilState(DSSDepthLess, 0);
//		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
//		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
//		SetHullShader(NULL);
//		SetDomainShader(NULL);
//		SetGeometryShader(NULL);
//		SetPixelShader(CompileShader(ps_4_0, PShaderTangents()));
//	}
//	pass P1
//	{
//		SetRasterizerState(RSWire);
//		SetDepthStencilState(DSSDepthLess, 0);
//		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
//		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
//		SetHullShader(NULL);
//		SetDomainShader(NULL);
//		SetGeometryShader(NULL);
//		SetPixelShader(CompileShader(ps_4_0, PShaderTangents()));
//	}
//}

//technique11 RenderTexCoords
//{
//	pass P0
//	{
//		//SetRasterizerState	( RSSolid );
//		SetDepthStencilState(DSSDepthLess, 0);
//		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
//		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
//		SetHullShader(NULL);
//		SetDomainShader(NULL);
//		SetGeometryShader(NULL);
//		SetPixelShader(CompileShader(ps_4_0, PShaderTexCoords()));
//	}
//	pass P1
//	{
//		SetRasterizerState(RSWire);
//		SetDepthStencilState(DSSDepthLess, 0);
//		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
//		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
//		SetHullShader(NULL);
//		SetDomainShader(NULL);
//		SetGeometryShader(NULL);
//		SetPixelShader(CompileShader(ps_4_0, PShaderTexCoords()));
//	}
//}

/*
technique11 RenderWires
{
pass P0
{
SetRasterizerState(RSWire);
SetDepthStencilState(DSSDepthLess, 0);
//SetBlendState( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );

SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
SetHullShader(NULL);
SetDomainShader(NULL);
SetGeometryShader(NULL);
SetPixelShader(CompileShader(ps_4_0, PShaderPhong()));
}
pass P1
{
SetRasterizerState(RSWire);
SetDepthStencilState(DSSDepthLess, 0);
//SetBlendState( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );

SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
SetHullShader(NULL);
SetDomainShader(NULL);
SetGeometryShader(NULL);
SetPixelShader(CompileShader(ps_4_0, PShaderPhong()));
}
}
*/

technique11 RenderWires
{
	pass P0
	{
		SetRasterizerState(RSSolid);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSNoBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);

		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(CompileShader(gs_4_0, GShaderWires()));
		SetPixelShader(CompileShader(ps_4_0, PShaderPhong()));
	}
}

technique11 RenderVertex
{
	pass P0
	{
		SetRasterizerState(RSVertex);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlendingVertex, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);

		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(CompileShader(gs_4_0, GShaderVertex_P1()));
		SetPixelShader(CompileShader(ps_4_0, PShaderPhong()));
	}
}

technique11 RenderCubeMap
{
	pass P0
	{
		SetRasterizerState(RSSolidCubeMap);
		SetDepthStencilState(DSSDepthLessEqual, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		//SetBlendState( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0x00000000 );

		SetVertexShader(CompileShader(vs_4_0, VShaderCubeMap()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderCubeMap()));
	}
}
