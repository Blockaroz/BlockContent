sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float uTime;
float4 uSourceRect;
float2 uImageSize0;
float2 uImageSize1;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 imageCoords = (coords * uImageSize0 - uSourceRect.xy - frac(uTime)) / uImageSize1;
    float4 image = tex2D(uImage1, imageCoords);
    color.rgb = image.rgb;
    return color * sampleColor;
}

technique Technique1
{
    pass NightEmpress
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}