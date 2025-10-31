using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script to verify SimpleSignInManager and SuiWalletManager setup
/// Attach this to any GameObject and check the Console for diagnostics
/// </summary>
public class SetupVerifier : MonoBehaviour
{
    [Header("Click 'Verify Setup' button in Inspector")]
    [SerializeField] private bool verifyOnStart = true;

    private void Start()
    {
        if (verifyOnStart)
        {
            VerifySetup();
        }
    }

    [ContextMenu("Verify Setup")]
    public void VerifySetup()
    {
        Debug.Log("========== SETUP VERIFICATION START ==========");
        
        VerifySimpleSignInManager();
        VerifySuiWalletManager();
        VerifyUISetup();
        
        Debug.Log("========== SETUP VERIFICATION END ==========");
    }

    private void VerifySimpleSignInManager()
    {
        Debug.Log("--- Checking SimpleSignInManager ---");
        
        SimpleSignInManager manager = FindObjectOfType<SimpleSignInManager>();
        
        if (manager == null)
        {
            Debug.LogError("❌ SimpleSignInManager NOT FOUND in scene!");
            Debug.LogError("   → Create an empty GameObject and add the SimpleSignInManager script to it");
            return;
        }
        
        Debug.Log("✅ SimpleSignInManager found");
        
        // Use reflection to check private fields
        var type = manager.GetType();
        
        // Check public fields via Inspector
        var loginButton = type.GetField("loginButton").GetValue(manager) as Button;
        var loginPanel = type.GetField("loginPanel").GetValue(manager) as GameObject;
        var walletPanel = type.GetField("walletPanel").GetValue(manager) as GameObject;
        var gamePanel = type.GetField("gamePanel").GetValue(manager) as GameObject;
        var statusText = type.GetField("statusText").GetValue(manager) as TextMeshProUGUI;
        var usernameText = type.GetField("usernameText").GetValue(manager) as TextMeshProUGUI;
        var walletAddressInput = type.GetField("walletAddressInput").GetValue(manager) as TMP_InputField;
        var confirmWalletButton = type.GetField("confirmWalletButton").GetValue(manager) as Button;
        
        CheckField("loginButton", loginButton);
        CheckField("loginPanel", loginPanel);
        CheckField("walletPanel", walletPanel);
        CheckField("gamePanel", gamePanel, optional: true);
        CheckField("statusText", statusText);
        CheckField("usernameText", usernameText);
        CheckField("walletAddressInput", walletAddressInput);
        CheckField("confirmWalletButton", confirmWalletButton);
        
        Debug.Log("");
    }

    private void VerifySuiWalletManager()
    {
        Debug.Log("--- Checking SuiWalletManager ---");
        
        SuiWalletManager manager = FindObjectOfType<SuiWalletManager>();
        
        if (manager == null)
        {
            Debug.LogError("❌ SuiWalletManager NOT FOUND in scene!");
            Debug.LogError("   → Create an empty GameObject and add the SuiWalletManager script to it");
            return;
        }
        
        Debug.Log("✅ SuiWalletManager found");
        
        if (SuiWalletManager.Instance == null)
        {
            Debug.LogWarning("⚠️  SuiWalletManager.Instance is null (will initialize on Awake)");
        }
        else
        {
            Debug.Log("✅ SuiWalletManager.Instance is initialized");
        }
        
        Debug.Log("");
    }

    private void VerifyUISetup()
    {
        Debug.Log("--- Checking UI Setup ---");
        
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        Debug.Log($"Found {canvases.Length} Canvas(es) in scene");
        
        Button[] buttons = FindObjectsOfType<Button>(true); // Include inactive
        Debug.Log($"Found {buttons.Length} Button(s) in scene (including inactive)");
        
        TMP_InputField[] inputFields = FindObjectsOfType<TMP_InputField>(true);
        Debug.Log($"Found {inputFields.Length} TMP_InputField(s) in scene (including inactive)");
        
        TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>(true);
        Debug.Log($"Found {texts.Length} TextMeshProUGUI(s) in scene (including inactive)");
        
        Debug.Log("");
    }

    private void CheckField(string fieldName, Object value, bool optional = false)
    {
        if (value == null)
        {
            if (optional)
            {
                Debug.LogWarning($"⚠️  {fieldName} is not assigned (optional)");
            }
            else
            {
                Debug.LogError($"❌ {fieldName} is NOT assigned!");
                Debug.LogError($"   → Assign it in the SimpleSignInManager Inspector");
            }
        }
        else
        {
            Debug.Log($"✅ {fieldName} is assigned: {value.name}");
        }
    }

    [ContextMenu("Force Show Wallet Panel")]
    public void ForceShowWalletPanel()
    {
        Debug.Log("Attempting to show wallet panel...");
        
        SimpleSignInManager manager = FindObjectOfType<SimpleSignInManager>();
        if (manager == null)
        {
            Debug.LogError("SimpleSignInManager not found!");
            return;
        }
        
        var type = manager.GetType();
        var walletPanel = type.GetField("walletPanel").GetValue(manager) as GameObject;
        
        if (walletPanel != null)
        {
            walletPanel.SetActive(true);
            Debug.Log("✅ Wallet panel activated!");
        }
        else
        {
            Debug.LogError("❌ Wallet panel is not assigned!");
        }
    }

    [ContextMenu("Test Manual Wallet Input")]
    public void TestManualWalletInput()
    {
        Debug.Log("Testing manual wallet input...");
        
        SuiWalletManager manager = SuiWalletManager.Instance;
        if (manager == null)
        {
            manager = FindObjectOfType<SuiWalletManager>();
        }
        
        if (manager == null)
        {
            Debug.LogError("❌ SuiWalletManager not found!");
            return;
        }
        
        string testAddress = "0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef";
        Debug.Log($"Setting test wallet address: {testAddress}");
        
        manager.SetManualAddress(testAddress);
        
        if (manager.IsWalletConnected())
        {
            Debug.Log($"✅ Wallet connected! Address: {manager.GetConnectedAddress()}");
        }
        else
        {
            Debug.LogError("❌ Wallet connection failed!");
        }
    }
}




