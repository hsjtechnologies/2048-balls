using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.SimpleSignIn.X.Scripts;
using System;

/// <summary>
/// Complete login system using SimpleSignIn X (Twitter) plugin + Sui Wallet
/// This replaces the TwitterOAuth script with proper SimpleSignIn integration
/// </summary>
public class SimpleSignInManager : MonoBehaviour
{
    [Header("UI References")]
    public Button loginButton;
    public GameObject loginPanel;
    public GameObject walletPanel;
    public GameObject gamePanel;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI usernameText;
    public TMP_InputField walletAddressInput;
    public Button confirmWalletButton;
    public Button logoutButton;

    [Header("Settings")]
    public bool debugMode = true;

    [Header("Google Sheets Integration")]
    public string googleSheetsWebhookURL = "";

    // Private variables
    private XAuth xAuth;
    private string twitterUsername = "";
    private string twitterName = "";
    private string twitterId = "";
    private string suiWalletAddress = "";
    private bool isLoggedIn = false;

    private void Start()
    {
        InitializeUI();
        InitializeXAuth();
        CheckExistingLogin();
    }

    private void InitializeUI()
    {
        // Setup button listeners
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginButtonClick);
        
        if (confirmWalletButton != null)
            confirmWalletButton.onClick.AddListener(OnConfirmWalletClick);
        
        if (logoutButton != null)
            logoutButton.onClick.AddListener(OnLogoutClick);

        // Setup wallet input validation
        if (walletAddressInput != null)
        {
            walletAddressInput.onValueChanged.AddListener(OnWalletAddressChanged);
        }

        // Initialize panel states
        ShowLoginPanel();
        UpdateStatusText("Ready to login with Twitter");
    }

    private void InitializeXAuth()
    {
        try
        {
            // Initialize SimpleSignIn XAuth
            xAuth = new XAuth();
            
            // Try to resume any existing auth session
            xAuth.TryResume(OnSignInCallback, null);
            
            LogDebug("XAuth initialized successfully");
        }
        catch (Exception e)
        {
            LogError($"Failed to initialize XAuth: {e.Message}");
            UpdateStatusText("Failed to initialize Twitter authentication");
        }
    }

    private void CheckExistingLogin()
    {
        // Check if user was previously logged in
        string savedUsername = PlayerPrefs.GetString("twitter_username", "");
        string savedWallet = PlayerPrefs.GetString("sui_wallet", "");
        
        if (!string.IsNullOrEmpty(savedUsername) && !string.IsNullOrEmpty(savedWallet))
        {
            LogDebug("Found existing login session");
            twitterUsername = savedUsername;
            suiWalletAddress = savedWallet;
            CompleteLogin();
        }
    }

    public void OnLoginButtonClick()
    {
        LogDebug("Login button clicked - starting SimpleSignIn flow");
        UpdateStatusText("Opening Twitter login...");

        try
        {
            // Use SimpleSignIn XAuth to sign in
            xAuth.SignIn(OnSignInCallback, caching: true);
        }
        catch (Exception e)
        {
            LogError($"Login failed: {e.Message}");
            UpdateStatusText("Login failed. Please try again.");
        }
    }

    private void OnSignInCallback(bool success, string error, UserInfo userInfo)
    {
        if (success && userInfo != null)
        {
            LogDebug($"Login successful: {userInfo.Name} (@{userInfo.Username})");
            
            // Store user info
            twitterUsername = userInfo.Username;
            twitterName = userInfo.Name;
            twitterId = userInfo.Id;
            
            // Update UI
            if (usernameText != null)
                usernameText.text = $"Welcome, {userInfo.Name}!\n@{userInfo.Username}";
            
            // Show wallet input panel
            ShowWalletPanel();
            
            // Check if wallet address is already saved
            string savedWallet = PlayerPrefs.GetString($"wallet_{userInfo.Id}", "");
            if (!string.IsNullOrEmpty(savedWallet))
            {
                suiWalletAddress = savedWallet;
                if (walletAddressInput != null)
                    walletAddressInput.text = savedWallet;
                
                // Auto-confirm if we have a saved wallet
                LogDebug("Found saved wallet, auto-confirming");
                UpdateStatusText("Using saved Sui wallet address...");
                OnConfirmWalletClick();
            }
            else
            {
                // Try to connect Sui Wallet automatically
                UpdateStatusText("Connecting to Sui Wallet...");
                TryConnectSuiWallet();
            }
        }
        else
        {
            LogError($"Login failed: {error}");
            UpdateStatusText($"Login failed: {error}");
        }
    }
    
    private void TryConnectSuiWallet()
    {
        // Subscribe to Sui Wallet events
        SuiWalletManager.OnWalletConnected += OnSuiWalletConnected;
        SuiWalletManager.OnWalletConnectionFailed += OnSuiWalletConnectionFailed;
        
        // Try to connect
        if (SuiWalletManager.Instance != null)
        {
            LogDebug("Attempting to connect Sui Wallet...");
            SuiWalletManager.Instance.ConnectWallet();
        }
        else
        {
            LogError("SuiWalletManager not found in scene!");
            UpdateStatusText("Please enter your Sui wallet address manually.");
        }
    }
    
    private void OnSuiWalletConnected(string address)
    {
        LogDebug($"Sui Wallet connected with address: {address}");
        
        // Unsubscribe from events
        SuiWalletManager.OnWalletConnected -= OnSuiWalletConnected;
        SuiWalletManager.OnWalletConnectionFailed -= OnSuiWalletConnectionFailed;
        
        // Set the wallet address
        suiWalletAddress = address;
        if (walletAddressInput != null)
            walletAddressInput.text = address;
        
        UpdateStatusText("Sui Wallet connected! Click confirm to continue.");
        
        // Auto-confirm
        OnConfirmWalletClick();
    }
    
    private void OnSuiWalletConnectionFailed(string error)
    {
        LogError($"Sui Wallet connection failed: {error}");
        
        // Unsubscribe from events
        SuiWalletManager.OnWalletConnected -= OnSuiWalletConnected;
        SuiWalletManager.OnWalletConnectionFailed -= OnSuiWalletConnectionFailed;
        
        UpdateStatusText($"Wallet connection failed. Please enter address manually.\nError: {error}");
    }

    private void OnWalletAddressChanged(string address)
    {
        suiWalletAddress = address.Trim();
        ValidateWalletAddress();
    }

    private void ValidateWalletAddress()
    {
        bool isValid = IsValidSuiAddress(suiWalletAddress);
        
        if (confirmWalletButton != null)
            confirmWalletButton.interactable = isValid;
        
        if (isValid)
        {
            UpdateStatusText("Valid wallet address format. Click confirm to continue.");
        }
        else if (!string.IsNullOrEmpty(suiWalletAddress))
        {
            UpdateStatusText("Invalid format - must start with 0x and be exactly 66 characters");
        }
    }

    private bool IsValidSuiAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            return false;
        
        // Simple Sui address validation: just check format (0x + 66 characters total)
        // No need to validate hex characters - just basic format check
        return address.StartsWith("0x") && address.Length == 66;
    }

    public void OnConfirmWalletClick()
    {
        LogDebug($"OnConfirmWalletClick called. Current wallet address: '{suiWalletAddress}'");
        
        // Get the current value from the input field if it's not set
        if (string.IsNullOrEmpty(suiWalletAddress) && walletAddressInput != null)
        {
            suiWalletAddress = walletAddressInput.text.Trim();
            LogDebug($"Retrieved wallet address from input field: '{suiWalletAddress}'");
        }
        
        if (string.IsNullOrEmpty(suiWalletAddress) || !IsValidSuiAddress(suiWalletAddress))
        {
            LogError($"Invalid wallet address: '{suiWalletAddress}'");
            UpdateStatusText("Please enter a valid wallet address (0x + exactly 66 characters)");
            return;
        }

        LogDebug($"Wallet address confirmed: {suiWalletAddress}");
        
        // Save user data
        PlayerPrefs.SetString("twitter_username", twitterUsername);
        PlayerPrefs.SetString("twitter_name", twitterName);
        PlayerPrefs.SetString("twitter_id", twitterId);
        PlayerPrefs.SetString("sui_wallet", suiWalletAddress);
        PlayerPrefs.SetString($"wallet_{twitterId}", suiWalletAddress);
        PlayerPrefs.Save();

        // Save user data to Google Sheets immediately
        SaveUserDataToSheets();
        
        // Complete login process
        CompleteLogin();
    }

    private void CompleteLogin()
    {
        LogDebug("Login process completed successfully");
        LogDebug($"Setting GameManager.IsLoggedIn = true");
        LogDebug($"Setting Time.timeScale = 1f");
        
        // Set game state
        GameManager.IsLoggedIn = true;
        Time.timeScale = 1f;
        isLoggedIn = true;
        
        LogDebug($"GameManager.IsLoggedIn is now: {GameManager.IsLoggedIn}");
        LogDebug($"Time.timeScale is now: {Time.timeScale}");
        
        // Update UI
        UpdateStatusText($"Welcome! Game ready. Wallet: {suiWalletAddress.Substring(0, 10)}...");
        ShowGamePanel();
        
        LogDebug("Calling OnLoginCompleted event");
        // Notify other systems
        OnLoginCompleted?.Invoke(twitterUsername, suiWalletAddress);
        
        LogDebug("CompleteLogin finished successfully");
    }

    private void OnLogoutClick()
    {
        LogDebug("Logout clicked");
        
        // Sign out from XAuth
        if (xAuth != null)
        {
            xAuth.SignOut(revokeAccessToken: true);
        }
        
        // Clear saved data
        PlayerPrefs.DeleteKey("twitter_username");
        PlayerPrefs.DeleteKey("twitter_name");
        PlayerPrefs.DeleteKey("twitter_id");
        PlayerPrefs.DeleteKey("sui_wallet");
        if (!string.IsNullOrEmpty(twitterId))
        {
            PlayerPrefs.DeleteKey($"wallet_{twitterId}");
        }
        PlayerPrefs.Save();
        
        // Reset game state
        GameManager.IsLoggedIn = false;
        Time.timeScale = 0f;
        isLoggedIn = false;
        
        // Reset variables
        twitterUsername = "";
        twitterName = "";
        twitterId = "";
        suiWalletAddress = "";
        
        if (walletAddressInput != null)
            walletAddressInput.text = "";
        
        // Reset UI
        ShowLoginPanel();
        UpdateStatusText("Logged out successfully");
        
        OnLogoutCompleted?.Invoke();
    }

    private void ShowLoginPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(true);
        if (walletPanel != null) walletPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
    }

    private void ShowWalletPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (walletPanel != null) walletPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);
    }

    private void ShowGamePanel()
    {
        LogDebug("ShowGamePanel called");
        
        if (loginPanel != null) 
        {
            loginPanel.SetActive(false);
            LogDebug("Login panel deactivated");
        }
        
        if (walletPanel != null) 
        {
            walletPanel.SetActive(false);
            LogDebug("Wallet panel deactivated");
        }
        
        if (gamePanel != null) 
        {
            gamePanel.SetActive(true);
            LogDebug("Game panel activated");
        }
        else
        {
            // If no game panel assigned, just hide wallet panel
            LogDebug("No game panel assigned - hiding wallet panel only");
        }
    }

    private void UpdateStatusText(string message)
    {
        if (statusText != null)
            statusText.text = message;
        
        LogDebug($"Status: {message}");
    }

    private void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"[SimpleSignInManager] {message}");
    }

    private void LogError(string message)
    {
        Debug.LogError($"[SimpleSignInManager] {message}");
    }

    // Public events for other systems to listen to
    public static event Action<string, string> OnLoginCompleted;
    public static event Action OnLogoutCompleted;

    // Public getters
    public bool IsLoggedIn => isLoggedIn;
    public string TwitterUsername => twitterUsername;
    public string TwitterName => twitterName;
    public string WalletAddress => suiWalletAddress;

    /// <summary>
    /// Save user data (username + wallet) to Google Sheets immediately after wallet confirmation
    /// </summary>
    private void SaveUserDataToSheets()
    {
        if (string.IsNullOrEmpty(twitterUsername) || string.IsNullOrEmpty(suiWalletAddress))
        {
            LogError("Cannot save user data - missing username or wallet address");
            return;
        }

        GoogleSheetsLogger sheetsLogger = FindObjectOfType<GoogleSheetsLogger>();
        if (sheetsLogger != null)
        {
            // Log user registration with score 0 to indicate this is a user registration, not a game score
            sheetsLogger.LogScore(twitterUsername, suiWalletAddress, 0, 0, "User Registration");
            LogDebug($"User data saved to Google Sheets: @{twitterUsername} - {suiWalletAddress}");
        }
        else
        {
            LogError("GoogleSheetsLogger not found in scene - user data not saved");
        }
    }

    // Method to log scores to Google Sheets
    public void LogScoreToDatabase(int score, int level = 1)
    {
        if (!isLoggedIn)
        {
            LogError("Cannot log score - user not logged in");
            return;
        }

        GoogleSheetsLogger sheetsLogger = FindObjectOfType<GoogleSheetsLogger>();
        if (sheetsLogger != null)
        {
            sheetsLogger.LogScore(twitterUsername, suiWalletAddress, score, level);
        }
        else
        {
            LogError("GoogleSheetsLogger not found in scene");
        }
    }
}
