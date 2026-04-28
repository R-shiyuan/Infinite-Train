Shader "Custom/ScrollingBackgroundProportional"
{
    Properties
    {
        _MainTex ("背景图", 2D) = "white" {}
        _ScrollSpeed ("滚动速度 (X轴)", Range(-2, 2)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "Canvas"="Overlay"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

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
            float _ScrollSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                // 获取纹理尺寸和UI尺寸
                float texWidth = _MainTex_ST.x;
                float texHeight = _MainTex_ST.y;
                float uiWidth = 1;
                float uiHeight = 1;

                // 计算宽高比
                float texAspect = texWidth / texHeight;
                float uiAspect = uiWidth / uiHeight;

                // 计算缩放系数，保持纹理比例不变
                float2 scale = float2(1, 1);
                if (texAspect > uiAspect)
                {
                    // 纹理更宽，在Y轴缩放
                    scale.y = texAspect / uiAspect;
                }
                else
                {
                    // 纹理更高，在X轴缩放
                    scale.x = uiAspect / texAspect;
                }

                // 应用缩放，再进行UV偏移
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) * scale;
                float scrollOffset = _Time.y * _ScrollSpeed;
                o.uv.x = frac(o.uv.x + scrollOffset);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}