using UnityEngine;
using TMPro;

public class WalletInputField : MonoBehaviour
{
    private TMP_InputField inputField;
    private TwitterOAuth twitterOAuth;
    
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        twitterOAuth = FindObjectOfType<TwitterOAuth>();
        
        if (inputField != null && twitterOAuth != null)
        {
            inputField.onValueChanged.AddListener((value) => {
                twitterOAuth.OnWalletAddressChanged(value);
                Debug.Log($"Wallet address changed: {value}");
            });
            
            // Handle Enter key press
            inputField.onSubmit.AddListener((value) => {
                Debug.Log($"Enter pressed with wallet: {value}");
                if (!string.IsNullOrEmpty(value.Trim()))
                {
                    twitterOAuth.OnConfirmWalletClick();
                }
            });
        }
        else
        {
            Debug.LogError("WalletInputField: InputField or TwitterOAuth not found!");
        }
    }
}
