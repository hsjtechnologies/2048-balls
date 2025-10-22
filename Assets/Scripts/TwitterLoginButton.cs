using UnityEngine;
using UnityEngine.UI;

public class TwitterLoginButton : MonoBehaviour
{
    private Button button;
    private TwitterOAuth twitterAuth;
    
    void Start()
    {
        button = GetComponent<Button>();
        twitterAuth = FindObjectOfType<TwitterOAuth>();
        
        if (button != null && twitterAuth != null)
        {
            button.onClick.AddListener(twitterAuth.OnLoginButtonClick);
        }
        else
        {
            Debug.LogError("TwitterLoginButton: Button or TwitterOAuth not found!");
        }
    }
}
