using UnityEngine;
using UnityEngine.UI;

public class WalletConfirmButton : MonoBehaviour
{
    private Button button;
    private TwitterOAuth twitterOAuth;
    
    void Start()
    {
        button = GetComponent<Button>();
        twitterOAuth = FindObjectOfType<TwitterOAuth>();
        
        if (button != null && twitterOAuth != null)
        {
            button.onClick.AddListener(() => twitterOAuth.OnConfirmWalletClick());
        }
        else
        {
            Debug.LogError("WalletConfirmButton: Button or TwitterOAuth not found!");
        }
    }
}