using UnityEngine;

public class NavigationVertex 
{
    public Vector2 position;
    public NavigationVertex nextVertex;
    public NavigationVertex previousVertex;

    public NavigationVertex ClosestVertexToPosition(Vector2 worldPosition)
    {
        if (nextVertex == null && previousVertex == null)
            return this;
        
        var previousDistance = DistanceToPosition(previousVertex, worldPosition);
        var nextDistance = DistanceToPosition(nextVertex, worldPosition);

        if (previousDistance > nextDistance)
            return nextVertex;
        
        return previousVertex;
    }

    private float DistanceToPosition(NavigationVertex vertex, Vector2 position)
    {
        var distance = -1f;

        if (vertex == null)
            return distance;

        return (position - vertex.position).magnitude;
    }
}
