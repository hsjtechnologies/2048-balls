using UnityEngine;
using System.Collections;

/// <summary>
/// Backend connectivity tester
/// Use this to test if your backend server is working properly
/// </summary>
public class BackendTester : MonoBehaviour
{
    [Header("Backend Configuration")]
    public string backendURL = "https://ball-game-hlvu.onrender.com";
    
    [ContextMenu("Test Backend Connection")]
    public void TestBackendConnection()
    {
        StartCoroutine(TestConnection());
    }
    
    private IEnumerator TestConnection()
    {
        Debug.Log("=== BACKEND CONNECTION TEST ===");
        
        // Test 1: Basic connectivity
        Debug.Log($"Testing basic connectivity to: {backendURL}");
        string testUrl = $"{backendURL}/";
        
        using (var request = new UnityEngine.Networking.UnityWebRequest(testUrl, "GET"))
        {
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Backend is responding! Status: {request.responseCode}");
                Debug.Log($"Response: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"❌ Backend connection failed: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
            }
        }
        
        // Test 2: Twitter OAuth endpoint
        Debug.Log($"Testing Twitter OAuth endpoint: {backendURL}/auth/twitter");
        string authUrl = $"{backendURL}/auth/twitter";
        
        using (var request = new UnityEngine.Networking.UnityWebRequest(authUrl, "GET"))
        {
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Twitter OAuth endpoint is responding! Status: {request.responseCode}");
                Debug.Log($"Response: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"❌ Twitter OAuth endpoint failed: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
            }
        }
        
        Debug.Log("=== END BACKEND TEST ===");
    }
    
    [ContextMenu("Test Twitter OAuth URL")]
    public void TestTwitterOAuthURL()
    {
        string authUrl = $"{backendURL}/auth/twitter";
        Debug.Log($"Twitter OAuth URL: {authUrl}");
        Debug.Log("Copy this URL and test it in your browser");
        
        // Open in browser for testing
        Application.OpenURL(authUrl);
    }
}
