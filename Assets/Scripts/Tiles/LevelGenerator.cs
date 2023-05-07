using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using WFC;

public class LevelGenerator : MonoBehaviour
{
    public Texture2D[] figures;
    public int tileSize;

    public Dirt dirt;

    private Generator<Tile> generator;
    private Tile[] tiles;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        tiles = ParseFigures();
        generator = new Generator<Tile>(tiles, GeneratorStateChangeHandler);
        spriteRenderer = GetComponent<SpriteRenderer>();
        DebugDraw();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            DebugDraw();
    }

    public void DebugDraw()
    {
        var generatedTexture = GenerateMap(6, 8);
        generatedTexture.Apply();
        
        dirt.SetDefaults();
        
        var sprite = Sprite.Create(generatedTexture, new Rect(0f, 0f, 6 * tileSize, 8 * tileSize), Vector2.zero);
        spriteRenderer.sprite = sprite;
    }

    public Texture2D GenerateMap(int width, int height)
    {
        var result = new Texture2D(width * tileSize, height * tileSize);
        
        Tile[,] map = generator.Generate(width, height);

        for(int x = 0; x < map.GetLength(0); x++)
            for (int y = 0; y < map.GetLength(1); y++)
            {
                result.SetPixels(x * tileSize, 
                                 y * tileSize, 
                                 tileSize, 
                                 tileSize,
                                 map[x,y].texture.GetPixels());
            }

        return result;
    }

    public Tile[] ParseFigures()
    {
        int tilesSummary = 0;
        foreach(var figure in figures)
            tilesSummary += (int)(figure.width * figure.height / math.pow(tileSize, 2));

        tiles = new Tile[tilesSummary];

        int counter = 0;
        foreach (var figure in figures)
            ParseTexture(figure, ref tiles, ref counter);

        return tiles;
    }

    public void ParseTexture(Texture2D texture, ref Tile[] tiles, ref int index)
    {
        for (int x = 0; x < texture.width; x += tileSize)
            for (int y = 0; y < texture.height; y += tileSize)
            {
                var tileTexture = new Texture2D(tileSize, tileSize);
                tileTexture.SetPixels(texture.GetPixels(x, y, tileSize, tileSize));
                
                Tile tile = new(tileTexture);

                if (x == 0)
                    tile.borders[TileBorderDirection.Left] = TileBorder.Edge;

                if (x + 1 >= texture.width / tileSize)
                    tile.borders[TileBorderDirection.Right] = TileBorder.Edge;

                if (y == 0)
                    tile.borders[TileBorderDirection.Bottom] = TileBorder.Edge;

                if (y + 1 >= texture.height / tileSize)
                    tile.borders[TileBorderDirection.Top] = TileBorder.Edge;

                tiles[index] = tile;
                index++;
            }
    }

    private void GeneratorStateChangeHandler(WFC.Component<Tile> component, ref WFC.Component<Tile>[,] map)
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

    public void DeleteStates(ref WFC.Component<Tile> component, TileBorderDirection direction, TileBorder border)
    {
        var possibleStates = new List<ComponentState<Tile>>();

        foreach(var state in component.possibleStates)
            if (state.data.borders[direction] == border)
                possibleStates.Add(state);

        component.possibleStates = possibleStates.ToArray();
    }

}
