//-----------------------------------------------------------------------------
// Sky.fx
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


float4x4 View;
float4x4 Projection;

texture Texture;


struct VS_INPUT
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};


struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};


VS_OUTPUT VertexShader(VS_INPUT input)
{
    VS_OUTPUT output;

    // The view matrix contains information about the position of the camera as
    // well as which way it is facing. We need the sky rendering to respond to
    // which way the camera is looking, but to ignore the position, because we
    // always want the skydome to be centered on the camera, no matter where it
    // moves in the world. To achieve this we declare the input position as a
    // float3, rather than the usual float4. By leaving out the w component, the
    // matrix multiply will discard the transform information from the view matrix.

    float3 position = mul(input.Position, View);
    
    // The perspective projection does still need to include a w value,
    // however, so we must now add that back in.

    output.Position = mul(float4(position, 1), Projection);
    
    // We always want the sky to be drawn as the furthest thing away,
    // so we artificially override the depth value.

    output.Position.z = output.Position.w;

    output.TexCoord = input.TexCoord;
    
    return output;
}


sampler Sampler = sampler_state
{
    Texture = (Texture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Clamp;
};


float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    return tex2D(Sampler, texCoord);
}


technique EnvironmentMap
{
    pass Pass1
    {
        VertexShader = compile vs_1_1 VertexShader();
        PixelShader = compile ps_1_1 PixelShader();
        
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
        
        ZEnable = true;
        ZWriteEnable = false;
    }
}
