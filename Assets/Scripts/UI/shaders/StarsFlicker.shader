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
        _BackgroundColor ("Background Color", Color) = (0, 0, 1, 1) // Синий цвет фона
    }
    SubShader
    {
        Tags { "RenderType"="Overlay" "Queue"="Overlay" }
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

            // Псевдослучайная функция на основе координат с привязкой к сетке
            float random(float2 uv)
            {
                float2 gridUV = floor(uv / _GridSize) * _GridSize; // Привязка к сетке
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
                // Основная текстура
                fixed4 col = tex2D(_MainTex, i.uv);

                // Рассчитываем яркость пикселя (используем среднее значений RGB)
                float brightness = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // Цвет фона
                fixed4 backgroundColor = _BackgroundColor;

                // Применяем перекрашивание только к звёздам (ярким пикселям выше порога яркости)
                if (brightness > _BrightnessThreshold)
                {
                    // Генерируем случайное число для каждой звезды, используя сетку
                    float rnd = random(i.uv);

                    // Генерация пауз в мерцании
                    float pause = step(_PauseChance, rnd);  // Вероятность паузы
                    float flicker = sin(_Time.y * (_FlickerSpeed + _FlickerSpeed* rnd * 5.0)) * _FlickerAmount;

                    // Перекрашиваем пиксель в синий цвет фона
                    if(pause > 0.0)
                    {
                         col.rgb = backgroundColor.rgb  +  (1, 1, 1, 1) * (1.0 + flicker);
                    }
                   
                }
                else
                {
                    // Если пиксель не звезда, оставляем исходный цвет
                    col.rgb = col.rgb;
                }

                // Устанавливаем альфа-канал как исходный
                col.a = col.a;

                return col;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}

