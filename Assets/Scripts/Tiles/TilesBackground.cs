using UnityEngine;

public class TilesBackground : MonoBehaviour
{
    private Material material;

    private void MaterialInit()
    {
        if (material == null)
            material = GetComponent<MeshRenderer>().material;
    }

    public void SetTexture(Texture2D texture)
    {
        MaterialInit();
        material.SetTexture("_MainTex", texture);
    }
}
