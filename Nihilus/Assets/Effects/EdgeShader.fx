sampler2D uImage0 : register(s0);
float3 uColor;
float uAlpha;
float2 uSize;

float2 resize(float2 coords, float2 offset)
{
    return ((coords * uSize) + offset) / uSize;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float edge[4];
    
    edge[0] = tex2D(uImage0, resize(coords, float2(1, 0))).a;
    edge[1] = tex2D(uImage0, resize(coords, float2(0, 1))).a;
    edge[2] = tex2D(uImage0, resize(coords, float2(-1, 0))).a;
    edge[3] = tex2D(uImage0, resize(coords, float2(0, -1))).a;
    
    float iAlpha = tex2D(uImage0, coords).a;
    
    if ((edge[0] < 1 || edge[0] < 1 || edge[0] < 1 || edge[0] < 1) && iAlpha == 0)
        return float4(uColor, uAlpha);

    float coloration = tex2D(uImage0, coords).r;
    return float4(coloration, coloration, coloration, coloration);

}

technique Technique1
{
    pass EdgePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}