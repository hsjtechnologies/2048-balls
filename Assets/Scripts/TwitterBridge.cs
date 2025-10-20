using System.Runtime.InteropServices;
using UnityEngine;

public class TwitterBridge : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void TwitterSignIn();  // must match function name in twitter.jslib
#endif

    public void StartTwitterLogin()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        TwitterSignIn();
#else
        Debug.Log("Twitter login only runs in WebGL builds.");
#endif
    }

    // Called from JS when login succeeds
    public void OnTwitterLoginSuccess(string userData)
    {
        Debug.Log($"✅ Logged in as: {userData}");
        // Example: resume game
        Time.timeScale = 1f;
    }

    // Called from JS when login fails
    public void OnTwitterLoginFailed(string error)
    {
        Debug.LogError($"❌ Login failed: {error}");
    }
}
