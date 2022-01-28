sampler2D baseImage : register(s0);

texture uImage0;
sampler2D image0 = sampler_state
{
    texture = <uImage0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float3 uColor0;
float3 uColor1;
float2 uImageSize;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 energy = float4(0, 0, 0, 0);
    //const float p = 1.5;
    //sampleColor += tex2D(baseImage, coords + (float2(p, p) / uImageSize));
    //sampleColor += tex2D(baseImage, coords + (float2(p, -p) / uImageSize));
    //sampleColor += tex2D(baseImage, coords + (float2(-p, p) / uImageSize));
    //sampleColor += tex2D(baseImage, coords + (float2(-p, -p) / uImageSize));
    if (sampleColor.r + sampleColor.g + sampleColor.b + sampleColor.a > 0.0)
    {
        energy.rgba = float4(uColor0, sampleColor.a);
    }

    return energy;
}

technique Technique1
{
    pass Color
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}