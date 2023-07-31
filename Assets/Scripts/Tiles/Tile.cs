using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Texture2D texture;
    public Color[] pixels;
    public Dictionary<TileBorderDirection, TileBorder> borders;
    public bool transparent = false;

    public Tile(Texture2D texture)
    {
        this.texture = texture;
        pixels = this.texture.GetPixels();
        FillBorders(TileBorder.Connector);
    }

    public Tile(Texture2D texture, TileBorder bordersType)
    {
        this.texture = texture;
        pixels = this.texture.GetPixels();
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
