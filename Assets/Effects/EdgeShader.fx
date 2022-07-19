sampler2D uImage0 : register(s0);
bool useLight;
float3 uColor;
float uAlpha;
float2 uSize;

float2 resize(float2 coords, float2 offset)
{
    return ((coords * uSize) + offset) / uSize;
}

float4 PixelShaderFunction(float2 input : TEXCOORD0) : COLOR0
{
    float edge[4];
    
    edge[0] = tex2D(uImage0, resize(input, float2(1, 0))).a;
    edge[1] = tex2D(uImage0, resize(input, float2(0, 1))).a;
    edge[2] = tex2D(uImage0, resize(input, float2(-1, 0))).a;
    edge[3] = tex2D(uImage0, resize(input, float2(0, -1))).a;
    
    float4 color = tex2D(uImage0, input);
    
    if ((edge[0] == 0 || edge[1] == 0 || edge[2] == 0 || edge[3] == 0) && color.a > 0)
    {
        if (useLight)
            return float4(uColor * color.rgb, uAlpha);
        else
            return float4(uColor, uAlpha);

    }
    
    return color * 0.33;

}

technique Technique1
{
    pass EdgePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}