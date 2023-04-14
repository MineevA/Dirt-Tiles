using UnityEngine;

public class NavigationQuad
{
    public Vector2 position;
    public NavigationVertex[] vertices;
    public NavigationVertex nullVertex;

    public NavigationQuad(Vector2 coordinates, int verticesCount, NavigationVertex nullVertex)
    {
        this.nullVertex = nullVertex;
        position = coordinates;
        vertices = new NavigationVertex[verticesCount];
    }

    public void FillCellQuad(int quadSideWidth)
    {
        for(int i = 0; i < vertices.Length; i++)
            vertices[i] = new NavigationVertex();

        for (int i = 0; i < vertices.Length; i++)
        {
            var previousIndex = i - 1;
            if (previousIndex < 0)
                previousIndex = vertices.Length - 1;

            var nextIndex = i + 1;
            if (nextIndex == vertices.Length)
                nextIndex = 0;

            vertices[i].previousVertex = vertices[previousIndex];
            vertices[i].nextVertex = vertices[nextIndex];
        }

        //Пересчитать в циклке через поворот вектора (quadSideWidth,0) на угол 360 * i / vertices.Length
        //можно делать любые симметричные плитки, не только квадратные
        vertices[0].position = position;
        vertices[1].position = position + new Vector2(0,quadSideWidth);
        vertices[2].position = position + new Vector2(quadSideWidth,quadSideWidth);
        vertices[3].position = position + new Vector2(quadSideWidth, 0);
    }

    public NavigationVertex ClosestVertexToWorldPosition(Vector2 worldPosition)
    {
        var closestVertex = nullVertex;
        var closestVertexDistance = -1f;

        foreach (var vert in vertices)
        {
            var distance = vert.position - worldPosition;
            if (distance.magnitude < closestVertexDistance || closestVertexDistance == -1f)
            {
                closestVertexDistance = distance.magnitude;
                closestVertex = vert;
            }
        }

        return closestVertex;
    }
}
