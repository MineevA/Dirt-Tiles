using UnityEngine;

public class NavigationVertex 
{
    public Vector2 position;
    public NavigationVertex nextVertex;
    public NavigationVertex previousVertex;

    public NavigationVertex GetClosestVertexToPosition(Vector2 worldPosition)
    {
        if (nextVertex == null && previousVertex == null)
            return this;
        
        var previousDistance = -1f;
        var nextDistance = -1f;

        if (previousVertex != null)
            previousDistance = (worldPosition - previousVertex.position).magnitude;

        if (nextVertex != null)
            nextDistance = (worldPosition - nextVertex.position).magnitude;

        if (previousDistance > nextDistance)
            return nextVertex;
        
        return previousVertex;
    }
}
