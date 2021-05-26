Shader "LevelSelect/BackgroundShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment IceLayerFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            // 波の頻度
            static const float wave_one_frequency = 80;
            static const float wave_two_frequency = 130;
            // 波の初期位相
            static const float wave_initial_phase = UNITY_PI;
            // 波の傾き
            static const float2 wave_one_angle = float2(0.4, -0.6);
            static const float2 wave_two_angle = float2(0.1, -0.9);
            // 波の速さ
            static const float wave_one_velocity = 1;
            static const float wave_two_velocity = 2;
            // 波の強さ
            static const float waveIntensity = 0.01;

            // 輝度に対応するRGB値を返す
            fixed4 GetLuminance(float luminance)
            {
                fixed4 col;
                col.x = 0.299 * luminance;
                col.y = 0.587 * luminance;
                col.z = 0.114 * luminance;
                col.a = 1;
                return col;
            }

            fixed2 random2(fixed2 st){
                st = fixed2( dot(st,fixed2(127.1,311.7)),
                               dot(st,fixed2(269.5,183.3)) );
                return -1.0 + 2.0*frac(sin(st)*43758.5453123);
            }

            float perlinNoise(fixed2 st) 
            {
                fixed2 p = floor(st);
                fixed2 f = frac(st);
                fixed2 u = f*f*(3.0-2.0*f);

                float v00 = random2(p+fixed2(0,0));
                float v10 = random2(p+fixed2(1,0));
                float v01 = random2(p+fixed2(0,1));
                float v11 = random2(p+fixed2(1,1));

                return lerp( lerp( dot( v00, f - fixed2(0,0) ), dot( v10, f - fixed2(1,0) ), u.x ),
                             lerp( dot( v01, f - fixed2(0,1) ), dot( v11, f - fixed2(1,1) ), u.x ), 
                             u.y)+0.5f;
            }

            float fBm (fixed2 st) 
            {
                float f = 0;
                fixed2 q = st;

                f += 0.5000*perlinNoise( q ); q = q*2.01;
                f += 0.2500*perlinNoise( q ); q = q*2.02;
                f += 0.1250*perlinNoise( q ); q = q*2.03;
                f += 0.0625*perlinNoise( q ); q = q*2.01;                

                return f;
            }
        
            fixed4 IceLayerFrag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                // 波の基となる関数
                float wave = cos(wave_one_frequency * UNITY_PI * (wave_one_angle.x * i.texcoord.x + wave_one_angle.y * i.texcoord.y) + (wave_one_velocity * _Time.y) + wave_initial_phase);
                float waveTwo = cos(wave_two_frequency * UNITY_PI * (wave_two_angle.x * i.texcoord.x + wave_two_angle.y * i.texcoord.y) + (wave_two_velocity * _Time.y) + wave_initial_phase);
                // ランダム値を生成
                float random = 20 * fBm(i.texcoord + 0.2 * _Time.y);
                // 輝きの強さを調整
                wave += waveTwo;
                wave += random;
                wave *= waveIntensity;
                // 輝度に変換してテクスチャに重ねる
                const fixed4 wave_color = GetLuminance(wave);
                col.r = clamp(col.x+wave_color.x, 0, 1);
                col.g = clamp(col.y+wave_color.y, 0, 1);
                col.b = clamp(col.z+wave_color.z, 0, 1);
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
        ENDCG
        }
    }
}