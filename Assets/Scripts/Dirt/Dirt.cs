using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(DirtCounter))]  
public class Dirt : MonoBehaviour
{
    public CustomRenderTexture alphaMap;
    public Material alphaMaterial;
    public DirtCounter dirtCounter;

    private readonly int ErasePosition          = Shader.PropertyToID("_ErasePosition");
    private readonly int Pattern                = Shader.PropertyToID("_Pattern");
    private readonly int PatternRelativeSize    = Shader.PropertyToID("_PatternRelativeSize");
    private readonly int EraseLineSegment       = Shader.PropertyToID("_EraseLineSegment");
    private readonly int EraseLineSegmentLength = Shader.PropertyToID("_EraseLineSegmentLength");
    private readonly int LineSegmentArccos      = Shader.PropertyToID("_LineSegmentArccos");
    private readonly int SegmentCount           = Shader.PropertyToID("_SegmentCount");
    private readonly int PatternOverlapModifier = Shader.PropertyToID("_PatternOverlapModifier");
    private readonly int SolidDirtModifier      = Shader.PropertyToID("_SolidDirtModifier");
    
    private void Start()
    {
        alphaMap.Initialize();
        dirtCounter = GetComponent<DirtCounter>();
    }

    public void DrawPixels(Vector2 uv, 
                           Vector2 patternRelativeSize,
                           float solidDirtModifier)
    {
        alphaMaterial.SetVector(ErasePosition, uv);
        alphaMaterial.SetVector(EraseLineSegment, Vector2.zero);
        alphaMaterial.SetFloat(EraseLineSegmentLength, .0f);
        alphaMaterial.SetFloat(LineSegmentArccos, .0f);
        alphaMaterial.SetFloat(SolidDirtModifier, solidDirtModifier);
        alphaMaterial.SetInt(PatternOverlapModifier, 0);
        alphaMaterial.SetInt(SegmentCount, 0);
        alphaMaterial.SetVector(PatternRelativeSize, patternRelativeSize);
        
        dirtCounter.Recalculate();
    }

    public void DrawLine(Vector2 uvStart, 
                         Vector2 uvFinish, 
                         Vector2 patternRelativeSize,
                         float solidDirtModifier)
    {
        var line = uvFinish - uvStart;
        var segmentVector = CalculateSegmentVector(line, patternRelativeSize);
        var segmentCount = 0f;
        var patternOverlapModifier = 0;
        
        if (segmentVector.magnitude > 0)
            segmentCount = line.magnitude / segmentVector.magnitude;

        if (segmentVector.y < 0)
            patternOverlapModifier = 1;

        alphaMaterial.SetVector(ErasePosition, uvStart);
        alphaMaterial.SetVector(EraseLineSegment, segmentVector);
        alphaMaterial.SetVector(PatternRelativeSize, patternRelativeSize);
        alphaMaterial.SetFloat(EraseLineSegmentLength,  segmentVector.magnitude);
        alphaMaterial.SetFloat(LineSegmentArccos, segmentVector.magnitude / Mathf.Abs(segmentVector.y));
        alphaMaterial.SetFloat(SolidDirtModifier, solidDirtModifier);
        alphaMaterial.SetInt(PatternOverlapModifier, patternOverlapModifier);
        alphaMaterial.SetInt(SegmentCount, (int)segmentCount);

        dirtCounter.Recalculate();
    }

    private Vector2 CalculateSegmentVector(Vector2 line, Vector2 patternRelativeSize)
    {
        var rectX = Mathf.Sign(line.x) * patternRelativeSize.x / 2;
        var rectY = Mathf.Sign(line.y) * patternRelativeSize.y / 2;
       
        if (line.y != 0)
        {
            var x = line.x * rectY / line.y;
            if (Mathf.Abs(x) <= Mathf.Abs(rectX))
                return new Vector2(x, rectY);
        }

        if (line.x != 0)
        {
            var y = line.y * rectX / line.x;
            if (Mathf.Abs(y) <= Mathf.Abs(rectY))
                return new Vector2(rectX, y);
        }

        return Vector2.zero;
    }

} 

