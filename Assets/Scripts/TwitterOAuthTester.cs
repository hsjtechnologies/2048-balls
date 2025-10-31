using UnityEngine;
using System.Collections;

/// <summary>
/// Twitter OAuth Backend Configuration Tester
/// Tests if your backend is properly configured for Twitter OAuth
/// </summary>
public class TwitterOAuthTester : MonoBehaviour
{
    [Header("Backend Configuration")]
    public string backendURL = "https://ball-game-hlvu.onrender.com";
    
    [ContextMenu("Test Twitter OAuth Configuration")]
    public void TestTwitterOAuthConfig()
    {
        StartCoroutine(TestOAuthConfig());
    }
    
    private IEnumerator TestOAuthConfig()
    {
        Debug.Log("=== TWITTER OAUTH CONFIGURATION TEST ===");
        
        // Test 1: Check if backend is responding
        Debug.Log("1. Testing backend connectivity...");
        string testUrl = $"{backendURL}/";
        
        using (var request = new UnityEngine.Networking.UnityWebRequest(testUrl, "GET"))
        {
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Backend is responding (Status: {request.responseCode})");
            }
            else
            {
                Debug.LogError($"❌ Backend is not responding: {request.error}");
                Debug.LogError("SOLUTION: Check if your Render.com service is running");
                yield break;
            }
        }
        
        // Test 2: Check Twitter OAuth endpoint
        Debug.Log("2. Testing Twitter OAuth endpoint...");
        string authUrl = $"{backendURL}/auth/twitter";
        
        using (var request = new UnityEngine.Networking.UnityWebRequest(authUrl, "GET"))
        {
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Twitter OAuth endpoint is responding (Status: {request.responseCode})");
                Debug.Log($"Response: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"❌ Twitter OAuth endpoint failed: {request.error}");
                Debug.LogError($"Status Code: {request.responseCode}");
                
                if (request.responseCode == 500)
                {
                    Debug.LogError("SOLUTION: Backend has internal error - check Render.com logs");
                }
                else if (request.responseCode == 401)
                {
                    Debug.LogError("SOLUTION: Backend authentication failed - check Twitter API keys");
                }
            }
        }
        
        // Test 3: Check callback endpoint
        Debug.Log("3. Testing Twitter OAuth callback endpoint...");
        string callbackUrl = $"{backendURL}/auth/twitter/callback";
        
        using (var request = new UnityEngine.Networking.UnityWebRequest(callbackUrl, "GET"))
        {
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Callback endpoint is responding (Status: {request.responseCode})");
            }
            else
            {
                Debug.LogError($"❌ Callback endpoint failed: {request.error}");
                Debug.LogError($"Status Code: {request.responseCode}");
            }
        }
        
        Debug.Log("=== CONFIGURATION CHECKLIST ===");
        Debug.Log("Please verify these in your Render.com dashboard:");
        Debug.Log("1. Environment Variables:");
        Debug.Log("   - TWITTER_API_KEY (your Twitter app API key)");
        Debug.Log("   - TWITTER_API_SECRET (your Twitter app API secret)");
        Debug.Log("   - TWITTER_CALLBACK_URL=https://two048-balls-webgl-build-1.onrender.com/auth/twitter/callback");
        Debug.Log("");
        Debug.Log("2. Twitter Developer Portal:");
        Debug.Log("   - Callback URL: https://two048-balls-webgl-build-1.onrender.com/auth/twitter/callback");
        Debug.Log("   - App permissions: Read (minimum)");
        Debug.Log("   - API Keys: Must match environment variables");
        Debug.Log("");
        Debug.Log("3. Render.com Service:");
        Debug.Log("   - Service is running (not sleeping)");
        Debug.Log("   - No errors in logs");
        Debug.Log("   - Environment variables are set");
        
        Debug.Log("=== END CONFIGURATION TEST ===");
    }
    
    [ContextMenu("Open Twitter Developer Portal")]
    public void OpenTwitterDeveloperPortal()
    {
        Application.OpenURL("https://developer.twitter.com/");
        Debug.Log("Opened Twitter Developer Portal - check your app configuration");
    }
    
    [ContextMenu("Open Render.com Dashboard")]
    public void OpenRenderDashboard()
    {
        Application.OpenURL("https://dashboard.render.com/");
        Debug.Log("Opened Render.com Dashboard - check your service and environment variables");
    }
}
