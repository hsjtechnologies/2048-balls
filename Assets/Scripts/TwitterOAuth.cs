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
    public GameObject menuGameObject; // The Menu GameObject to disable after login

    [Header("Backend Configuration")]
    public string backendURL = "https://two048-balls-webgl-build-1.onrender.com";
    public string gameURL = "https://app.bratonsui.com/";
    public bool debugMode = true;
    [Tooltip("If true, creates a minimal UI at runtime when references are missing.")]
    public bool autoCreateUIIfMissing = false;

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
        
        // Check for OAuth callback in URL parameters
        CheckForOAuthCallbackInURL();
        
        // Only check PlayerPrefs if there's no OAuth callback in URL
        if (!string.IsNullOrEmpty(Application.absoluteURL) && !Application.absoluteURL.Contains("twitter=success"))
        {
            CheckExistingLogin();
        }
    }
    
    private void CheckForOAuthCallbackInURL()
    {
        string url = Application.absoluteURL;
        
        if (!string.IsNullOrEmpty(url) && url.Contains("twitter=success"))
        {
            LogDebug("=== OAuth callback detected in URL on Start ===");
            LogDebug($"URL: {url}");
            UpdateStatusText("Login successful! Processing...");
            
            // Extract username and token from URL parameters
            ExtractOAuthData(url);
            
            // Handle post-login flow
            HandlePostLoginFlow();
        }
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

        // Ensure required UI exists
        if (autoCreateUIIfMissing)
        {
            EnsureRuntimeUI();
        }

        // Initialize panel states
        ShowLoginPanel();
        UpdateStatusText("Ready to login with Twitter");
    }

    private void EnsureRuntimeUI()
    {
        // If wallet UI is not wired, create a minimal one so the user can proceed
        bool needsWalletUi = walletPanel == null || walletAddressInput == null || confirmWalletButton == null;
        bool needsLoginUi = loginPanel == null || loginButton == null || statusText == null;

        if (!needsWalletUi && !needsLoginUi && gamePanel != null && logoutButton != null) return;

        // Find or create canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Container
        GameObject container = GameObject.Find("TwitterOAuthContainer");
        if (container == null)
        {
            container = new GameObject("TwitterOAuthContainer");
            container.transform.SetParent(canvas.transform, false);
            var rt = container.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
        }

        // Minimal Login Panel
        if (needsLoginUi)
        {
            loginPanel = new GameObject("LoginPanel");
            loginPanel.transform.SetParent(container.transform, false);
            var prt = loginPanel.AddComponent<RectTransform>();
            prt.anchorMin = Vector2.zero; prt.anchorMax = Vector2.one; prt.sizeDelta = Vector2.zero;
            var bg = loginPanel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.5f);

            // Status Text
            var statusGO = new GameObject("StatusText");
            statusGO.transform.SetParent(loginPanel.transform, false);
            var srt = statusGO.AddComponent<RectTransform>();
            srt.anchorMin = new Vector2(0.5f, 0.6f); srt.anchorMax = srt.anchorMin; srt.sizeDelta = new Vector2(600, 40);
            statusText = statusGO.AddComponent<TextMeshProUGUI>();
            statusText.alignment = TextAlignmentOptions.Center;
            statusText.fontSize = 20;
            statusText.color = Color.yellow;

            // Login Button
            var btnGO = new GameObject("LoginButton");
            btnGO.transform.SetParent(loginPanel.transform, false);
            var brt = btnGO.AddComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.5f, 0.4f); brt.anchorMax = brt.anchorMin; brt.sizeDelta = new Vector2(220, 60);
            var img = btnGO.AddComponent<Image>(); img.color = new Color(0.2f, 0.6f, 1f, 1f);
            loginButton = btnGO.AddComponent<Button>(); loginButton.targetGraphic = img;
            var txtGO = new GameObject("Text");
            txtGO.transform.SetParent(btnGO.transform, false);
            var trt = txtGO.AddComponent<RectTransform>(); trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one; trt.sizeDelta = Vector2.zero;
            var t = txtGO.AddComponent<TextMeshProUGUI>(); t.text = "Login with Twitter"; t.alignment = TextAlignmentOptions.Center; t.fontSize = 18; t.color = Color.white;
            loginButton.onClick.AddListener(OnLoginButtonClick);
        }

        // Minimal Wallet Panel
        if (needsWalletUi)
        {
            walletPanel = new GameObject("WalletPanel");
            walletPanel.transform.SetParent(container.transform, false);
            var prt = walletPanel.AddComponent<RectTransform>();
            prt.anchorMin = Vector2.zero; prt.anchorMax = Vector2.one; prt.sizeDelta = Vector2.zero;
            var bg = walletPanel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.5f);
            walletPanel.SetActive(false);

            // Username label
            var userGO = new GameObject("UsernameText");
            userGO.transform.SetParent(walletPanel.transform, false);
            var urt = userGO.AddComponent<RectTransform>();
            urt.anchorMin = new Vector2(0.5f, 0.7f); urt.anchorMax = urt.anchorMin; urt.sizeDelta = new Vector2(600, 40);
            usernameText = userGO.AddComponent<TextMeshProUGUI>();
            usernameText.alignment = TextAlignmentOptions.Center; usernameText.fontSize = 22; usernameText.color = Color.white;

            // Input field base
            var inputGO = new GameObject("WalletInputField");
            inputGO.transform.SetParent(walletPanel.transform, false);
            var irt = inputGO.AddComponent<RectTransform>();
            irt.anchorMin = new Vector2(0.5f, 0.5f); irt.anchorMax = irt.anchorMin; irt.sizeDelta = new Vector2(520, 56);
            var inputBg = inputGO.AddComponent<Image>(); inputBg.color = Color.white;
            walletAddressInput = inputGO.AddComponent<TMP_InputField>();
            
            // Text component
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(inputGO.transform, false);
            var txrt = textGO.AddComponent<RectTransform>(); txrt.anchorMin = Vector2.zero; txrt.anchorMax = Vector2.one; txrt.sizeDelta = Vector2.zero;
            var text = textGO.AddComponent<TextMeshProUGUI>(); text.color = Color.black; text.fontSize = 18; text.alignment = TextAlignmentOptions.Left;
            walletAddressInput.textComponent = text;
            
            // Placeholder
            var phGO = new GameObject("Placeholder");
            phGO.transform.SetParent(inputGO.transform, false);
            var phrt = phGO.AddComponent<RectTransform>(); phrt.anchorMin = Vector2.zero; phrt.anchorMax = Vector2.one; phrt.sizeDelta = Vector2.zero;
            var placeholder = phGO.AddComponent<TextMeshProUGUI>(); placeholder.text = "Enter your Sui wallet address (0x...)"; placeholder.color = Color.gray; placeholder.fontSize = 18; placeholder.alignment = TextAlignmentOptions.Left;
            walletAddressInput.placeholder = placeholder;
            walletAddressInput.onValueChanged.AddListener(OnWalletAddressChanged);

            // Confirm button
            var cbtnGO = new GameObject("ConfirmButton");
            cbtnGO.transform.SetParent(walletPanel.transform, false);
            var crt = cbtnGO.AddComponent<RectTransform>();
            crt.anchorMin = new Vector2(0.5f, 0.32f); crt.anchorMax = crt.anchorMin; crt.sizeDelta = new Vector2(180, 52);
            var cimg = cbtnGO.AddComponent<Image>(); cimg.color = new Color(0.2f, 0.8f, 0.2f, 1f);
            confirmWalletButton = cbtnGO.AddComponent<Button>(); confirmWalletButton.targetGraphic = cimg; confirmWalletButton.onClick.AddListener(OnConfirmWalletClick);
            var ctextGO = new GameObject("Text"); ctextGO.transform.SetParent(cbtnGO.transform, false);
            var ctrt = ctextGO.AddComponent<RectTransform>(); ctrt.anchorMin = Vector2.zero; ctrt.anchorMax = Vector2.one; ctrt.sizeDelta = Vector2.zero;
            var ctext = ctextGO.AddComponent<TextMeshProUGUI>(); ctext.text = "Confirm Wallet"; ctext.fontSize = 18; ctext.color = Color.white; ctext.alignment = TextAlignmentOptions.Center;
        }

        // Minimal Game Panel
        if (gamePanel == null || logoutButton == null)
        {
            gamePanel = new GameObject("GamePanel");
            gamePanel.transform.SetParent(container.transform, false);
            var grt = gamePanel.AddComponent<RectTransform>();
            grt.anchorMin = Vector2.zero; grt.anchorMax = Vector2.one; grt.sizeDelta = Vector2.zero;
            gamePanel.SetActive(false);

            var lbtnGO = new GameObject("LogoutButton");
            lbtnGO.transform.SetParent(gamePanel.transform, false);
            var lrt = lbtnGO.AddComponent<RectTransform>(); lrt.anchorMin = new Vector2(1f, 1f); lrt.anchorMax = lrt.anchorMin; lrt.sizeDelta = new Vector2(100, 40); lrt.anchoredPosition = new Vector2(-60, -30);
            var limg = lbtnGO.AddComponent<Image>(); limg.color = new Color(0.8f, 0.2f, 0.2f, 1f);
            logoutButton = lbtnGO.AddComponent<Button>(); logoutButton.targetGraphic = limg; logoutButton.onClick.AddListener(OnLogoutClick);
            var ltxtGO = new GameObject("Text"); ltxtGO.transform.SetParent(lbtnGO.transform, false);
            var ltrt = ltxtGO.AddComponent<RectTransform>(); ltrt.anchorMin = Vector2.zero; ltrt.anchorMax = Vector2.one; ltrt.sizeDelta = Vector2.zero;
            var ltxt = ltxtGO.AddComponent<TextMeshProUGUI>(); ltxt.text = "Logout"; ltxt.fontSize = 14; ltxt.color = Color.white; ltxt.alignment = TextAlignmentOptions.Center;
        }
    }

    private void CheckExistingLogin()
    {
        // Check if user was previously logged in (only check username now)
        string savedUsername = PlayerPrefs.GetString("twitter_username", "");
        
        if (!string.IsNullOrEmpty(savedUsername))
        {
            twitterUsername = savedUsername;
            
            // Disable the Menu GameObject since user is already logged in
            if (menuGameObject != null)
            {
                menuGameObject.SetActive(false);
                LogDebug("Menu GameObject disabled - user already logged in");
            }
            
            CompleteLogin();
        }
    }

    public void OnLoginButtonClick()
    {
        LogDebug("Login button clicked");
        UpdateStatusText("Opening Twitter login...");

        // Open Twitter OAuth flow directly
        string authUrl = $"{backendURL}/auth/twitter";
        LogDebug($"Opening Twitter OAuth: {authUrl}");

        try
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            OpenURL(authUrl);
#else
            Application.OpenURL(authUrl);
#endif
        }
        catch (System.Exception e)
        {
            LogError($"Failed to open URL: {e.Message}");
            UpdateStatusText("Failed to open login page. Please try again.");
            return;
        }

        // Start listening for callback
        StartCoroutine(ListenForOAuthCallback());
    }

    // Temporary method for testing when backend is down
    [ContextMenu("Test Login (Backend Down)")]
    public void TestLoginWhenBackendDown()
    {
        LogDebug("Testing login with fake credentials (backend is down)");
        UpdateStatusText("Using test login - backend is down");
        
        // Set fake credentials for testing
        twitterUsername = "test_user";
        accessToken = "test_token";
        
        // Show wallet input panel
        ShowWalletPanel();
        UpdateStatusText("Test login successful! Please enter your Sui wallet address.");
    }

    // Quick test method to verify login flow and timescale
    [ContextMenu("Test Complete Login Flow")]
    public void TestCompleteLoginFlow()
    {
        LogDebug("Testing complete login flow");
        twitterUsername = "test_user";
        CompleteLogin();
    }

    public void LoginButton()
    {   
        // Just call the login button click - don't force wallet panel
        OnLoginButtonClick();
        // Wallet panel will be shown automatically after successful Twitter authentication
        // in ListenForOAuthCallback() method
    }

    private IEnumerator ListenForOAuthCallback()
    {
        LogDebug("=== STARTING OAUTH CALLBACK LISTENER ===");
        LogDebug("Waiting for Twitter login callback...");
        UpdateStatusText("Waiting for Twitter login...");
        
        float timeoutSeconds = 120f; // 2 minute timeout
        float elapsedTime = 0f;
        
        while (elapsedTime < timeoutSeconds)
        {
            // Check if there's a URL with OAuth parameters
            string url = Application.absoluteURL;
            LogDebug($"Current URL: {url}");
            
            // Check for callback parameters in the URL
            if (url.Contains("twitter=success"))
            {
                LogDebug("=== OAUTH CALLBACK DETECTED ===");
                LogDebug($"Full callback URL: {url}");
                UpdateStatusText("Login successful! Processing...");
                
                // Extract username and token from URL parameters
                ExtractOAuthData(url);
                
                LogDebug($"Extracted username: {twitterUsername}");
                LogDebug($"Extracted token length: {accessToken.Length}");
                
                // Handle post-login flow
                HandlePostLoginFlow();
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

    /// <summary>
    /// Handle what happens after successful Twitter login
    /// MODIFY THIS METHOD TO CHANGE THE POST-LOGIN FLOW
    /// </summary>
    private void HandlePostLoginFlow()
    {
        LogDebug("Handling post-login flow...");
        
        // Skip wallet input, go directly to game
        CompleteLogin();
        
        LogDebug("Post-login flow completed");
    }

    // Public helper to force show wallet panel (e.g., if callback is blocked)
    public void ForceShowWalletPanel()
    {
        ShowWalletPanel();
        UpdateStatusText("Please enter your Sui wallet address to continue");
    }

    public void OnWalletAddressChanged(string address)
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
            UpdateStatusText("Please enter a valid Sui wallet address (0x + 66 characters)");
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
        LogDebug($"Setting GameManager.IsLoggedIn = true");
        LogDebug($"Setting Time.timeScale = 1f");
        
        // Save the username to PlayerPrefs so it persists after reload
        if (!string.IsNullOrEmpty(twitterUsername))
        {
            PlayerPrefs.SetString("twitter_username", twitterUsername);
            PlayerPrefs.Save();
            LogDebug($"Saved username to PlayerPrefs: {twitterUsername}");
        }
        
        // Set game state
        // GameManager.IsLoggedIn = true;
        // Time.timeScale = 1f;
        // isLoggedIn = true;
        
        LogDebug($"GameManager.IsLoggedIn is now: {GameManager.IsLoggedIn}");
        LogDebug($"Time.timeScale is now: {Time.timeScale}");
        
        // Force update timescale to ensure it's properly set
        Time.timeScale = 1f;
        LogDebug($"Time.timeScale forced to: {Time.timeScale}");
        
        // Disable the Menu GameObject after successful login
        if (menuGameObject != null)
        {
            menuGameObject.SetActive(false);
            LogDebug("Menu GameObject disabled after successful login");
        }
        else
        {
            LogDebug("WARNING: menuGameObject is not assigned - Menu may still be visible");
        }
        
        // Update UI
        UpdateStatusText($"Welcome @{twitterUsername}! Game ready to play!");
        ShowGamePanel();
        
        LogDebug("Calling OnLoginCompleted event");
        // Notify other systems (pass empty wallet address since we're not using it)
        OnLoginCompleted?.Invoke(twitterUsername, "");
        
        LogDebug($"OnLoginCompleted event invoked with username: {twitterUsername}");
        LogDebug("CompleteLogin finished successfully");
    }

    private void OnLogoutClick()
    {
        LogDebug("Logout clicked");
        
        // Clear saved data (only username now)
        PlayerPrefs.DeleteKey("twitter_username");
        PlayerPrefs.Save();
        
        // Reset game state
        GameManager.IsLoggedIn = false;
        Time.timeScale = 0f;
        isLoggedIn = false;
        
        // Reset variables
        twitterUsername = "";
        accessToken = "";
        
        // Re-enable the Menu GameObject on logout
        if (menuGameObject != null)
        {
            menuGameObject.SetActive(true);
            LogDebug("Menu GameObject re-enabled on logout");
        }
        
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
        LogDebug($"loginPanel: {loginPanel?.name ?? "null"}");
        LogDebug($"walletPanel: {walletPanel?.name ?? "null"}");
        LogDebug($"gamePanel: {gamePanel?.name ?? "null"}");
        
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
            LogDebug("No game panel assigned - wallet panel will remain visible");
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
            Debug.Log($"[TwitterOAuth] {message}");
    }

    private void LogError(string message)
    {
        Debug.LogError($"[TwitterOAuth] {message}");
    }

    // Public method to be called from JavaScript after OAuth callback
    public void OnOAuthCallback(string url)
    {
        LogDebug($"OAuth callback received: {url}");
        
        if (url.Contains("twitter=success"))
        {
            LogDebug("OAuth callback detected!");
            UpdateStatusText("Login successful! Processing...");
            
            // Extract username and token from URL parameters
            ExtractOAuthData(url);
            
            // Handle post-login flow
            HandlePostLoginFlow();
        }
        else
        {
            LogError("Invalid OAuth callback URL");
            UpdateStatusText("Login failed. Please try again.");
        }
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