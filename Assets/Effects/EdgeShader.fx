sampler2D uImage0 : register(s0);
float useLight;
float3 uColor;
float4 uSecColor;
float uAlpha;
float2 uSize;

float2 resize(float2 coords, float2 offset)
{
    return ((coords * uSize) + offset) / uSize;
}

float4 PixelShaderFunction(float2 input : TEXCOORD0) : COLOR0
{
    float edge[4];
    
    edge[0] = length(tex2D(uImage0, resize(input, float2(1, 0))).rgba) / 4;
    edge[1] = length(tex2D(uImage0, resize(input, float2(0, 1))).rgba) / 4;
    edge[2] = length(tex2D(uImage0, resize(input, float2(-1, 0))).rgba) / 4;
    edge[3] = length(tex2D(uImage0, resize(input, float2(0, -1))).rgba) / 4;
    
    float4 color = tex2D(uImage0, input);
    
    if ((edge[0] == 0 || edge[1] == 0 || edge[2] == 0 || edge[3] == 0) && length(color.rgba) / 4 > 0)
    {
        return float4(lerp(uColor, color.rgb * uColor * 2, useLight), uAlpha);
    }
    
    return color * uSecColor;

}

technique Technique1
{
    pass EdgePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}