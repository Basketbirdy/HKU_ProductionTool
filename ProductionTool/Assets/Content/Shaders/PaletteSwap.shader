Shader"ColorSwapper/PaletteSwap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)

_BaseColors ("Base Colors", 2D) = "white" {}
_OutputColors ("Output Colors", 2D) = "white" {}

_LookupOffset ("Lookup Offset", Float) = 0.0


    }
    SubShader
    {
        Tags { 	
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        LOD 100

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }


sampler2D _BaseColors;
float4 _BaseColors_ST;
sampler2D _OutputColors;
float4 _OutputColors_ST;

float _LookupOffset;


            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 col = tex2D(_MainTex, i.uv);
               
    if (col.a == 0)
    {
        discard;
    }
    
    float lookupPosX = -1;
    
    for (int j = 0; j < 256; j++)
    {
        lookupPosX = (j + 0.5) * (1.0 / 255.0);
        float4 targetColor = tex2Dlod(_BaseColors, lookupPosX);

        float distance = dot(col.rgb - targetColor.rgb, col.rgb - targetColor.rgb);
        if (distance < 0.001f)
        {
            float4 newColor = tex2Dlod(_OutputColors, lookupPosX);
            return newColor;
        }
    }
    
    //    float4 targetcolor = tex2D(_BaseColors, lookupPosX);
    
    //float4 outputColor = tex2D(_OutputColors, lookupPosX);
    
    return col;
    
    
    //if (col.a > 0)
    //{
    //    return targetColor * col.a;
    //}
    //else
    //{
    //    discard;
    //}
    
            }
            ENDCG
        }
    }
}
