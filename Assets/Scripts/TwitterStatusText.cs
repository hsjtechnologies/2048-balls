using UnityEngine;
using TMPro;

public class TwitterStatusText : MonoBehaviour
{
    private TextMeshProUGUI statusText;
    private TwitterOAuth twitterAuth;
    
    void Start()
    {
        statusText = GetComponent<TextMeshProUGUI>();
        twitterAuth = FindObjectOfType<TwitterOAuth>();
        
        if (statusText != null && twitterAuth != null)
        {
            // Connect to TwitterOAuth status updates
            twitterAuth.statusText = statusText;
        }
        else
        {
            Debug.LogError("TwitterStatusText: TextMeshProUGUI or TwitterOAuth not found!");
        }
    }
}
