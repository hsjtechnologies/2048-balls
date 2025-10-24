using UnityEngine;
using UnityEngine.UI;

public class TwitterLoginButton : MonoBehaviour
{
    private Button button;
    private TwitterOAuth twitterOAuth;
    
    void Start()
    {
        button = GetComponent<Button>();
        twitterOAuth = FindObjectOfType<TwitterOAuth>();
        
        if (button != null && twitterOAuth != null)
        {
            button.onClick.AddListener(twitterOAuth.OnLoginButtonClick);
        }
        else
        {
            Debug.LogError("TwitterLoginButton: Button or TwitterOAuth not found!");
        }
    }
}
