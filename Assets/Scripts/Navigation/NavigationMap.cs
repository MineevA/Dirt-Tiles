using System;
using UnityEngine;

public class NavigationMap
{
    public int quadSideWidth;
    public NavigationQuad[,] quadMap;

    private NavigationVertex nullVertex;

    public NavigationMap(Vector2Int bounds, int quadSideWidth)
    {
        quadMap = new NavigationQuad[bounds.x, bounds.y];
        this.quadSideWidth = quadSideWidth;

        nullVertex = new NavigationVertex();
        nullVertex.position = new Vector2(-1f,-1f);
    }

    public void FillCellMap()
    {
        for (int x = 0; x <= quadMap.GetUpperBound(0); x++)
            for (int y = 0; y <= quadMap.GetUpperBound(1); y++)
            {
                var position = new Vector2(x * quadSideWidth, y * quadSideWidth);
                quadMap[x, y] = new NavigationQuad(position, 4, nullVertex);
                quadMap[x, y].FillCellQuad(quadSideWidth);
            }
    }

    public NavigationVertex GetClosestVertexByWorldPosition(Vector2 worldPosition)
    {

        var quad = GetQuadByWorldPosition(worldPosition);
        return quad.GetClosestVertexByWorldPosition(worldPosition);
    }

    public NavigationVertex GetClosestVertexByUV(Vector2 uv)
    {
        var worldLength = quadMap.GetUpperBound(0) * quadSideWidth;
        var worldHeight = quadMap.GetUpperBound(1) * quadSideWidth;

        return GetClosestVertexByWorldPosition(new Vector2(worldLength * uv.x, worldHeight * uv.y));
    }
    
    private NavigationQuad GetQuadByWorldPosition(Vector2 worldPosition)
    {
        var quadX = (int)Math.Ceiling(worldPosition.x / quadSideWidth);
        var quadY = (int)Math.Ceiling(worldPosition.y / quadSideWidth);
        
        return quadMap[quadX, quadY];
    }
}
