using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using WFC;

public class TileMap
{
    private Tile[,] map;
    public readonly int width;
    public readonly int height;
    public readonly int tileSize;

    private Tile[] tilesLib;
    private Tile transparentTile;
    private Tile fullEdgeSingleTile;
    private Generator<Tile> generator;
    private int generatedTiles;
    private int tilesToGenerate;

    public TileMap(int width, int height, int tileSize)
    {
        this.width = width;
        this.height = height;
        this.tileSize = tileSize;
    }

    public Tile Tile(int x, int y) => map[x, y];
    
    public Tile Tile(float x, float y) => map[(int)x, (int)y];
    
    #region textures

    private void AddTransparentTileToLib()
    {
        var texture = new Texture2D(tileSize, tileSize);
        for (int x = 0; x < texture.height; x++)
            for (int y = 0; y < texture.width; y++)
                texture.SetPixel(x, y, Color.clear);

        transparentTile = new Tile(texture, TileBorder.Edge);
        transparentTile.transparent = true;

        tilesLib[tilesLib.Length - 1] = transparentTile;
    }

    public void ConvertTexturesToTile(Texture2D[] tileTextures)
    {
        int tilesSummary = 0;
        foreach (var figure in tileTextures)
            tilesSummary += figure.width * figure.height / (tileSize * tileSize);

        tilesLib = new Tile[tilesSummary + 1];

        int counter = 0;
        foreach (var figure in tileTextures)
            ConvertTextureToTile(figure, ref tilesLib, ref counter);

        AddTransparentTileToLib();

        generator = new Generator<Tile>(tilesLib, GeneratorStateChangeHandler, BeforeFillMap);
    }

    public void ConvertTextureToTile(Texture2D texture, ref Tile[] tiles, ref int index)
    {
        for (int x = 0; x < texture.width; x += tileSize)
            for (int y = 0; y < texture.height; y += tileSize)
            {   
                var tileTexture = new Texture2D(tileSize, tileSize);
                var onlyEdges = true;
                tileTexture.SetPixels(texture.GetPixels(x, y, tileSize, tileSize));

                Tile tile = new(tileTexture);

                if (x == 0)
                    tile.borders[TileBorderDirection.Left] = TileBorder.Edge;
                else
                    onlyEdges = false;

                if (x + 1 >= texture.width / tileSize)
                    tile.borders[TileBorderDirection.Right] = TileBorder.Edge;
                else
                    onlyEdges = false;

                if (y == 0)
                    tile.borders[TileBorderDirection.Bottom] = TileBorder.Edge;
                else
                    onlyEdges = false;

                if (y + 1 >= texture.height / tileSize)
                    tile.borders[TileBorderDirection.Top] = TileBorder.Edge;
                else
                    onlyEdges = false;

                tiles[index] = tile;
                index++;

                if (onlyEdges)
                    fullEdgeSingleTile = tile;
            }
    }

    #endregion

    #region generator

    public void GenerateMap(int tilesCount)
    {
        if (tilesLib.Length == 0)
            throw new Exception("Библиотека тайлов пуста");

        generatedTiles = 0;
        tilesToGenerate = tilesCount;
        
        map = generator.Generate(width, height);
    }

    private void BeforeFillMap(ref Component<Tile>[,] map)
    {
        var bounds = new Vector2Int(map.GetUpperBound(0), map.GetUpperBound(1));
        var maxPriority = 6;
        var maxPriorityPosition = bounds / 2;

        for (int x = 1; x < bounds.x; x++)
            for (int y = 1; y < bounds.y; y++)
                map[x, y].priorityWeight = maxPriority - math.abs(maxPriorityPosition.x - x) - math.abs(maxPriorityPosition.y - y);

        for (int x = 0; x <= bounds.x; x++)
        {
            SetPossibleStatesByCondition(ref map[x, 0], TileBorderDirection.Bottom, TileBorder.Edge);
            SetPossibleStatesByCondition(ref map[x, bounds.y], TileBorderDirection.Top, TileBorder.Edge);
            map[x, 0].priorityWeight = -1;
            map[x, bounds.x].priorityWeight = -1;
        }

        for (int y = 0; y <= bounds.y; y++)
        {
            SetPossibleStatesByCondition(ref map[0, y], TileBorderDirection.Left, TileBorder.Edge);
            SetPossibleStatesByCondition(ref map[bounds.x, y], TileBorderDirection.Right, TileBorder.Edge);
            map[0, y].priorityWeight = -1;
            map[bounds.x, y].priorityWeight = -1;
        }
    }

    private void GeneratorStateChangeHandler(Component<Tile> component, ref Component<Tile>[,] map)
    {
        if (!component.state.data.transparent)
            generatedTiles++;

        if (generatedTiles >= tilesToGenerate)
        {
            generator.SetStateToUndefinedComponents(new ComponentState<Tile>(transparentTile, 0));
            return;
        }

        var tileBorders = component.state.data.borders;

        if (component.x > 0)
            SetPossibleStatesByCondition(ref map[component.x - 1, component.y],
                        TileBorderDirection.Right,
                        tileBorders[TileBorderDirection.Left]);

        if (component.y > 0)
            SetPossibleStatesByCondition(ref map[component.x, component.y - 1],
                        TileBorderDirection.Top,
                        tileBorders[TileBorderDirection.Bottom]);

        if (component.x < map.GetUpperBound(0))
            SetPossibleStatesByCondition(ref map[component.x + 1, component.y],
                        TileBorderDirection.Left,
                        tileBorders[TileBorderDirection.Right]);

        if (component.y < map.GetUpperBound(1))
            SetPossibleStatesByCondition(ref map[component.x, component.y + 1],
                        TileBorderDirection.Bottom,
                        tileBorders[TileBorderDirection.Top]);

        if (generatedTiles == tilesToGenerate - 1 && !ComponentHaveConnectors(component))
        {
            var possibleStates = new ComponentState<Tile>[2];
            possibleStates[0] = new ComponentState<Tile>(transparentTile, 0);
            possibleStates[1] = new ComponentState<Tile>(fullEdgeSingleTile, 0);
            generator.SetPossibleStatesToUndefinedComponents(possibleStates);
        }

    }

    private void SetPossibleStatesByCondition(ref Component<Tile> component, TileBorderDirection direction, TileBorder border)
    {
        var possibleStates = new List<ComponentState<Tile>>();

        foreach (var state in component.possibleStates)
            if (state.data.borders[direction] == border)
                possibleStates.Add(state);

        component.possibleStates = possibleStates.ToArray();

        if (border.Equals(TileBorder.Connector))
            component.priorityWeight = 100;
    }

    private bool ComponentHaveConnectors(Component<Tile> component)
    {
        foreach (var border in component.state.data.borders)
            if (border.Equals(TileBorder.Connector))
                return true;

        return false;
    }

    #endregion
}
