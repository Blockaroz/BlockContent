sampler2D uImage0 : register(s0);
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
    float2 polar = float2(atan2(center.x, center.y) / 6.28 - length(center) * 0.4, length(center * 0.5) + uTime);
    
    float4 soul = tex2D(soulTex, polar);
    float4 dist = tex2D(distTex, polar) * (length(center) < 0.96 ? 1 : 0);
    float check = length(1 - soul.rgb) + smoothstep(0.9, uSize, length(center));
    float power = check < 0.6 ? 1 : 0;
    
    return power * uColor * smoothstep(uSize * 1.1, 1.1, length(center)) * smoothstep(0.98, 0.93, length(center));
}

technique Technique1
{
    pass SoulPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
