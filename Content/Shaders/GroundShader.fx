#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float3 CameraPos;
float4x4 World;
float4x4 View;
float4x4 Projection;

#define GridSize 1
#define LineWidth 0.45
#define Green float4(62 / 255.0, 184 / 255.0, 66 / 255.0, 1)
#define White float4(116 / 255.0, 232 / 255.0, 120 / 255.0, 1)

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

float Grid(float2 coord)
{
    float2 grid = abs(frac(coord * GridSize) - 0.5);
    float gridLine = step(max(grid.x, grid.y), LineWidth);
    return gridLine;
}

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    float4 wPos = mul(input.Position, World);
    float4 oPos = mul(wPos, mul(View, Projection));

    wPos.w = (wPos.y - CameraPos.y) / 10;

    output.Position = oPos;
    output.Color = wPos;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float gridEffect = Grid(input.Color.xz);
    float val = input.Color.w + .99;
    float4 col = lerp(White, Green, gridEffect) * clamp((val * val * val), 0, 1.5); //(1 - ((input.Color.w * input.Color.w) / Depth));
    return col;
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};