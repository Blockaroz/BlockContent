sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;

float4 GrayscaleFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;

    color.r = (color.r * 0.299) + (color.g * 0.587) + (color.b * 0.114);
    color.g = (color.r * 0.299) + (color.g * 0.587) + (color.b * 0.114);
    color.b = (color.r * 0.299) + (color.g * 0.587) + (color.b * 0.114);

    return color;
}

technique Technique1
{
    pass GrayscalePass
    {
        PixelShader = compile ps_2_0 GrayscaleFunction();
    }
}