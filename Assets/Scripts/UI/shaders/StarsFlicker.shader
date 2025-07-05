Shader "Custom/StarsFlicker"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlickerSpeed ("Flicker Speed", Range(0.01, 5.0)) = 1.0
        _FlickerAmount ("Flicker Amount", Range(0.0, 1.0)) = 0.5
        _BrightnessThreshold ("Brightness Threshold", Range(0.0, 1.0)) = 0.5
        _GridSize ("Grid Size", Range(0.01, 0.1)) = 0.05
        _PauseFrequency ("Pause Frequency", Range(0.1, 20.0)) = 1.0
        _PauseChance ("Pause Chance", Range(0.0, 1.0)) = 0.5
        _BackgroundColor ("Background Color", Color) = (0, 0, 1, 1)
    }
    SubShader
    {
        //Tags { "RenderType"="Overlay" "Queue"="Overlay" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha

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
            float _FlickerSpeed;
            float _FlickerAmount;
            float _BrightnessThreshold;
            float _GridSize;
            float _PauseFrequency;
            float _PauseChance;
            float4 _BackgroundColor;

            float random(float2 uv)
            {
                float2 gridUV = floor(uv / _GridSize) * _GridSize;
                return frac(sin(dot(gridUV, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float brightness = dot(col.rgb, float3(0.299, 0.587, 0.114));
                fixed4 backgroundColor = _BackgroundColor;

                if (brightness > _BrightnessThreshold)
                {
                    float rnd = random(i.uv);
                    float pause = step(_PauseChance, rnd);
                    float flicker = sin(_Time.y * (_FlickerSpeed + _FlickerSpeed * rnd * 5.0)) * _FlickerAmount;

                    if (pause > 0.0)
                    {
                        col.rgb = backgroundColor.rgb + float3(1, 1, 1) * (1.0 + flicker);
                    }
                }

                return col;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
