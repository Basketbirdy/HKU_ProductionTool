Shader"Unlit/PaletteSwapShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
_InPalette ("In Palette", 2D) = "white" {}
_OutPalette ("Out Palette", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
LOD100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};

sampler2D _MainTex;
float4 _MainTex_ST;

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    return o;
}

sampler2D _InPalette;
sampler2D _OutPalette;

fixed4 frag(v2f i) : SV_Target
{
                // sample the texture
    fixed4 col = tex2D(_MainTex, i.uv);
    
    //if (col.a == 0)
    //{
    //    return float4(0, 0, 0, 0);
    //}
    
    //for (int j = 0; j < 256; j++)
    //{
    //    float4 targetColor = tex2D(_InPalette, float2((.5 / 256) * j, .5));
        
    //    if (targetColor.a == 0)
    //    {
    //        return float4(0, 0, 0, 0);
    //    }
        
    //    float distance = dot(col.rgb - targetColor.rgb, col.rgb - targetColor.rgb);
    //    if (distance < 0.001f)
    //    {
    //        float4 newColor = tex2D(_OutPalette, float2((.5 / 256) * j, .5));
    //        return newColor;
    //    }
    //}
    
    return col;
}
            ENDCG
        }
    }
}
