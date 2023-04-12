using System;
using UnityEngine;

public class NavigationMap
{
    private int quadSideWidth;
    private Vector2 worldPosition;
    private NavigationQuad[,] quadMap;
    private NavigationVertex nullVertex;

    public NavigationMap(Vector2Int bounds, int quadSideWidth, Vector2 worldPosition)
    {
        quadMap = new NavigationQuad[bounds.x, bounds.y];
        this.quadSideWidth = quadSideWidth;
        this.worldPosition = worldPosition;

        nullVertex = new NavigationVertex();
        nullVertex.position = new Vector2(-1f,-1f);
    }

    public void FillCellMap()
    {
        for (int x = 0; x <= quadMap.GetUpperBound(0); x++)
            for (int y = 0; y <= quadMap.GetUpperBound(1); y++)
            {
                var position = worldPosition + new Vector2(x * quadSideWidth, y * quadSideWidth);
                quadMap[x, y] = new NavigationQuad(position, 4, nullVertex);
                quadMap[x, y].FillCellQuad(quadSideWidth);
            }
    }

    #region public_methods

    public NavigationVertex GetClosestVertexByWorldPosition(Vector2 worldPosition)
    {
        var quad = GetQuadByWorldPosition(worldPosition);
        return quad.GetClosestVertexByWorldPosition(worldPosition);
    }

    public NavigationVertex GetClosestVertexByUV(Vector2 uv)
    {
        var relativePosition = new Vector2(quadMap.GetUpperBound(0) * quadSideWidth * uv.x,
                                            quadMap.GetUpperBound(1) * quadSideWidth * uv.y);

        var quad = GetQuadByRelativePosition(relativePosition);

        return quad.GetClosestVertexByWorldPosition(relativePosition + worldPosition);
    }

    public Vector2 GetClosestPointOnMapInWorldPosition(Vector2 worldPosition)
    {
        var edgeStart = GetClosestVertexByWorldPosition(worldPosition);
        
        if (edgeStart.Equals(nullVertex))
            return worldPosition;

        var edgeEnd = edgeStart.GetClosestVertexToPosition(worldPosition);

        if (edgeEnd == edgeStart)
            return edgeStart.position;

        var segment = edgeEnd.position - edgeStart.position;
        var pointVector = worldPosition - edgeStart.position;

        return edgeStart.position + VectorToVectorProjection(segment, pointVector);
    }

    public Vector2 GetClosestPointOnMapInUV(Vector2 uv)
    {
        var worldPoint = GetClosestPointOnMapInWorldPosition(UVToWorldPoint(uv) + worldPosition);
        return WorldPointToMapUV(worldPoint - worldPosition);
    }

    #endregion

    #region private_methods

    private NavigationQuad GetQuadByWorldPosition(Vector2 worldPosition)
    {
        var relativePosition = worldPosition - this.worldPosition;
        return GetQuadByRelativePosition(relativePosition);
    }

    private NavigationQuad GetQuadByRelativePosition(Vector2 relativePosition)
    {
        var quadX = (int)Math.Floor(relativePosition.x / quadSideWidth);
        var quadY = (int)Math.Floor(relativePosition.y / quadSideWidth);

        return quadMap[quadX, quadY];
    }

    private Vector2 VectorToVectorProjection(Vector2 main, Vector2 projected)
    {
        return main * (Vector2.Dot(main, projected) / main.sqrMagnitude);
    }

    private Vector2 WorldPointToMapUV(Vector2 worldPoint)
    {
        return new Vector2(worldPoint.x / quadMap.GetUpperBound(0) * quadSideWidth,
                           worldPoint.y / quadMap.GetUpperBound(1) * quadSideWidth);
    }

    private Vector2 UVToWorldPoint(Vector2 uv)
    {
        return new Vector2(quadMap.GetUpperBound(0) * quadSideWidth * uv.x,
                           quadMap.GetUpperBound(1) * quadSideWidth * uv.y);
    }

    #endregion
}
