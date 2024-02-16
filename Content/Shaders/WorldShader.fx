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
float3 ViewDir;

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

	output.ViewDir = normalize(ViewDir - mul(input.Position, World));
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{ 
	float4 CT = tex2D(Sampler_CT, input.TextureCoordinate);
	float4 ADD = tex2D(Sampler_ADD, input.TextureCoordinate);

	float intensity = saturate(dot(input.Normal, DiffuseDirection));
	float3 ambient = AmbientColor * AmbientIntensity;
	float3 diffuse = DiffuseIntensity * DiffuseColor * intensity;

	float3 reflection = normalize(2 * dot(input.Normal, -DiffuseDirection) * input.Normal + DiffuseDirection);
	float specular = pow(saturate(dot(reflection, input.ViewDir)), ADD.b) * (1 - ADD.g);

	float4 finalColor = (float4(ambient + diffuse + specular, 1) * CT * ADD.r) + float4(Color * (1 - ADD.a), 1) ;
	finalColor.a = CT.a * Transparency;
    return finalColor;
}

technique Default
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};

technique Transparent
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