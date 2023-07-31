using UnityEngine;

public class NavigationVertex 
{
    public readonly Vector2Int position;

    public bool active;
    public NavigationVertex[] neighbors;

    public bool debugDrawn;

    private int neighborCount = 0;
    
    public NavigationVertex(int x, int y)
    {
        position = new Vector2Int(x, y);
        InitNeighbors();
    }

    public void InitNeighbors()
    {
        neighbors = new NavigationVertex[4];
        neighborCount = 0;
    }

    public void AddNeighbor(NavigationVertex vertex)
    {
        foreach(var neighbor in neighbors)
            if (neighbor != null && neighbor.Equals(vertex))
                return;

        neighbors[neighborCount] = vertex;
        neighborCount++;
    }

    public void SetNeighboors(NavigationVertex[,] map, Vector2Int[] directions)
    {
        var neighborsCount = 0;
        
        foreach(var direction in directions)
        { 
            var neighborX = direction.x + position.x;
            var neighborY = direction.y + position.y;

            if (neighborX < 0 || neighborX > map.GetUpperBound(0) || neighborY < 0 || neighborY > map.GetUpperBound(1))
                continue;

            neighbors[neighborsCount] = map[neighborX, neighborY];
            neighborsCount++;
        }
    }


}
