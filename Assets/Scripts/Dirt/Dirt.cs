using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(DirtCounter))]  
public class Dirt : MonoBehaviour
{
    public CustomRenderTexture alphaMap;
    public Material alphaMaterial;
    public DirtCounter dirtCounter;

    private Vector2 patternRelativeSize = new Vector2(0.08f,0.02f);

    private readonly int ErasePosition          = Shader.PropertyToID("_ErasePosition");
    private readonly int Pattern                = Shader.PropertyToID("_Pattern");
    private readonly int PatternRelativeSize    = Shader.PropertyToID("_PatternRelativeSize");
    private readonly int EraseLineSegment       = Shader.PropertyToID("_EraseLineSegment");
    private readonly int EraseLineSegmentLength = Shader.PropertyToID("_EraseLineSegmentLength");
    private readonly int LineSegmentArccos      = Shader.PropertyToID("_LineSegmentArccos");
    private readonly int SegmentCount           = Shader.PropertyToID("_SegmentCount");
    private readonly int PatternOverlapModifier = Shader.PropertyToID("_PatternOverlapModifier");
    
    private void Start()
    {
        alphaMap.Initialize();
        dirtCounter = GetComponent<DirtCounter>();
    }

    public void DrawPixels(Vector2 uv)
    {
        alphaMaterial.SetVector(ErasePosition, uv);
        alphaMaterial.SetVector(EraseLineSegment, Vector2.zero);
        alphaMaterial.SetFloat(EraseLineSegmentLength, .0f);
        alphaMaterial.SetFloat(LineSegmentArccos, .0f);
        alphaMaterial.SetInt(PatternOverlapModifier, 0);
        alphaMaterial.SetInt(SegmentCount, 0);

        dirtCounter.Recalculate();
    }

    public void DrawLine(Vector2 uvStart, Vector2 uvFinish)
    {
        var line = uvFinish - uvStart;
        var segmentVector = CalculateSegmentVector(line);
        var segmentCount = 0f;
        var patternOverlapModifier = 0;
        
        if (segmentVector.magnitude > 0)
            segmentCount = line.magnitude / segmentVector.magnitude;

        if (segmentVector.y < 0)
            patternOverlapModifier = 1;

        alphaMaterial.SetVector(ErasePosition, uvStart);
        alphaMaterial.SetVector(EraseLineSegment, segmentVector);
        alphaMaterial.SetFloat(EraseLineSegmentLength,  segmentVector.magnitude);
        alphaMaterial.SetFloat(LineSegmentArccos, segmentVector.magnitude / Mathf.Abs(segmentVector.y));
        alphaMaterial.SetInt(PatternOverlapModifier, patternOverlapModifier);
        alphaMaterial.SetInt(SegmentCount, (int)segmentCount);

        dirtCounter.Recalculate();
    }

    public void SetErasePattern(Texture2D pattern, Vector2 patternRelativeSize)
    {
        alphaMaterial.SetTexture(Pattern, pattern);
        alphaMaterial.SetVector(PatternRelativeSize, patternRelativeSize);
        
        this.patternRelativeSize = patternRelativeSize;
    }

    private Vector2 CalculateSegmentVector(Vector2 line)
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

