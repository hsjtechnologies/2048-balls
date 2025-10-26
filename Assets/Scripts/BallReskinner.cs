using UnityEngine;

[System.Serializable]
public class BallSkin
{
    public string name;
    public Material material;
    public Sprite icon;
}

public class BallReskinner : MonoBehaviour
{
    [Header("Ball Skins")]
    public BallSkin[] ballSkins;
    
    [Header("Ball Prefabs")]
    public GameObject[] ballPrefabs;
    
    [ContextMenu("Apply Food Theme")]
    public void ApplyFoodTheme()
    {
        // Apply food materials to ball prefabs
        for (int i = 0; i < ballPrefabs.Length && i < ballSkins.Length; i++)
        {
            if (ballPrefabs[i] != null && ballSkins[i].material != null)
            {
                MeshRenderer renderer = ballPrefabs[i].GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = ballSkins[i].material;
                    Debug.Log($"Applied {ballSkins[i].name} material to {ballPrefabs[i].name}");
                }
            }
        }
    }
    
    [ContextMenu("Reset to Default")]
    public void ResetToDefault()
    {
        // Reset to original materials
        foreach (GameObject ball in ballPrefabs)
        {
            if (ball != null)
            {
                MeshRenderer renderer = ball.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    // Reset to default material
                    renderer.material = null;
                }
            }
        }
    }
    
    public void ApplySkin(int ballIndex, int skinIndex)
    {
        if (ballIndex < ballPrefabs.Length && skinIndex < ballSkins.Length)
        {
            MeshRenderer renderer = ballPrefabs[ballIndex].GetComponent<MeshRenderer>();
            if (renderer != null && ballSkins[skinIndex].material != null)
            {
                renderer.material = ballSkins[skinIndex].material;
                Debug.Log($"Applied {ballSkins[skinIndex].name} to {ballPrefabs[ballIndex].name}");
            }
        }
    }
}



