using UnityEngine;
using UnityEngine.UI;

public class WalletConfirmButton : MonoBehaviour
{
    private Button button;
    private TwitterOAuth twitterAuth;
    
    void Start()
    {
        button = GetComponent<Button>();
        twitterAuth = FindObjectOfType<TwitterOAuth>();
        
        if (button != null && twitterAuth != null)
        {
            button.onClick.AddListener(() => twitterAuth.OnConfirmWalletClick());
        }
    }
}