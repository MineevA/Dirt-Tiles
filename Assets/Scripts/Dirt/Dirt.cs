using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Dirt : MonoBehaviour
{
    public Texture2D dirtTexture;

    private MeshRenderer mesh;
    private Texture2D localTexture;

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        LoadTexture(dirtTexture);
        mesh.material.mainTexture = localTexture;
    }

    public void LoadTexture(Texture2D texture, int scale = 100)
    {
        localTexture = new Texture2D(texture.width, texture.height, texture.format, false);
        localTexture.filterMode = FilterMode.Bilinear;
        localTexture.wrapMode = TextureWrapMode.Clamp;
        localTexture.SetPixels32(texture.GetPixels32());
        localTexture.Apply();

        transform.localScale = new Vector3(texture.width / scale,
                                           texture.height / scale,
                                           transform.localScale.z);
    }

    public void DrawPixels(Vector2 uv, bool[,] alphaFlagArray)
    {
        var point = new Vector2Int((int)(localTexture.width * uv.x),
                                   (int)(localTexture.height * uv.y));

        var transparentColor = new Color(0, 0, 0, 0);

        for (int x = 0; x < alphaFlagArray.GetLength(0); x++)
            for (int y = 0; y < alphaFlagArray.GetLength(1); y++)
            {
                if (alphaFlagArray[x, y])
                    localTexture.SetPixel(point.x + x, point.y + y, transparentColor);
            }

        localTexture.Apply();
    }
}
