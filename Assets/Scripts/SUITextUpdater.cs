using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using PlayfabRequests;
using Core.DataModels;
using WebGLCopyAndPaste;

public class SUITextUpdater : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Button updateButton;
    
    [Header("Submit Actions")]
    [SerializeField] private GameObject[] gameObjectsToDisable; // GameObjects to disable after submit
    [SerializeField] private float disableDelay = 0f; // Delay before disabling (in seconds)
    [SerializeField] private bool completeGameLogin = true; // Call GameManager.CompleteSUILogin() after submit
    
    [Header("Text Formatting")]
    [SerializeField] private string prefix = "Wallet: ";
    [SerializeField] private string suffix = "";
    [SerializeField] private bool showPlaceholderWhenEmpty = true;
    [SerializeField] private string placeholderText = "Enter wallet address...";
    [SerializeField] private bool shortenAddress = true;
    [SerializeField] private int startChars = 6; // Characters to show at start (e.g., "0x8f17")
    [SerializeField] private int endChars = 4;  // Characters to show at end (e.g., "7248")
    
    [Header("Validation")]
    [SerializeField] private bool validateSuiAddress = true;
    [SerializeField] private Color validColor = Color.green;
    [SerializeField] private Color invalidColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;
    
    private void Start()
    {
        // Setup button listener
        if (updateButton != null)
        {
            updateButton.onClick.AddListener(OnButtonClick);
        }
        
        // Setup input field listener for real-time validation
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnInputChanged);
        }
        
        // Initialize text
        UpdateText();
    }
    
    public async void UpdateText()
    {
        if (targetText == null) return;
        
        string inputValue = inputField != null ? inputField.text.Trim() : "";
        
        // Handle empty input
        if (string.IsNullOrEmpty(inputValue))
        {
            if (showPlaceholderWhenEmpty)
            {
                targetText.text = placeholderText;
                targetText.color = Color.gray;
            }
            else
            {
                targetText.text = prefix + "No address" + suffix;
                targetText.color = normalColor;
            }
            return;
        }
        
        // Validate SUI address if enabled
        if (validateSuiAddress)
        {
            bool isValid = IsValidSuiAddress(inputValue);
            targetText.color = isValid ? validColor : invalidColor;

            if (!isValid)
            {
                targetText.text = prefix + inputValue + " (Invalid)" + suffix;
                return;
            }

            var (loadState, message) = await PlayfabManager.Instance.SavePlayerWallet(inputValue);

            if (loadState != LoaderEnum.Loaded)
            {
                PlayfabManager.Instance.PlayerSuiWalletAddr = inputValue;
            }
        }
        else
        {
            targetText.color = normalColor;
        }
        
        // Format the address for display
        string displayText = FormatAddressForDisplay(inputValue);
        targetText.text = prefix + displayText + suffix;
        
        Debug.Log($"SUI Text Updated: {targetText.text}");
    }
    
    private void OnButtonClick()
    {
        string inputValue = inputField != null ? inputField.text.Trim() : "";
        
        // Check if input is valid before updating
        if (validateSuiAddress && !IsValidSuiAddress(inputValue))
        {
            Debug.LogWarning("Cannot submit: Invalid SUI address format. Must be 66 characters starting with 0x");
            return;
        }
        
        UpdateText();
        
        // Complete game login if enabled
        if (completeGameLogin)
        {
            CompleteGameLogin();
        }
        
        // Disable GameObjects after successful submission
        DisableGameObjectsAfterSubmit();
    }
    
    private void OnInputChanged(string newValue)
    {
        // Real-time validation as user types
        if (validateSuiAddress && targetText != null)
        {
            bool isValid = IsValidSuiAddress(newValue);
            targetText.color = isValid ? validColor : invalidColor;
            
            // Enable/disable button based on validation
            if (updateButton != null)
            {
                updateButton.interactable = isValid || string.IsNullOrEmpty(newValue);
            }
        }
    }
    
    private bool IsValidSuiAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            return false;
        
        // SUI address validation (starts with 0x and is 66 characters long)
        return address.StartsWith("0x") && address.Length == 66;
    }
    
    private string FormatAddressForDisplay(string address)
    {
        if (string.IsNullOrEmpty(address))
            return address;
        
        // If shortening is disabled or address is too short, return as is
        if (!shortenAddress || address.Length < (startChars + endChars))
            return address;
        
        // Extract start and end parts
        string startPart = address.Substring(0, startChars);
        string endPart = address.Substring(address.Length - endChars);
        
        // Return formatted address with ellipsis
        return startPart + "..." + endPart;
    }
    
    private void DisableGameObjectsAfterSubmit()
    {
        if (gameObjectsToDisable != null && gameObjectsToDisable.Length > 0)
        {
            if (disableDelay > 0f)
            {
                StartCoroutine(DisableGameObjectsWithDelay());
            }
            else
            {
                DisableGameObjects();
            }
        }
    }
    
    private IEnumerator DisableGameObjectsWithDelay()
    {
        yield return new WaitForSeconds(disableDelay);
        DisableGameObjects();
    }
    
    private void DisableGameObjects()
    {
        foreach (GameObject obj in gameObjectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                Debug.Log($"GameObject disabled after submit: {obj.name}");
            }
        }
    }
    
    private void CompleteGameLogin()
    {
        // Find GameManager and call CompleteSUILogin
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.CompleteSUILogin();
            Debug.Log("GameManager.CompleteSUILogin() called - game should now start");
        }
        else
        {
            Debug.LogWarning("GameManager not found - cannot complete login process");
        }
    }
    
    // Public method to set input field value programmatically
    public void SetInputValue(string value)
    {
        if (inputField != null)
        {
            inputField.text = value;
            UpdateText();
        }
    }
    
    // Public method to get current input value
    public string GetInputValue()
    {
        return inputField != null ? inputField.text.Trim() : "";
    }
    
    // Public method to clear input and reset text
    public void ClearInput()
    {
        if (inputField != null)
        {
            inputField.text = "";
            UpdateText();
        }
    }
    
    // Public method to update text without validation
    public void UpdateTextDirect(string customText)
    {
        if (targetText != null)
        {
            targetText.text = customText;
            targetText.color = normalColor;
        }
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (updateButton != null)
        {
            updateButton.onClick.RemoveListener(OnButtonClick);
        }

        if (inputField != null)
        {
            inputField.onValueChanged.RemoveListener(OnInputChanged);
        }
    }

    public void CopyWalletAddressToClipboard()
    {
        string walletAddress = PlayfabManager.Instance.PlayerSuiWalletAddr;
        if (!string.IsNullOrEmpty(walletAddress))
        {
            WebGLCopyAndPasteAPI.CopyToClipboard(walletAddress);
            Debug.Log($"Wallet address copied to clipboard: {walletAddress}");
        }
        else
        {
            Debug.LogWarning("Cannot copy: Wallet address is empty");
        }
    }
}
