using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Texture2D texture;
    public Dictionary<TileBorderDirection, TileBorder> borders;

    public Tile(Texture2D texture)
    {
        this.texture = texture;
        FillBorders(TileBorder.Connector);
    }

    public Tile(Texture2D texture, TileBorder bordersType)
    {
        this.texture = texture;
        FillBorders(bordersType);       
    }

    private void FillBorders(TileBorder bordersType)
    {
        borders = new Dictionary<TileBorderDirection, TileBorder>
        {
            { TileBorderDirection.Left, bordersType },
            { TileBorderDirection.Top, bordersType },
            { TileBorderDirection.Right, bordersType },
            { TileBorderDirection.Bottom, bordersType }
        };
    }
}
