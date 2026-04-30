#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float time = 0;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;
    //uv = (int2)(uv * 8) / 8.0; //or floor <--pixelate
    float wave = 0.5 + 0.3 * sin(time * 1.7 + uv.y * 14 + sin(uv.x * 3.1415));
    //float edge = uv.y > 0.9 ? 1 : 0;
    //float edge = uv.y * uv.y * uv.y;
    float edge = max(0, -6 + uv.y * 7.5);
    float sides = max(0, -5 + abs(uv.x - 0.5) * 12);
    //float wiggle = 0.9 + 0.15 * cos(abs(uv.x - 0.5) * 2 * 5 - time * 5 + uv.y * 5);
    float4 res = input.Color;
    //float a = uv.y * uv.y;
    //float a = 0.5 - 0.5*cos(uv.y * 1.57);
    float a = tex2D(SpriteTextureSampler, uv + float2(0, sin(time + uv.y * 6 + uv.x * 15) * 0.1));
    a *= wave * wave; //*wiggle;
    a += edge - sides;
    res.a = a;
    res.rgb *= a;
    return res;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};