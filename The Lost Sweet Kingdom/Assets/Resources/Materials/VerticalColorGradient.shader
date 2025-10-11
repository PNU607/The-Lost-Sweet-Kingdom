Shader "Custom/Particles/VerticalColorGradient"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _TopColor ("Top Color", Color) = (1,0,0,1)
        _BottomColor ("Bottom Color", Color) = (0,0,1,1)

        _TopPower ("Top Blend Power", Range(0.1,5.0)) = 1.0
        _BottomPower ("Bottom Blend Power", Range(0.1,5.0)) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        ZWrite Off
        Cull Off
        Blend SrcAlpha One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _TopColor;
            float4 _BottomColor;
            float _TopPower;
            float _BottomPower;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = saturate(v.color) * _Color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                float uvY = i.uv.y;

                fixed3 gradColor;

                if (uvY >= 0.5)
                {
                    // 위쪽 영역: 위 → 중앙 (0~1)
                    float t = saturate((uvY - 0.5) * 2.0);
                    t = pow(t, _TopPower);
                    gradColor = lerp(fixed3(1,1,1), _TopColor.rgb, t);
                }
                else
                {
                    // 아래쪽 영역: 아래 → 중앙 (0~1)
                    float t = saturate((0.5 - uvY) * 2.0);
                    t = pow(t, _BottomPower);
                    gradColor = lerp(fixed3(1,1,1), _BottomColor.rgb, t);
                }

                fixed4 col;
                col.rgb = tex.rgb * gradColor * i.color.rgb;
                col.a = tex.a * i.color.a;

                return col;
            }
            ENDCG
        }
    }

    FallBack "Particles/Alpha Blended"
}
