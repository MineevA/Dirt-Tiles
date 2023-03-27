Shader "EraserShader"
{
    Properties
    {
        _ErasePosition("Erase position", Vector) = (-1, -1, 0, 0)
        _Pattern("Erase pattern",2D) = "white" {}
        _PatternRelativeSize("Pattern relative size", Vector) = (0, 0, 0, 0)
        _EraseLineSegment("Erase line segment", Vector) = (0, 0, 0, 0)
        _EraseLineSegmentLength("Erase line segment length", float) = .0
        _LineSegmentArccos("Line segment arccos", float) = .0
        _PatternOverlapModifier("Pattern overlap modifier", int) = 0
        _SegmentCount("Segment count", int) = 0
        _Test("Test", int) = 0
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

            float4 _ErasePosition;
            float4 _PatternRelativeSize;
            float4 _EraseLineSegment;
            float _EraseLineSegmentLength;
            float _LineSegmentArccos;
            int _SegmentCount;
            int _PatternOverlapModifier;
            int _Test;
            sampler2D _Pattern;
            
            float GetColorFromPattern(float2 uv, float2 patternStartUV)
            {
                float2 distance = uv - patternStartUV;
                
                float multyplier = step(0, distance.x) 
                    * step(0, distance.y)
                    * step(distance.x,_PatternRelativeSize.x)
                    * step(distance.y,_PatternRelativeSize.y);

                distance.x = distance.x * multyplier / _PatternRelativeSize.x;
                distance.y = distance.y * multyplier / _PatternRelativeSize.y;
               
                return tex2D(_Pattern, distance).r;
            }

            float4 GetPatternStartPositions(float2 uv)
            {
                float4 patternStartPositions = _ErasePosition.xyxy;
                
                if (_SegmentCount == 0)
                    return patternStartPositions;
                
                float2 distanceVec = uv - _ErasePosition;
                float relativeLength = abs(distanceVec.y) * _LineSegmentArccos;

                int currentSegment = (int) (relativeLength / _EraseLineSegmentLength) + _PatternOverlapModifier;
                currentSegment = min(currentSegment, _SegmentCount);

                patternStartPositions.rg = _ErasePosition.xy + _EraseLineSegment.xy * currentSegment;
                patternStartPositions.ba = _ErasePosition.xy + _EraseLineSegment.xy * (currentSegment - sign(_EraseLineSegment.y));

                return patternStartPositions;
            }

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                float4 color = tex2D(_SelfTexture2D, IN.localTexcoord.xy);
                float4 patternStartPositions = GetPatternStartPositions(IN.localTexcoord.xy); 
                _Test += step(0, color.r);
                
                color.r = min(GetColorFromPattern(IN.localTexcoord.xy, patternStartPositions.rg),color.r);
                color.r = min(GetColorFromPattern(IN.localTexcoord.xy, patternStartPositions.ba),color.r);

                return color;
            }

            ENDCG   
        }
    }
}