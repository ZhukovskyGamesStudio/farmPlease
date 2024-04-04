Shader "Custom/GrayscaleWithColor_Maskable"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness ("Brightness", Range(0, 2)) = 1
        _Color ("Color", Color) = (1, 1, 1, 1)
        _UseMask ("Use Mask", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
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
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float _Brightness;
            fixed4 _Color;
            float _UseMask;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 color = tex2D(_MainTex, i.uv);
                
                // Convert to grayscale
                float grayscale = dot(color.rgb, float3(0.299, 0.587, 0.114));
                
                // Apply brightness
                grayscale *= _Brightness;
                
                // Apply color tint
                color *= _Color;
                
                // Mix grayscale and color, preserving alpha
                color.rgb *= grayscale;
                color.a *= _Color.a;

                // Apply mask if enabled
                if (_UseMask > 0.5)
                {
                    // Set the stencil buffer
                    clip(1 - tex2D(_MainTex, i.uv).a);
                }

                return color;
            }
            ENDCG
        }
    }
}