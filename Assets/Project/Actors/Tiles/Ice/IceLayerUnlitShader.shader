Shader "IceLayerUnlitShader"
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

            // 輝きの頻度
            static const float shine_frequency = 0.60;
            // 輝きの初期位相
            static const float shine_initial_phase = UNITY_PI;
            // 輝きの閾値
            static const float shine_min_threshold = 0.90;
            // 輝きの強さ
            static const float shine_max_value = 0.85;
        
            fixed4 IceLayerFrag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                // 輝きの傾き
                const float angle = (40 * i.texcoord.x - 60 * i.texcoord.y) / 100;
                // 輝きの速さ
                const float speed = -3 * _Time.y;
                // 輝きの基となる関数
                fixed4 shine = cos(shine_frequency * UNITY_PI * angle + speed + shine_initial_phase);
                // 強度が弱い部分を0にする
                shine -= shine_min_threshold;
                shine = max(shine, 0);
                shine /= (1 - shine_min_threshold);
                // 輝きの強さを調整
                shine *= shine_max_value;
                // 輝度に変換してテクスチャに重ねる
                shine.r = clamp(col.x+0.299*shine.x, 0, 1);
                shine.g = clamp(col.y+0.587*shine.y, 0, 1);
                shine.b = clamp(col.z+0.114*shine.z, 0, 1);
                col.rgb = shine;
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
        ENDCG
        }
    }
}
