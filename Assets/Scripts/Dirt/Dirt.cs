using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(DirtCounter))]  
public class Dirt : MonoBehaviour
{
    public CustomRenderTexture  alphaMap;
    public Material             alphaMaterial;
    public DirtCounter          dirtCounter;
    public NavigationMap        navigationMap;

    private Material dirtMaterial;

    private readonly int ErasePosition          = Shader.PropertyToID("_ErasePosition");
    private readonly int PatternRelativeSize    = Shader.PropertyToID("_PatternRelativeSize");
    private readonly int EraseLineSegment       = Shader.PropertyToID("_EraseLineSegment");
    private readonly int EraseLineSegmentLength = Shader.PropertyToID("_EraseLineSegmentLength");
    private readonly int LineSegmentArccos      = Shader.PropertyToID("_LineSegmentArccos");
    private readonly int LineSegmentArcsin      = Shader.PropertyToID("_LineSegmentArcsin");
    private readonly int SegmentCount           = Shader.PropertyToID("_SegmentCount");
    private readonly int PatternOverlapModifier = Shader.PropertyToID("_PatternOverlapModifier");
    private readonly int SolidDirtModifier      = Shader.PropertyToID("_SolidDirtModifier");
    
    private void Start()
    {
        navigationMap = new NavigationMap(new Vector2Int(8,10),
                                          1,
                                          new Vector2(-4,-5));
        navigationMap.FillCellMap();
        SetDefaults();
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
        var segmentArccos = 0f;
        var segmentArcsin = 0f;
        var patternOverlapModifier = 0;
        
        if (segmentVector.magnitude > 0)
            segmentCount = line.magnitude / segmentVector.magnitude;

        if (segmentVector.y < 0 || segmentVector.y == 0 && segmentVector.x < 0)
            patternOverlapModifier = 1;

        if (segmentVector.y != 0)
            segmentArccos = segmentVector.magnitude / Mathf.Abs(segmentVector.y);

        if (segmentVector.x != 0)
            segmentArcsin = segmentVector.magnitude / Math.Abs(segmentVector.x);

        alphaMaterial.SetVector(ErasePosition, uvStart);
        alphaMaterial.SetVector(EraseLineSegment, segmentVector);
        alphaMaterial.SetVector(PatternRelativeSize, patternRelativeSize);
        alphaMaterial.SetFloat(EraseLineSegmentLength,  segmentVector.magnitude);
        alphaMaterial.SetFloat(LineSegmentArccos, segmentArccos);
        alphaMaterial.SetFloat(LineSegmentArcsin, segmentArcsin);
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
            if (Mathf.Abs(x) <= Mathf.Abs(rectX) && Mathf.Abs(rectY) > 0.00f)
                return new Vector2(x, rectY);
        }

        if (line.x != 0)
        {
            var y = line.y * rectX / line.x;
            if (Mathf.Abs(y) <= Mathf.Abs(rectY) && Mathf.Abs(rectX) > 0.00f)
                return new Vector2(rectX, 0f);
        }

        return Vector2.zero;
    }

    public void SetDefaults()
    {
        InitAlphaMaterial();
        InitCounter();
    }

    private void InitAlphaMaterial()
    {
        alphaMaterial.SetVector(ErasePosition, Vector2.left);
        alphaMaterial.SetVector(EraseLineSegment, Vector2.zero);
        alphaMaterial.SetInt(SegmentCount, 0);

        alphaMap.Initialize();
    }

    private void InitCounter()
    {
        if (dirtCounter == null)
            dirtCounter = GetComponent<DirtCounter>();

        dirtCounter.InitCounter();
    }

    public void SetDirtTexture(Texture2D texture)
    {
        if (dirtMaterial == null)
            dirtMaterial = GetComponent<MeshRenderer>().material;

        dirtMaterial.SetTexture("_MainTex", texture);
    }

} 

