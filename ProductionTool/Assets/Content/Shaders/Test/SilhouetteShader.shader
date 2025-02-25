Shader "Custom/SolidColorSpriteWithAlpha"
{
    Properties
    {
        _Color ("Sprite Color", Color) = (1, 1, 1, 1)  // The solid color
        _MainTex ("Base (RGB)", 2D) = "white" { }    // The main sprite texture for transparency
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            // Make sure to handle transparency
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
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
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Input
            uniform float4 _Color;     // The solid color to apply
            uniform float4 _MainTex_ST; // Texture scale/offset

            // Vertex Shader
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // Pass the UV coordinates as they are
                return o;
            }

            // Fragment Shader
            sampler2D _MainTex;
            float4 frag(v2f i) : COLOR
            {
                // Sample the texture to check for alpha transparency
                float4 texColor = tex2D(_MainTex, i.uv);

                // // If the texture has any alpha (i.e. transparency), we discard the pixel
                // if (texColor.a < 0.1) 
                //     discard; // This makes the sprite transparent where it should be

                // If the texture is not transparent, apply the solid color
                return _Color * texColor.a; // Use texColor.a to keep transparency
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}




