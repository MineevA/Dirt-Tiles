using UnityEngine;

public class TileRenderer
{
    private Texture2D tempDirt;
    private Texture2D tempSolidDirt;
    private Texture2D tempBackground;

    private Color32[] dirtPixels;
    private Color32[] solidDirtPixels;
    
    public TileRenderer(Texture2D dirt, Texture2D solidDirt, Vector2Int backgroundBounds)
    {
        tempDirt = new Texture2D(dirt.width, dirt.height);
        tempSolidDirt = new Texture2D(solidDirt.width, solidDirt.height);
        tempBackground = new Texture2D(backgroundBounds.x, backgroundBounds.y);

        dirtPixels = dirt.GetPixels32();
        solidDirtPixels = solidDirt.GetPixels32();
    }

    public void Render(TileMap tileMap, 
                       Dirt dirt, 
                       int solidDirtThickness,
                       TilesBackground tilesBackground)
    {
        SetBackgroundFromTileMap(tileMap, dirt, tilesBackground);
        SetDirtFromTileMap(tileMap, dirt);
        SetSolidDirtFromTileMap(tileMap, dirt, solidDirtThickness);
    }

    private void SetDirtFromTileMap(TileMap tileMap, Dirt dirt)
    {
        tempDirt.SetPixels32(dirtPixels);

        for (int x = 0; x < tileMap.width; x++)
            for (int y = 0; y < tileMap.height; y++)
                if (tileMap.Tile(x, y).transparent)
                    tempDirt.SetPixels((x + 1) * tileMap.tileSize,
                                                 (y + 1) * tileMap.tileSize,
                                                 tileMap.tileSize,
                                                 tileMap.tileSize,
                                                 tileMap.Tile(x, y).pixels);

        tempDirt.Apply();

        dirt.SetDirtTexture(tempDirt);
    }

    private void SetSolidDirtFromTileMap(TileMap tileMap, 
                                         Dirt dirt, 
                                         int solidDirtThickness)
    {
        tempSolidDirt.SetPixels32(solidDirtPixels);

        var cleanPixels = new Color[solidDirtThickness * tileMap.tileSize];
        for (int i = 0; i < cleanPixels.Length; i++)
            cleanPixels[i] = new Color(0f, 0f, 0f, 0f);

        for (int x = 0; x < tileMap.width; x++)
            for (int y = 0; y < tileMap.height; y++)
                foreach (var border in tileMap.Tile(x, y).borders)
                    if (border.Value == TileBorder.Connector || tileMap.Tile(x, y).transparent)
                        SetSolidDirtPixels(ref tempSolidDirt,
                                            (x + 1) * tileMap.tileSize,
                                            (y + 1) * tileMap.tileSize,
                                            cleanPixels,
                                            border.Key,
                                            solidDirtThickness,
                                            tileMap.tileSize);


        tempSolidDirt.Apply();

        dirt.SetSolidDirtTexture(tempSolidDirt);
    }

    private void SetBackgroundFromTileMap(TileMap tileMap, Dirt dirt, TilesBackground tilesBackground)
    {
        for (int x = 0; x < tileMap.width; x++)
            for (int y = 0; y < tileMap.height; y++)
            {
                tempBackground.SetPixels(x * tileMap.tileSize,
                                 y * tileMap.tileSize,
                                 tileMap.tileSize,
                                 tileMap.tileSize,
                                 tileMap.Tile(x, y).pixels);
            }

        tempBackground.Apply();

        tilesBackground.SetTexture(tempBackground);
    }

    private void SetSolidDirtPixels(ref Texture2D texture, 
                                    int x, 
                                    int y, 
                                    Color[] cleanPixels, 
                                    TileBorderDirection direction,
                                    int solidDirtThickness,
                                    int tileSizeInPixels)
    {
        switch (direction)
        {
            case TileBorderDirection.Left:
                texture.SetPixels(x, y, solidDirtThickness, tileSizeInPixels, cleanPixels);
                break;
            case TileBorderDirection.Bottom:
                texture.SetPixels(x, y, tileSizeInPixels, solidDirtThickness, cleanPixels);
                break;
            case TileBorderDirection.Top:
                texture.SetPixels(x, y + tileSizeInPixels - solidDirtThickness, tileSizeInPixels, solidDirtThickness, cleanPixels);
                break;
            case TileBorderDirection.Right:
                texture.SetPixels(x + tileSizeInPixels - solidDirtThickness, y, solidDirtThickness, tileSizeInPixels, cleanPixels);
                break;
            default:
                break;
        }
    }
}
