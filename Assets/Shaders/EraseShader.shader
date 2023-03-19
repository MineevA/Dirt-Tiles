Shader "EraserShader"
{
    Properties
    {
        _ErasePosition("ErasePosition", Vector) = (-1, -1, 0, 0)
        _ErasePattern("ErasePattern",2D) = "white" {}
        _ErasePatternRelativeSize("PatternRelativeSize", Vector) = (0, 0, 0, 0)
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
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4 _ErasePosition;
            sampler2D _ErasePattern;
            float4 _ErasePatternRelativeSize;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                float4 color = tex2D(_SelfTexture2D, IN.localTexcoord.xy);
                float2 distance = IN.localTexcoord.xy - _ErasePosition.xy;
                
                float multyplier = step(0, distance.x) 
                                   * step(0, distance.y)
                                   * step(-_ErasePatternRelativeSize.x, -distance.x)
                                   * step(-_ErasePatternRelativeSize.y, -distance.y);

                float2 patternUV;
                patternUV.x = distance.x * multyplier / _ErasePatternRelativeSize.x;
                patternUV.y = distance.y * multyplier / _ErasePatternRelativeSize.y;
                
                float4 drawAlpha = tex2D(_ErasePattern, patternUV);

                color.r = min(drawAlpha.r,color.r);
                color.g = min(drawAlpha.g,color.g);
                color.b = min(drawAlpha.b,color.b);

                return color;
            }
            ENDCG   
        }
    }
}