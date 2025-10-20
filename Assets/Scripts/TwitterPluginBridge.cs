using System.Runtime.InteropServices;
using UnityEngine;

public class TwitterPluginBridge : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void TwitterSignIn();
#endif

    public void OnTwitterLogin()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        TwitterSignIn();
#else
        Debug.Log("Twitter sign-in only works in WebGL builds.");
#endif
    }

    // This method will be called from JS after successful login
    public void OnTwitterLoginSuccess(string userData)
    {
        Debug.Log("✅ Twitter Login Success: " + userData);
        GameManager.IsLoggedIn = true;
        Time.timeScale = 1f;
    }

    public void OnTwitterLoginFailed(string error)
    {
        Debug.LogError("❌ Twitter Login Failed: " + error);
    }
}
