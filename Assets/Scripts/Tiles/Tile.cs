using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Texture2D texture;
    public Dictionary<TileBorderDirection, TileBorder> borders;

    public Tile(Texture2D texture)
    {
        this.texture = texture;

        borders = new Dictionary<TileBorderDirection, TileBorder>();
        borders.Add(TileBorderDirection.Left, TileBorder.Connector);
        borders.Add(TileBorderDirection.Top, TileBorder.Connector);
        borders.Add(TileBorderDirection.Right, TileBorder.Connector);
        borders.Add(TileBorderDirection.Bottom, TileBorder.Connector);
    }
}
