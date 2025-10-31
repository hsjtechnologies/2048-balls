using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Google Sheets integration for logging game scores
/// This script handles sending score data to Google Sheets via Google Apps Script webhook
/// </summary>
public class GoogleSheetsLogger : MonoBehaviour
{
    [Header("Google Sheets Configuration")]
    [Tooltip("Your Google Apps Script webhook URL")]
    public string webhookURL = "";
    
    [Header("Debug Settings")]
    public bool debugMode = true;
    public bool enableLogging = true;

    private static GoogleSheetsLogger instance;
    public static GoogleSheetsLogger Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GoogleSheetsLogger>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GoogleSheetsLogger");
                    instance = go.AddComponent<GoogleSheetsLogger>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Log a score entry to Google Sheets
    /// </summary>
    /// <param name="twitterUsername">Twitter username</param>
    /// <param name="suiWallet">Sui wallet address</param>
    /// <param name="score">Game score</param>
    /// <param name="level">Game level reached</param>
    /// <param name="gameMode">Game mode or additional info</param>
    public void LogScore(string twitterUsername, string suiWallet, int score, int level = 1, string gameMode = "2048 Balls")
    {
        if (!enableLogging)
        {
            LogDebug("Score logging is disabled");
            return;
        }

        if (string.IsNullOrEmpty(webhookURL))
        {
            LogError("Google Sheets webhook URL not configured!");
            return;
        }

        StartCoroutine(SendScoreToSheets(twitterUsername, suiWallet, score, level, gameMode));
    }

    private IEnumerator SendScoreToSheets(string twitterUsername, string suiWallet, int score, int level, string gameMode)
    {
        // Create score data object
        var scoreData = new ScoreData
        {
            twitter_username = twitterUsername,
            sui_wallet = suiWallet,
            score = score,
            level = level,
            game_mode = gameMode,
            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString()
        };

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(scoreData);
        LogDebug($"Sending score data: {jsonData}");

        // Create web request
        using (var request = new UnityEngine.Networking.UnityWebRequest(webhookURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("User-Agent", "Unity-2048Balls-Game");

            // Send request
            yield return request.SendWebRequest();

            // Handle response
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                LogDebug($"Score logged successfully! Response: {request.downloadHandler.text}");
                
                // Show success message to user (optional)
                ShowScoreLoggedMessage(score);
            }
            else
            {
                LogError($"Failed to log score: {request.error}");
                LogError($"Response: {request.downloadHandler.text}");
                
                // Retry once after 2 seconds
                yield return new WaitForSeconds(2f);
                yield return StartCoroutine(RetryLogScore(scoreData));
            }
        }
    }

    private IEnumerator RetryLogScore(ScoreData scoreData)
    {
        LogDebug("Retrying score log...");
        
        string jsonData = JsonUtility.ToJson(scoreData);
        
        using (var request = new UnityEngine.Networking.UnityWebRequest(webhookURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("User-Agent", "Unity-2048Balls-Game");

            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                LogDebug("Score logged successfully on retry!");
            }
            else
            {
                LogError($"Score logging failed after retry: {request.error}");
            }
        }
    }

    private void ShowScoreLoggedMessage(int score)
    {
        // Optional: Show a UI message that score was logged
        // You can implement this based on your UI system
        LogDebug($"Score {score} has been logged to the leaderboard!");
    }

    /// <summary>
    /// Test the Google Sheets connection
    /// </summary>
    [ContextMenu("Test Google Sheets Connection")]
    public void TestConnection()
    {
        if (string.IsNullOrEmpty(webhookURL))
        {
            LogError("Webhook URL not set!");
            return;
        }

        LogDebug("Testing Google Sheets connection...");
        LogScore("test_user", "0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef", 999, 1, "Test");
    }

    private void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"[GoogleSheetsLogger] {message}");
    }

    private void LogError(string message)
    {
        Debug.LogError($"[GoogleSheetsLogger] {message}");
    }

    /// <summary>
    /// Score data structure for JSON serialization
    /// </summary>
    [System.Serializable]
    public class ScoreData
    {
        public string twitter_username;
        public string sui_wallet;
        public int score;
        public int level;
        public string game_mode;
        public string date;
        public string timestamp;
    }
}
