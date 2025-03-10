Shader "Test/Test_simple_color"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        [Toggle] _UseEffect("Enable Effect", Float) = 0
        [Enum(Red, 0, Green, 1, Blue, 2)] _ColorMode ("Color Mode", Float) = 0
        [PowerSlider(2.0)] _Shininess ("Shininess", Range(0,1)) = 0.5


        [Space(20)]
        [Header(Category name)]
        [KeywordEnum(None, Mode1, Mode2)] _EffectMode ("Effect Mode", Float) = 0
    }
    SubShader
    {
        Tags 
        { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma shader_feature _EFFECTMODE_NONE _EFFECTMODE_MODE1 _EFFECTMODE_MODE2


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;       // 색상 변수 연결

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                #if defined(_EFFECTMODE_MODE1)
                    _Color.rg *= 2.0; // Mode1 적용
                #elif defined(_EFFECTMODE_MODE2)
                    _Color.b *= 2.0; // Mode2 적용
                #endif

                return col * _Color;
            }
            ENDCG
        }
    }
}
