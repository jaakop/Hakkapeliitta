#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
sampler TextureSampler : register(s0);

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float4 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float4 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	output.TexCoord = input.TexCoord;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 diffuse = tex2D(TextureSampler, input.TexCoord.xy);
	float l = length(0.5 - input.TexCoord.xy);
	float d = smoothstep(0.008, 0.1, 0.5 - l);
    return float4(1, l, l * 0.1 + 0.5, d);	
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};