sampler2D uImage0 : register(s0);
texture uTexture;
sampler2D tex = sampler_state
{
    texture = <uTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float4 uColor;
float uProgress;

float4 PixelShaderFunction(float2 input : TEXCOORD0) : COLOR0
{
    return tex2D(tex, input + float2(uProgress, 0)) * uColor;
}

technique Technique1
{
    pass EdgePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}