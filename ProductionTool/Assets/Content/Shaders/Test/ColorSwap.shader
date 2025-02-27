Shader "Unlit/ColorSwap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _OldColors ("Old Colors", Vector) = (1,1,1,1)
        _NewColors ("New Colors", Vector) = (1,1,1,1) 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            // Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
            #pragma exclude_renderers d3d11 gles
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
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float4[] _OldColors;
                float4[] _NewColors;

                float4 oldColor;
                float4 newColor;

                for(int i = 0; i < _OldColors.length; i++)
                {
                    oldColor = _OldColors[i];
                    newColor = _NewColors[i];
                }

                float distance = dot(col.rgb - oldColor.rgb, col.rgb - oldColor.rgb);

                if(distance < 0.01f && col.a > 0.0f)
                {
                    return newColor;
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
