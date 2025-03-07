Shader "Unlit/ReplaceColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _OldColor ("Old Color", Color) = (1,1,1,1)
        _NewColor ("New Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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

            float4 _OldColor;
            float4 _NewColor;

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float distance = dot(col.rgb - _OldColor.rgb, col.rgb - _OldColor.rgb);

                if(distance < 0.001f && col.a > 0.0f)
                {
                    return _NewColor;
                }
                else
                {
                    if(col.a == 0)
                    {
                        discard;
                    }
                    return col;
                }
            }
            ENDCG
        }
    }
}
