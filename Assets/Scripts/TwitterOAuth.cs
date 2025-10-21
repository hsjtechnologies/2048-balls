using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class TwitterOAuth : MonoBehaviour
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

    [Header("Backend Configuration")]
    public string backendURL = "https://ball-game-hlvu.onrender.com";
    public bool debugMode = true;

    [Header("Google Sheets Integration")]
    public string googleSheetsWebhookURL = ""; // Set this to your Google Apps Script webhook

    // Private variables
    private string twitterUsername = "";
    private string accessToken = "";
    private string suiWalletAddress = "";
    private bool isLoggedIn = false;

    // WebGL support
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void OpenURL(string url);
#endif

    private void Start()
    {
        InitializeUI();
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
            if (walletAddressInput.placeholder != null)
            {
                var placeholderText = walletAddressInput.placeholder.GetComponent<TextMeshProUGUI>();
                if (placeholderText != null)
                    placeholderText.text = "Enter your Sui wallet address (0x...)";
            }
        }

        // Initialize panel states
        ShowLoginPanel();
        UpdateStatusText("Ready to login with Twitter");
    }

    private void CheckExistingLogin()
    {
        // Check if user was previously logged in
        string savedUsername = PlayerPrefs.GetString("twitter_username", "");
        string savedWallet = PlayerPrefs.GetString("sui_wallet", "");
        
        if (!string.IsNullOrEmpty(savedUsername) && !string.IsNullOrEmpty(savedWallet))
        {
            twitterUsername = savedUsername;
            suiWalletAddress = savedWallet;
            CompleteLogin();
        }
    }

    public void OnLoginButtonClick()
    {
        LogDebug("Login button clicked");
        UpdateStatusText("Opening Twitter login...");

        // Open Twitter OAuth flow
        string authUrl = $"{backendURL}/auth/twitter";
        LogDebug($"Opening: {authUrl}");

#if UNITY_WEBGL && !UNITY_EDITOR
        OpenURL(authUrl);
#else
        Application.OpenURL(authUrl);
#endif

        // Start listening for callback
        StartCoroutine(ListenForOAuthCallback());
    }

    private IEnumerator ListenForOAuthCallback()
    {
        LogDebug("Waiting for Twitter login callback...");
        UpdateStatusText("Waiting for Twitter login...");
        
        float timeoutSeconds = 120f; // 2 minute timeout
        float elapsedTime = 0f;
        
        while (elapsedTime < timeoutSeconds)
        {
            // Check if there's a URL with OAuth parameters
            string url = Application.absoluteURL;
            
            if (url.Contains("twitter=success"))
            {
                LogDebug("OAuth callback detected!");
                UpdateStatusText("Login successful! Processing...");
                
                // Extract username and token from URL parameters
                ExtractOAuthData(url);
                
                // Show wallet input panel
                ShowWalletPanel();
                yield break;
            }
            
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(1f);
        }
        
        LogError("OAuth callback timeout - user may have cancelled login");
        UpdateStatusText("Login timeout. Please try again.");
    }

    private void ExtractOAuthData(string url)
    {
        LogDebug($"Extracting OAuth data from: {url}");
        
        // Extract username parameter
        if (url.Contains("username="))
        {
            int startIndex = url.IndexOf("username=") + 9;
            int endIndex = url.IndexOf("&", startIndex);
            if (endIndex == -1) endIndex = url.Length;
            
            twitterUsername = url.Substring(startIndex, endIndex - startIndex);
            twitterUsername = Uri.UnescapeDataString(twitterUsername);
            LogDebug($"Twitter Username: {twitterUsername}");
        }
        
        // Extract token parameter
        if (url.Contains("token="))
        {
            int startIndex = url.IndexOf("token=") + 6;
            int endIndex = url.IndexOf("&", startIndex);
            if (endIndex == -1) endIndex = url.Length;
            
            accessToken = url.Substring(startIndex, endIndex - startIndex);
            accessToken = Uri.UnescapeDataString(accessToken);
            LogDebug($"Access Token received (length: {accessToken.Length})");
        }

        // Update UI
        if (usernameText != null)
            usernameText.text = $"Welcome, @{twitterUsername}!";
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
            UpdateStatusText("Valid Sui wallet address. Click confirm to continue.");
        }
        else if (!string.IsNullOrEmpty(suiWalletAddress))
        {
            UpdateStatusText("Invalid Sui wallet address format (must start with 0x and be 66 characters)");
        }
    }

    private bool IsValidSuiAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            return false;
        
        // Sui address validation (starts with 0x and is 66 characters long)
        return address.StartsWith("0x") && address.Length == 66;
    }

    private void OnConfirmWalletClick()
    {
        if (string.IsNullOrEmpty(suiWalletAddress) || !IsValidSuiAddress(suiWalletAddress))
        {
            UpdateStatusText("Please enter a valid Sui wallet address");
            return;
        }

        LogDebug($"Wallet address confirmed: {suiWalletAddress}");
        
        // Save user data
        PlayerPrefs.SetString("twitter_username", twitterUsername);
        PlayerPrefs.SetString("sui_wallet", suiWalletAddress);
        PlayerPrefs.Save();

        // Complete login process
        CompleteLogin();
    }

    private void CompleteLogin()
    {
        LogDebug("Login process completed successfully");
        
        // Set game state
        GameManager.IsLoggedIn = true;
        Time.timeScale = 1f;
        isLoggedIn = true;
        
        // Update UI
        UpdateStatusText($"Welcome! Game ready. Wallet: {suiWalletAddress.Substring(0, 10)}...");
        ShowGamePanel();
        
        // Notify other systems
        OnLoginCompleted?.Invoke(twitterUsername, suiWalletAddress);
    }

    private void OnLogoutClick()
    {
        LogDebug("Logout clicked");
        
        // Clear saved data
        PlayerPrefs.DeleteKey("twitter_username");
        PlayerPrefs.DeleteKey("sui_wallet");
        PlayerPrefs.Save();
        
        // Reset game state
        GameManager.IsLoggedIn = false;
        Time.timeScale = 0f;
        isLoggedIn = false;
        
        // Reset variables
        twitterUsername = "";
        accessToken = "";
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
        if (loginPanel != null) loginPanel.SetActive(false);
        if (walletPanel != null) walletPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);
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
            Debug.Log($"[TwitterOAuth] {message}");
    }

    private void LogError(string message)
    {
        Debug.LogError($"[TwitterOAuth] {message}");
    }

    // Public events for other systems to listen to
    public static event Action<string, string> OnLoginCompleted;
    public static event Action OnLogoutCompleted;

    // Public getters
    public bool IsLoggedIn => isLoggedIn;
    public string TwitterUsername => twitterUsername;
    public string WalletAddress => suiWalletAddress;
    public string AccessToken => accessToken;

    // Method to log scores to Google Sheets
    public void LogScoreToDatabase(int score)
    {
        if (!isLoggedIn)
        {
            LogError("Cannot log score - user not logged in");
            return;
        }

        StartCoroutine(SendScoreToGoogleSheets(score));
    }

    private IEnumerator SendScoreToGoogleSheets(int score)
    {
        if (string.IsNullOrEmpty(googleSheetsWebhookURL))
        {
            LogError("Google Sheets webhook URL not configured");
            yield break;
        }

        // Create score data
        var scoreData = new
        {
            twitter_username = twitterUsername,
            sui_wallet = suiWalletAddress,
            score = score,
            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            game = "2048 Balls"
        };

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(scoreData);
        LogDebug($"Sending score data: {jsonData}");

        // Send to Google Sheets via webhook
        using (var request = new UnityEngine.Networking.UnityWebRequest(googleSheetsWebhookURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                LogDebug("Score logged successfully to Google Sheets");
            }
            else
            {
                LogError($"Failed to log score: {request.error}");
            }
        }
    }
}