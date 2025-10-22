using UnityEngine;
using TMPro;

public class WalletInputField : MonoBehaviour
{
    private TMP_InputField inputField;
    private TwitterOAuth twitterAuth;
    
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        twitterAuth = FindObjectOfType<TwitterOAuth>();
        
        if (inputField != null && twitterAuth != null)
        {
            inputField.onValueChanged.AddListener(twitterAuth.OnWalletAddressChanged);
        }
    }
}
