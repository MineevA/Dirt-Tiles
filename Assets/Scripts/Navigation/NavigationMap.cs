using System;
using UnityEngine;

public class NavigationMap
{
    private int quadSideWidth;
    private Vector2 worldPosition;
    private NavigationQuad[,] quadMap;
    private NavigationVertex nullVertex;
    private NavigationQuad nullQuad;

    public NavigationMap(Vector2Int bounds, int width, Vector2 position)
    {
        quadMap = new NavigationQuad[bounds.x, bounds.y];
        quadSideWidth = width;
        worldPosition = position;

        InitNullObjects();
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

    private void InitNullObjects()
    {
        nullVertex = new NavigationVertex();
        nullVertex.position = -Vector2.one;
        nullQuad = new NavigationQuad(Vector2.zero, 0, nullVertex);
    }

    #region public_methods

    public NavigationVertex ClosestVertexToWorldPosition(Vector2 worldPosition)
    {
        var quad = FindQuadByWorldPosition(worldPosition);
        return quad.ClosestVertexToWorldPosition(worldPosition);
    }

    public Vector2 ClosestPointToWorldPosition(Vector2 worldPosition)
    {
        var edgeStart = ClosestVertexToWorldPosition(worldPosition);
        
        if (edgeStart.Equals(nullVertex))
            return worldPosition;

        var edgeEnd = edgeStart.ClosestVertexToPosition(worldPosition);

        if (edgeEnd == edgeStart)
            return edgeStart.position;

        var segment = edgeEnd.position - edgeStart.position;
        var pointVector = worldPosition - edgeStart.position;

        return edgeStart.position + VectorToVectorProjection(segment, pointVector);
    }
    
    public Vector3 ClosestPointToWorldPosition(Vector3 worldPosition)
    {
        var result = (Vector3)ClosestPointToWorldPosition((Vector2)worldPosition);
        result.z = worldPosition.z;

        return result;
    }

    public Vector3 ClosestPointToWorldPosition(Vector3 worldPosition, float stickDistance)
    {
        var result = ClosestPointToWorldPosition(worldPosition);

        if ((worldPosition - result).magnitude > stickDistance)
            return worldPosition;

        return result;
    }

    #endregion

    #region private_methods

    private NavigationQuad FindQuadByWorldPosition(Vector2 worldPosition)
    {
        var relativePosition = worldPosition - this.worldPosition;
        return FindQuadByRelativePosition(relativePosition);
    }

    private NavigationQuad FindQuadByRelativePosition(Vector2 relativePosition)
    {
        var quadX = (int)Math.Floor(relativePosition.x / quadSideWidth);
        var quadY = (int)Math.Floor(relativePosition.y / quadSideWidth);

        if (quadX < 0 || quadY < 0 || quadX > quadMap.GetUpperBound(0) || quadY > quadMap.GetUpperBound(1))
            return nullQuad;

        return quadMap[quadX, quadY];
    }

    private Vector2 VectorToVectorProjection(Vector2 main, Vector2 projected)
    {
        return main * (Vector2.Dot(main, projected) / main.sqrMagnitude);
    }

    #endregion
}
