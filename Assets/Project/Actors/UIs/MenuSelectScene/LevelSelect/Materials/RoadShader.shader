Shader "Unlit/RoadShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _UnreleasedColor ("Unreleased Color", Color) = (0.2,0.2,0.7,1)
        [PerRendererData] _fillAmount ("Fill Amount", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _fillAmount;
            
            sampler2D _MainTex;
            half4 _UnreleasedColor;

            fixed4 frag (v2f_img i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // if (i.uv.x < _fillAmount) {
                //     return col;
                // }
                // return col * _UnreleasedColor;
                // 高速版
                fixed4 unreleased_color = (1 - step(_fillAmount, i.uv.x)) + step(_fillAmount, i.uv.x) * _UnreleasedColor;
                return col * unreleased_color;
            }
            ENDCG
        }
    }
}
