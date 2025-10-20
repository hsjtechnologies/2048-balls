using UnityEngine;

public class TwitterLoginManager : MonoBehaviour
{
    public GameObject loginUI;      // UI before login
    public GameObject loggedInUI;   // UI after login

    void Start()
    {
        string url = Application.absoluteURL;

        if (!string.IsNullOrEmpty(url) && url.Contains("twitter=success"))
        {
            Debug.Log("✅ Twitter login detected!");

            if (loginUI != null) loginUI.SetActive(false);
            if (loggedInUI != null) loggedInUI.SetActive(true);
        }
        else
        {
            Debug.Log("⏳ No Twitter login detected yet.");
        }
    }
}
