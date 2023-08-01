using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Texture2D[]      figures;

    public Texture2D        dirtTexture;
    public Texture2D        solidTexture;

    public int              tileSizeInPixels;
    public int              solidDirtThickness;
    public int              levelNumber;
    public Vector2Int       tileMapSize;
    public PercentCounter   mainCounter;

    public TileMap         tileMap;
    public TileRenderer    tileRenderer;
    public Dirt            dirt;
    public TilesBackground tilesBackground;
    public NavigationMap   navigationMap;

    void Start()
    {
        tileMap = new TileMap(tileMapSize.x, tileMapSize.y, tileSizeInPixels);
        tileRenderer = new TileRenderer(dirtTexture, solidTexture, tileMapSize * tileSizeInPixels);
        navigationMap = new NavigationMap(tileMapSize + Vector2Int.one, new Vector2(-3, -4));
        
        tileMap.ConvertTexturesToTile(figures);
        mainCounter.OnMainCounterUpdate += OnMainCounterUpdate;

        GenerateLevel();
    }

    private void OnMainCounterUpdate(float counter)
    {
        if (counter >= 99.995f)
            GenerateLevel();
    }

    public void GenerateLevel()
    {
        levelNumber++;
        
        tileMap.GenerateMap(levelNumber);
        navigationMap.FillMap(tileMap);
        tileRenderer.Render(tileMap, dirt, solidDirtThickness, tilesBackground);
        
        dirt.SetDefaults();
    }
}
