#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 View;
float4x4 Projection;

Texture SkyboxTexture;
samplerCUBE SkyboxSampler = sampler_state
{
	texture = (SkyboxTexture);
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 ViewDirection : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 viewPosition = mul(input.Position, View);
	output.Position = mul(viewPosition, Projection);

	output.ViewDirection = normalize(input.Position.xyz);

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return texCUBE(SkyboxSampler, input.ViewDirection);
}

technique SkyboxRendering
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};