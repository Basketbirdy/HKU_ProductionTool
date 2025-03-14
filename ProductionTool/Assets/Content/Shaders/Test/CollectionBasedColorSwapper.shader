Shader"Unlit/CollectionBasedColorSwapper"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _ColorTex ("Color Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Texture
            // Texture samplers
            sampler2D _MainTex;
            sampler2D _ColorTex;

            // Structure for vertex data
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Structure for fragment data
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 _ColorTex_TexelSize;

// Fragment shader for color replacement
half4 frag(v2f i) : SV_Target
{
    // Sample the original color from the main texture
    float4 originalColor = tex2D(_MainTex, i.uv);
    
    //if (originalColor.a == 0)
    //{
    //    discard;
    //}
    
    float finalLookupX = -1;
    
    // Get the color texture size
    float2 colorTexSize = tex2D(_ColorTex, float2(0.5, 0.5)).xy;
    float colorTexWidth = colorTexSize.x;
    
    // Set a maximum loop count (based on the number of colors in the palette)
    int maxColorCount = 256; // You can change this to the actual number of colors
    
    for (int j = 0; j < maxColorCount; j++)
    {
        float lookupX = (j + 0.5) * (1 / 256);
        float4 targetColor = tex2D(_ColorTex, float2(lookupX, (0 + 0.5) * (1 / 2)));
        
        float distance = dot(originalColor.rgb - targetColor.rgb, originalColor.rgb - targetColor.rgb);
        if (distance < 0.01)
        {
            finalLookupX = lookupX;
            break;
        }
    }
                
    if (finalLookupX < 0)
    {
        return originalColor;
    }
    
    float4 replacementColor = tex2D(_ColorTex, float2(finalLookupX, (1 + 0.5) * (1 / 2)));
    
    return replacementColor;
}

//half4 frag(v2f i) : SV_Target
//{   
//    // Sample the original color from the main texture
//    float4 originalColor = tex2D(_MainTex, i.uv);
    
//    // determine target color
    
//    float distance = dot(originalColor.rgb - targetColor.rgb, originalColor.rgb - targetColor.rgb);
    
//    if (distance < 0.001f && col.a > 0.0f)
//    {
//        return _NewColor;
//    }
//    else
//    {
//        if (col.a == 0)
//        {
//            discard;
//        }
//        return col;
//    }
//}

            ENDCG
        }
    }
FallBack"Diffuse"
}
