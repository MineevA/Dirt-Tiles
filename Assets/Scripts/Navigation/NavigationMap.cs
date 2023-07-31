using System;
using UnityEngine;

public class NavigationMap
{
    private Vector2 worldPosition;
    private NavigationVertex[,] vertices;

    public NavigationMap(Vector2Int bounds, Vector2 position)
    {
        worldPosition = position;
        InitVertices(bounds);
    }

    private void InitVertices(Vector2Int bounds)
    {
        vertices = new NavigationVertex[bounds.x, bounds.y];
        
        for (int x = 0; x < bounds.x; x++)
            for (int y = 0; y < bounds.y; y++)
                vertices[x, y] = new NavigationVertex(x, y);
    }

    #region Fill_map

    public void FillMap(Tile[,] tileMap)
    {
        ClearVertices();
        
        for (int x = 0; x <= tileMap.GetUpperBound(0); x++)
            for (int y = 0; y <= tileMap.GetUpperBound(1); y++)
            {
                if (tileMap[x, y].transparent)
                    continue;

                var tile = tileMap[x, y];

                foreach (var border in tile.borders)
                    if (border.Value.Equals(TileBorder.Edge))
                        SetVerticesByBorderDirection(x, y, border.Key);
            }
    }

    private void ClearVertices()
    {
        foreach (var vertex in vertices)
        {
            vertex.active = false;
            vertex.InitNeighbors();
        }
    }

    private void SetVerticesByBorderDirection(int x, int y, TileBorderDirection direction)
    {
        switch (direction)
        {
            case TileBorderDirection.Left:
                LinkEdgeVertices(vertices[x, y], vertices[x, y + 1]);
                break;
            case TileBorderDirection.Top:
                LinkEdgeVertices(vertices[x, y + 1], vertices[x + 1, y + 1]);
                break;
            case TileBorderDirection.Right:
                LinkEdgeVertices(vertices[x + 1, y + 1], vertices[x + 1, y]);
                break;
            case TileBorderDirection.Bottom:
                LinkEdgeVertices(vertices[x, y], vertices[x + 1, y]);
                break;
            default:
                break;
        }
    }

    private void LinkEdgeVertices(NavigationVertex vertex1, NavigationVertex vertex2)
    {
        vertex1.active = true;
        vertex2.active = true;

        vertex1.AddNeighbor(vertex2);
        vertex2.AddNeighbor(vertex1);
    }

    #endregion

    #region Navigation

    public Vector3 GetClosestPosition(Vector3 worldPosition)
    {
        var relativePosition = (Vector2)worldPosition - this.worldPosition;
        
        var closestVertexX = (int)Math.Round(relativePosition.x);
        var closestVertexY = (int)Math.Round(relativePosition.y);

        if (!CoordinatesInBounds(closestVertexX, closestVertexY))
            return worldPosition;

        var edgeStart = vertices[closestVertexX, closestVertexY];
        if (!edgeStart.active)
            return worldPosition;
        
        var edgeEnd = edgeStart;
        var closestDistance = -1f;

        foreach(var vertex in edgeStart.neighbors)
        {
            if (vertex == null || !vertex.active)
                continue;

            var distanceToPosition = (relativePosition - vertex.position).magnitude;
            if (distanceToPosition < closestDistance || closestDistance == -1f)
            {
                closestDistance = distanceToPosition;
                edgeEnd = vertex;
            }
        }

        var edgeVector = edgeEnd.position - edgeStart.position;
        var relativeToEdgeStart = relativePosition - edgeStart.position;
        
        var closest = VectorToVectorProjection(edgeVector, relativeToEdgeStart) + edgeStart.position + this.worldPosition;

        return new Vector3(closest.x, closest.y, worldPosition.z);
    }

    public bool CoordinatesInBounds(int x, int y)
    {
        if (x < 0 || y < 0 || x > vertices.GetUpperBound(0) || y > vertices.GetUpperBound(1)) 
            return false;

        return true;
    }

    private Vector2 VectorToVectorProjection(Vector2 main, Vector2 projected)
    {
        return main * (Vector2.Dot(main, projected) / main.sqrMagnitude);
    }

    #endregion

    #region debug

    public void DebugDraw()
    {
        foreach (var vertex in vertices)
            vertex.debugDrawn = false;
        
        foreach(var vertex in vertices)
        {
            if (!vertex.active)
                continue;

            foreach(var neighbor in vertex.neighbors)
            {
                if (neighbor == null || !neighbor.active)
                    continue;

                Debug.DrawLine(vertex.position + worldPosition, neighbor.position + worldPosition,Color.green,3);
            }
        }
    }

    #endregion
}
