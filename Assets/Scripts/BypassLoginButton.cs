using UnityEngine;
using UnityEngine.UI;

public class BypassLoginButton : MonoBehaviour
{
    private Button button;
    private TwitterOAuth twitterOAuth;
    
    void Start()
    {
        button = GetComponent<Button>();
        twitterOAuth = FindObjectOfType<TwitterOAuth>();
        
        if (button != null && twitterOAuth != null)
        {
            button.onClick.AddListener(() => {
                Debug.Log("Bypass login button clicked");
                twitterOAuth.BypassLoginAndStartGame();
            });
        }
        else
        {
            Debug.LogError("BypassLoginButton: Button or TwitterOAuth not found!");
        }
    }
}
