using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Dirt : MonoBehaviour
{
    //public Texture2D dirtTexture;

    public CustomRenderTexture alphaMap;
    public Material alphaMaterial;

    private readonly int ErasePosition = Shader.PropertyToID("_ErasePosition");

    private void Start()
    {
        alphaMap.Initialize();
    }

    public void DrawPixels(Vector2 uv)
    {
        alphaMaterial.SetVector(ErasePosition, uv);
    }
}
