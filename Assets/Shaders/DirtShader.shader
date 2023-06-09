Shader "Unlit/DirtShader"
{
    Properties
    {
        _MainTex ("Dirt texture", 2D) = "white" {}
        _SolidDirt ("Solid dirt texture", 2D) = "white" {}
        _AlphaTex ("Alpha texture",2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off

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

            sampler2D _SolidDirt;
            float4 _SolidDirt_ST;

            sampler2D _AlphaTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                fixed4 solidColor = tex2D(_SolidDirt, i.uv);

                fixed4 alphaColor = tex2D(_AlphaTex, i.uv);

                color.a = min(alphaColor.r, color.a);
                solidColor.a = min(alphaColor.g, solidColor.a);

                float colorAlpha = color.a / (color.a + solidColor.a);
                    
                return fixed4(color.rgb * colorAlpha + solidColor.rgb * (1 - colorAlpha), color.a + solidColor.a);
            }
            ENDCG
        }
    }
}
