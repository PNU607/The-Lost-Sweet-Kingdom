Shader "Unlit/FanShapedShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Center ("Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Max Radius", Float) = 1
        _Progress ("Progress (0~1)", Float) = 0
        _Angle ("Angle (Deg)", Float) = 60
        _Softness ("Edge Softness", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;
            float2 _Center;
            float _Radius;
            float _Progress;
            float _Angle;
            float _Softness;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float remap(float value, float inMin, float inMax, float outMin, float outMax)
            {
                return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float2 dir = uv - _Center;
                float dist = length(dir);

                // ���� ���� ���
                float angle = degrees(atan2(dir.y, dir.x));

                // angle ����ȭ: -180 ~ 180 �� 0 ~ 180
                float absAngle = abs(angle);

                // ��ä�� ���� Ȯ��
                float angleMask = step(absAngle, _Angle * 0.5);

                // ���൵ ��� �Ÿ� ���
                float waveRadius = _Radius * _Progress;

                // �ε巯�� �����ڸ�
                float edge = smoothstep(waveRadius, waveRadius - _Softness, dist);

                // ���� ����ũ ����
                float final = edge * angleMask;

                float4 tex = tex2D(_MainTex, uv);
                return tex * _Color * final;
            }
            ENDCG
        }
    }
}
