float uRotation;
float lengthPercent;
float4 uColor;

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

float cosine(float value)
{
    return sin(value + 1.57079);
}

float4 PixelShaderFunction(float2 uv : TEXCOORD) : COLOR
{
    float2x2 rotate = float2x2(cosine(uRotation), -sin(uRotation), sin(uRotation), cosine(uRotation));
    float diagonalDistance = (1 / (sqrt(2) / 4)) * (1 / (1 - lengthPercent));
    float2x2 rescale = float2x2(diagonalDistance, 0, 0, diagonalDistance);
    float realDistance = (1 / (1 - lengthPercent)) * (1 - 0.5 * (1 - lengthPercent));
   
    uv -= 0.5;
    uv = mul(uv, rotate);
    uv = mul(uv, rescale);
    uv += float2(-realDistance, realDistance);
    uv += 0.5;
    
    if (uv.x < 0 || uv.x >= 1 || uv.y < 0 || uv.y >= 1)
        return float4(0, 0, 0, 0);
    
    return tex2D(tex, uv) * uColor;
}

technique Technique1
{
    pass SwingPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}