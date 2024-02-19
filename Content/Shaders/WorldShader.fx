#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Data
float4x4 World;
float4x4 ViewProjection;
float3x3 WorldInverseTranspose;
float3 CameraPos;

float3 Color;

float3 DiffuseDirection;
float4 DiffuseColor;
float DiffuseIntensity;

float3 AmbientColor;
float AmbientIntensity;

float Transparency;

texture Texture_Skybox;

// color
// rgb - color
// alpha - transparency
texture Texture_CT;
// spectular 
// red - AO
// green - roughness
// blue - metal
// alpha - colorability
texture Texture_ADD;

sampler2D Sampler_CT = sampler_state {
	Texture = (Texture_CT);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};
sampler2D Sampler_ADD = sampler_state {
	Texture = (Texture_ADD);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};
sampler2D Sampler_Skybox = sampler_state {
	Texture = (Texture_Skybox);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture Enviroment;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 ViewDir : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	output.TextureCoordinate = input.TextureCoordinate;
	output.Normal = mul(input.Normal, WorldInverseTranspose);
	
	float4 worldPos = mul(input.Position, World);
	output.Position = mul(worldPos, ViewProjection);

	output.ViewDir = normalize(CameraPos - worldPos);
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{ 
	float4 CTTexture = tex2D(Sampler_CT, input.TextureCoordinate);
	float4 ADDTexture = tex2D(Sampler_ADD, input.TextureCoordinate);

	float intensity = saturate(dot(input.Normal, DiffuseDirection));
	float3 ambient = AmbientColor * AmbientIntensity;
	float3 diffuse = DiffuseColor * intensity * DiffuseIntensity;

	float3 reflection = normalize(reflect(-DiffuseDirection, input.Normal));
	float specularIntensity = pow(saturate(dot(reflection, input.ViewDir)), (1 + ADDTexture.g) * 5);
	float specular = specularIntensity * (1 - ADDTexture.g) * ADDTexture.b;

	float3 finalColorRGB = (ambient + diffuse + specular) * (ADDTexture.a * CTTexture.rgb + (1 - ADDTexture.a) * Color) * ADDTexture.r;
	return float4(finalColorRGB, CTTexture.a * Transparency);
}

technique Default
{
	pass Pass0
	{
		AlphaBlendEnable = TRUE;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};