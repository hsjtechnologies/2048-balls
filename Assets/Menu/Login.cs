using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Login : MonoBehaviour
{
    public Button loginButton;
    public GameObject walletPanel;  
    
    // 🌐 Backend URL (change this based on where your backend is)
    private string backendURL = "https://two048-balls-webgl-build-1.onrender.com";
    
    private string twitterUsername = "";
    private string accessToken = "";

    private void Start()
    {
        if (loginButton == null)
            loginButton = GetComponent<Button>();

        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginButtonClick);
        else
            Debug.LogError("Button not assigned!");
    }

    private void OnLoginButtonClick()
    {
        Debug.Log("🔐 Login button clicked!");
        
        // ✅ Step 1: Open your Render backend OAuth flow
        string authUrl = $"{backendURL}/auth/twitter";
        Debug.Log($"🌐 Opening: {authUrl}");
        Application.OpenURL(authUrl);

        // ✅ Step 2: Start listening for OAuth callback
        StartCoroutine(ListenForOAuthCallback());
    }

    private System.Collections.IEnumerator ListenForOAuthCallback()
    {
        Debug.Log("⏳ Waiting for Twitter login callback...");
        
        // Wait for user to complete Twitter login
        // (In production, use deep linking to detect callback)
        // For now, poll for URL parameters
        
        float timeoutSeconds = 120f; // 2 minute timeout
        float elapsedTime = 0f;
        
        while (elapsedTime < timeoutSeconds)
        {
            // Check if there's a URL with OAuth parameters
            string url = Application.absoluteURL;
            
            if (url.Contains("twitter=success"))
            {
                Debug.Log("✅ OAuth callback detected!");
                
                // Extract username and token from URL parameters
                ExtractOAuthData(url);
                
                // Show wallet panel
                if (walletPanel != null)
                    walletPanel.SetActive(true);
                
                loginButton.gameObject.SetActive(false);
                yield break;
            }
            
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(1f);
        }
        
        Debug.LogWarning("⏱️ OAuth callback timeout - user may have cancelled login");
    }

    private void ExtractOAuthData(string url)
    {
        Debug.Log($"🔍 Extracting OAuth data from: {url}");
        
        // Extract username parameter
        if (url.Contains("username="))
        {
            int startIndex = url.IndexOf("username=") + 9;
            int endIndex = url.IndexOf("&", startIndex);
            if (endIndex == -1) endIndex = url.Length;
            
            twitterUsername = url.Substring(startIndex, endIndex - startIndex);
            twitterUsername = System.Uri.UnescapeDataString(twitterUsername);
            Debug.Log($"👤 Twitter Username: {twitterUsername}");
        }
        
        // Extract token parameter
        if (url.Contains("token="))
        {
            int startIndex = url.IndexOf("token=") + 6;
            int endIndex = url.IndexOf("&", startIndex);
            if (endIndex == -1) endIndex = url.Length;
            
            accessToken = url.Substring(startIndex, endIndex - startIndex);
            accessToken = System.Uri.UnescapeDataString(accessToken);
            Debug.Log($"🔑 Access Token received (length: {accessToken.Length})");
        }
        
        // Resume game
        // Time.timeScale = 1f;
        GameManager.IsLoggedIn = true;
    }

    // ✅ Public methods to get user data
    public string GetTwitterUsername() => twitterUsername;
    public string GetAccessToken() => accessToken;
}