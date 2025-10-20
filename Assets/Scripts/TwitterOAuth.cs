using UnityEngine;
using System.Runtime.InteropServices;

public class TwitterOAuth : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void OpenURL(string url);

    public string backendLoginUrl = "https://my-twitter-backend.onrender.com/auth/twitter";

    public void ConnectToTwitter()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenURL(backendLoginUrl);
#else
        Application.OpenURL(backendLoginUrl);
#endif
    }
}