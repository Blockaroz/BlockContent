sampler2D uImage0 : register(s0);
texture distortionTexture0;
sampler2D distTex0 = sampler_state
{
    texture = <distortionTexture0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture distortionTexture1;
sampler2D distTex1 = sampler_state
{
    texture = <distortionTexture1>;
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

float Donut(float2 coords, float4 distortion)
{
    float ring = length(coords) > 0.85 - (distortion / uSize) && length(coords) < 1.0 - (distortion * uSize * 2) ? 1 : 0;
    return mul(ring, 1.0 - distortion * 2);
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 center = coords * 2.0 - 1.0;
    float2 polar = float2((atan2(center.x, center.y) / 6.28 + uTime), length(center) + uTime * 5);

    float4 distA = tex2D(distTex0, polar) * tex2D(distTex1, polar);

    float distortion = Donut(center, distA);

    if (distortion > 0)
        return lerp(uSecColor, uColor, Donut(center, distA));

    
    return 0;
}

technique Technique1
{
    pass OuterRingPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
