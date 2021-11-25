sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uImageSize0;
float2 uImageSize1;

float4 NightEmpressWingsFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 imageCoords = (coords * uImageSize0 - uSourceRect.xy) / uImageSize1;
    float4 image = tex2D(uImage1, imageCoords);
    color.rgb = image.rgb;
    return color * sampleColor.a;
}

technique Technique1
{
    pass NightEmpressWings
    {
        PixelShader = compile ps_2_0 NightEmpressWingsFunction();
    }
}