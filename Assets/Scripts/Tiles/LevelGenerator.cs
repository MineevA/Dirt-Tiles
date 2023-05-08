using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using WFC;

public class LevelGenerator : MonoBehaviour
{
    public Texture2D[]      figures;
    public Texture2D        dirtTexture;
    public Texture2D        solidTexture;

    public int              tileSizeInPixels;
    public int              solidDirtThickness;
    public Vector2Int       tileMapSize;
    public Dirt             dirt;
    public SpriteRenderer   backgroundSpriteRenderer;

    private Generator<Tile> generator;
    private Tile[]          tilesLib;
    private Tile            transparentTile;

    void Start()
    {
        transparentTile = TransparentTile();
        tilesLib = ParseFigures();

        generator = new Generator<Tile>(tilesLib, GeneratorStateChangeHandler);
        backgroundSpriteRenderer = GetComponent<SpriteRenderer>();

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        var tileMap = generator.Generate(tileMapSize.x, tileMapSize.y);
        SetBackgroundFromTileMap(tileMap);
        SetDirtFromTileMap(tileMap);
        SetSolidDirtFromTileMap(tileMap);

        dirt.SetDefaults();
    }

    private Tile TransparentTile()
    {
        var texture = new Texture2D(tileSizeInPixels, tileSizeInPixels);
        for (int x = 0; x < texture.height; x++)
            for (int y = 0; y < texture.width; y++)
                texture.SetPixel(x, y, Color.clear);

        var tile = new Tile(texture, TileBorder.Edge);
        return tile;
    }

    private void SetDirtFromTileMap(Tile[,] tileMap)
    {
        var currentDirtTexture = new Texture2D(dirtTexture.width, dirtTexture.height);
        currentDirtTexture.SetPixels(dirtTexture.GetPixels(0, 0, dirtTexture.width, dirtTexture.height));

        for (int x = 0; x < tileMapSize.x; x++)
            for (int y = 0; y < tileMapSize.y; y++)
                if (tileMap[x, y].Equals(transparentTile))
                    currentDirtTexture.SetPixels((x + 1) * tileSizeInPixels,
                                                 (y + 1) * tileSizeInPixels,
                                                 tileSizeInPixels,
                                                 tileSizeInPixels,
                                                 tileMap[x, y].texture.GetPixels());

        currentDirtTexture.Apply();

        dirt.SetDirtTexture(currentDirtTexture);               
    }
    
    private void SetSolidDirtFromTileMap(Tile[,] tileMap)
    {
        var currentSolidTexture = new Texture2D(solidTexture.width, solidTexture.height);
        currentSolidTexture.SetPixels(solidTexture.GetPixels(0, 0, solidTexture.width, solidTexture.height));

        var cleanPixels = new Color[solidDirtThickness * tileSizeInPixels];
        for (int i = 0; i < cleanPixels.Length; i++)
            cleanPixels[i] = new Color(0f, 0f, 0f, 0f);

        for (int x = 0; x < tileMapSize.x; x++)
            for (int y = 0; y < tileMapSize.y; y++)
                foreach (var border in tileMap[x, y].borders)
                    if (border.Value == TileBorder.Connector || tileMap[x,y].Equals(transparentTile))
                        SetSolidDirtPixels(ref currentSolidTexture,
                                            (x + 1) * tileSizeInPixels,
                                            (y + 1) * tileSizeInPixels,
                                            cleanPixels,
                                            border.Key);


        currentSolidTexture.Apply();
        
        dirt.SetSolidDirtTexture(currentSolidTexture);
    }

    private void SetSolidDirtPixels(ref Texture2D texture, int x, int y, Color[] cleanPixels, TileBorderDirection direction)
    {
        switch(direction)
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

    private void SetBackgroundFromTileMap(Tile[,] tileMap)
    {
        var backgroundTexture = new Texture2D(tileMapSize.x * tileSizeInPixels, tileMapSize.y * tileSizeInPixels);

        for (int x = 0; x < tileMap.GetLength(0); x++)
            for (int y = 0; y < tileMap.GetLength(1); y++)
            {
                backgroundTexture.SetPixels(x * tileSizeInPixels,
                                 y * tileSizeInPixels,
                                 tileSizeInPixels,
                                 tileSizeInPixels,
                                 tileMap[x, y].texture.GetPixels());
            }

        backgroundTexture.Apply();

        var sprite = Sprite.Create(backgroundTexture, new Rect(0f, 0f, tileMapSize.x * tileSizeInPixels, tileMapSize.y * tileSizeInPixels), Vector2.zero);
        backgroundSpriteRenderer.sprite = sprite;
    }

    public Tile[] ParseFigures()
    {
        int tilesSummary = 0;
        foreach(var figure in figures)
            tilesSummary += (int)(figure.width * figure.height / math.pow(tileSizeInPixels, 2));

        tilesLib = new Tile[tilesSummary + 1];

        int counter = 0;
        foreach (var figure in figures)
            ParseTexture(figure, ref tilesLib, ref counter);

        tilesLib[tilesLib.Length - 1] = transparentTile;

        return tilesLib;
    }

    public void ParseTexture(Texture2D texture, ref Tile[] tiles, ref int index)
    {
        for (int x = 0; x < texture.width; x += tileSizeInPixels)
            for (int y = 0; y < texture.height; y += tileSizeInPixels)
            {
                var tileTexture = new Texture2D(tileSizeInPixels, tileSizeInPixels);
                tileTexture.SetPixels(texture.GetPixels(x, y, tileSizeInPixels, tileSizeInPixels));
                
                Tile tile = new(tileTexture);

                if (x == 0)
                    tile.borders[TileBorderDirection.Left] = TileBorder.Edge;

                if (x + 1 >= texture.width / tileSizeInPixels)
                    tile.borders[TileBorderDirection.Right] = TileBorder.Edge;

                if (y == 0)
                    tile.borders[TileBorderDirection.Bottom] = TileBorder.Edge;

                if (y + 1 >= texture.height / tileSizeInPixels)
                    tile.borders[TileBorderDirection.Top] = TileBorder.Edge;

                tiles[index] = tile;
                index++;
            }
    }

    private void GeneratorStateChangeHandler(Component<Tile> component, ref Component<Tile>[,] map)
    {
        var tileBorders = component.state.data.borders;
        
        if (component.x > 0)
            DeleteStates(ref map[component.x - 1, component.y], 
                        TileBorderDirection.Right,
                        tileBorders[TileBorderDirection.Left]);
        
        if (component.y > 0)
            DeleteStates(ref map[component.x, component.y - 1],
                        TileBorderDirection.Top,
                        tileBorders[TileBorderDirection.Bottom]);

        if (component.x < map.GetUpperBound(0))
            DeleteStates(ref map[component.x + 1, component.y],
                        TileBorderDirection.Left,
                        tileBorders[TileBorderDirection.Right]);

        if (component.y < map.GetUpperBound(1))
            DeleteStates(ref map[component.x, component.y + 1],
                        TileBorderDirection.Bottom,
                        tileBorders[TileBorderDirection.Top]);
    }

    public void DeleteStates(ref Component<Tile> component, TileBorderDirection direction, TileBorder border)
    {
        var possibleStates = new List<ComponentState<Tile>>();

        foreach(var state in component.possibleStates)
            if (state.data.borders[direction] == border)
                possibleStates.Add(state);

        component.possibleStates = possibleStates.ToArray();
    }

}
