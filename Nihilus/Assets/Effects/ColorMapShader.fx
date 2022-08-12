sampler2D uImage0 : register(s0);
texture sampleTexture0;
sampler2D sampleTex0 = sampler_state
{
    texture = <sampleTexture0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture sampleTexture1;
sampler2D sampleTex1 = sampler_state
{
    texture = <sampleTexture1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture palette;
sampler2D paletteTex = sampler_state
{
    texture = <palette>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = clamp;
    AddressV = clamp;
};
float2 uSizeImage;
float2 uSize0;
float2 uSize1;
float2 uPosition;
float uAlpha;
float uTime;

float4 PixelShaderFunction(float2 input : TEXCOORD0) : COLOR0
{
    float4 image = tex2D(uImage0, input);
    float2 dCoords0 = (input * uSizeImage + uPosition) / uSize0 + float2(-sin(uTime * 6.28 + 1.57) * 0.3, uTime);
    float2 dCoords1 = (input * uSizeImage + uPosition) / uSize1 + float2(sin(uTime * 6.28) * 0.1 - uTime, uTime * 2);
    float d0 = length(tex2D(sampleTex0, dCoords0).rgb);
    float d1 = length(tex2D(sampleTex1, dCoords1).rgb);
    float4 paletteColor = tex2D(paletteTex, float2(image.g, (d0 * d1) + image.b));
    if (length(image.rgb) / 3 > 0)
        return float4(paletteColor.rgb, uAlpha);
    
    return image;
}

technique Technique1
{
    pass ColorMapPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}