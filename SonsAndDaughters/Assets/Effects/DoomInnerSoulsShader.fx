﻿sampler2D uImage0 : register(s0);
texture soulTexture;
sampler2D soulTex = sampler_state
{
    texture = <soulTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture distortionTexture;
sampler2D distTex = sampler_state
{
    texture = <distortionTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float4 uColor;
float4 uSecColor;
float uTime;
float uSize;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 center = coords * 2.0 - 1.0;
    float2 polar = float2(atan2(center.x, center.y) / 6.28 - sin(length(center)) * 0.4, pow(length(center * 0.66), 2) + uTime);
    
    float4 souls = tex2D(soulTex, polar + float2(uTime, 0)) * smoothstep(0.66, 0.1, length(center)) * uColor;
    
    if (length(center) < 0.98)
        return lerp(uSecColor, float4(0, 0, 0, 1), length(center)) + souls;
    
    return 0;
}

technique Technique1
{
    pass SoulPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
