using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using PlayfabRequests;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool IsLoggedIn = false; //tracks login status
    public GameObject[] balls;
    public GameObject menuGameObject; // Reference to the Menu GameObject to disable after login
    public GameObject gameOverGameObject; // Reference to the Game Over GameObject to enable when game ends
    public GameObject suiGameObject; // Reference to the SUI GameObject to enable after login
    public Instantiater instantiater;
    private bool gameOver = false;
    [SerializeField]
    private float score = 0;
    [SerializeField]
    private int level = 1;
    [SerializeField]
    private float remainingForNextLevel = 25;
    public int howmany2048;
    public int get2048   // property
    {
        get { return howmany2048; }
        set { howmany2048 = value;
            howmany2048Text.text = howmany2048.ToString();
            PlayerPrefs.SetInt("howmany2048", howmany2048);
        }
    }
    //[SerializeField]
    //private long howmany2048best = 0;
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private TMP_Text currentScoreText; // Display the actual current score
    [SerializeField]
    private TMP_Text LevelText;
    [SerializeField]
    private TMP_Text NextLevelText;
    [SerializeField]
    private TextMesh howmany2048Text;
    public Slider scoreSlider;
    //public float shrinkBallSizes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Subscribe to login events from TwitterOAuth
        TwitterOAuth.OnLoginCompleted += OnUserLoggedIn;
        TwitterOAuth.OnLogoutCompleted += OnUserLoggedOut;

        // TEMPORARY: Bypass login for testing prefab spawning
        // IsLoggedIn = true;
        // Time.timeScale = 1f; 
        // Debug.Log("BYPASSED LOGIN FOR TESTING - Prefabs should now spawn");

        // Check if user was already logged in from a previous session
        string savedUsername = PlayerPrefs.GetString("twitter_username", "");

        // Also check for OAuth callback in URL parameters
        string url = Application.absoluteURL;
        bool hasOAuthCallback = !string.IsNullOrEmpty(url) && url.Contains("twitter=success");

        if (!string.IsNullOrEmpty(savedUsername) || hasOAuthCallback)
        {
            // IsLoggedIn = true;
            Time.timeScale = 1f;
            string username = hasOAuthCallback ? "OAuth callback detected" : savedUsername;
            Debug.Log($"Game started - user was already logged in: {username}");

            //Playfab login
            PlayfabManager.Instance.InitLogin(username);
            // Disable the Menu GameObject since user is already logged in
            if (menuGameObject != null)
            {
                menuGameObject.SetActive(false);
                Debug.Log("Menu GameObject disabled - user already logged in");
            }

            // Enable the SUI GameObject since user is already logged in
            if (suiGameObject != null)
            {
                suiGameObject.SetActive(true);
                Debug.Log("SUI GameObject enabled - user already logged in");
            }
        }
        // Check login status
        else if (!IsLoggedIn)
        {
            Time.timeScale = 1f; // Pause all physics & updates
            Debug.Log("Game paused - waiting for user login");

            // Make sure SUI GameObject is disabled when not logged in
            // if (suiGameObject != null)
            // {
            //     suiGameObject.SetActive(false);
            //     Debug.Log("SUI GameObject disabled - waiting for login");
            // }
        }
        else
        {
            Time.timeScale = 1f;
            Debug.Log("Game started - user is logged in");
        }

        //long.TryParse(balls[balls.Length - 1].gameObject.name, out toReach);
        //toReach *= 2;
        //shrinkSizes();

        if (PlayerPrefs.GetInt("level") > 0)
        {
            score = PlayerPrefs.GetFloat("score");
            level = PlayerPrefs.GetInt("level");
            remainingForNextLevel = PlayerPrefs.GetFloat("remainingForNextLevel");
            howmany2048 = PlayerPrefs.GetInt("howmany2048");
        }

        LevelText.text = level.ToString();
        NextLevelText.text = (level + 1).ToString();
        scoreSlider.maxValue = remainingForNextLevel;
        scoreSlider.value = score;
        howmany2048Text.text = howmany2048.ToString();
        scoreText.text = (remainingForNextLevel - score).ToString();

        // Display current score
        if (currentScoreText != null)
            currentScoreText.text = score.ToString();
    }

    void shrinkSizes()
    {
        /*if (shrinkBallSizes == 0 || shrinkBallSizes == 1)
            return;
        GameObject[] ballsInGame = GameObject.FindGameObjectsWithTag("Ball");
        if (ballsInGame.Length > 0)
        {
            for (int i = 0; i < ballsInGame.Length; i++)
            {
                if (shrinkBallSizes > 0)
                    ballsInGame[i].transform.localScale /= shrinkBallSizes;
                else
                    ballsInGame[i].transform.localScale *= -shrinkBallSizes;
            }
        }*/
    }

    public void Merging(string newsize)
    {
        int size;
        int.TryParse(newsize,out size);

        score += size / 2;

        if (score >= remainingForNextLevel)
        {
            score -= remainingForNextLevel;
            if (score < 0)
                score = 0;
            level++;
            remainingForNextLevel *= level / 2;


            scoreSlider.maxValue = remainingForNextLevel;
            LevelText.text = level.ToString();
            NextLevelText.text = (level + 1).ToString();
            PlayerPrefs.SetFloat("remainingForNextLevel", remainingForNextLevel);
            PlayerPrefs.SetInt("level", level);
        }
        scoreSlider.value = score;
        // scoreText.text = (remainingForNextLevel - score).ToString();
        scoreText.text = Mathf.RoundToInt(score + (level * 100)).ToString();
        
        // Update current score display
        if (currentScoreText != null)
            currentScoreText.text = score.ToString();
            
        PlayerPrefs.SetFloat("score", score);
    }


    public void GameOver()
    {
        Debug.Log("GAME OVER");

        // Log final score to database
        LogFinalScore();

        Destroy(GameObject.Find("Instantiater"));
        PlayerPrefs.DeleteAll();
        gameOver = true;
        StartCoroutine(darkenBalls());

        // Enable the Game Over GameObject
        if (gameOverGameObject != null)
        {
            gameOverGameObject.SetActive(true);
            Debug.Log("Game Over UI enabled");
        }
        else
        {
            Debug.LogWarning("Game Over GameObject not assigned in GameManager");
        }
    }

    private void LogFinalScore()
    {
        int finalScore = Mathf.RoundToInt(score + (level * 100));
        Debug.Log("Saving player score...");
        PlayfabManager.Instance.SaveScore(finalScore, level);
        // Find SimpleSignInManager component to log score

        SimpleSignInManager signInManager = FindObjectOfType<SimpleSignInManager>();
        if (signInManager != null && signInManager.IsLoggedIn)
        {
            // Calculate final score (you can adjust this based on your scoring system)

            signInManager.LogScoreToDatabase(finalScore, level);
            Debug.Log($"Final score logged: {finalScore}");

        }
        else
        {
            Debug.LogWarning("Cannot log score - user not logged in or SimpleSignInManager not found");
        }
    }
    IEnumerator darkenBalls() {
      //Declare a yield instruction.
      WaitForSeconds wait = new WaitForSeconds(0.03f);
 
        object[] obj = GameObject.FindSceneObjectsOfType(typeof (GameObject));
        foreach (object o in obj)
        {
            // GameObject g = (GameObject) o;
            // if (g.tag == "Ball")
            // {
            //     // Change ball color to grey
            //     MeshRenderer meshRenderer = g.gameObject.GetComponent<MeshRenderer>();
            //     if (meshRenderer != null && meshRenderer.material != null)
            //     {
            //         meshRenderer.material.color = Color.grey;
            //     }
                
            //     // Only disable first child if it exists
            //     if (g.transform.childCount > 0)
            //     {
            //         g.transform.GetChild(0).gameObject.SetActive(false);
            //     }
            // }
            yield return wait; //Pause the loop for 3 seconds.
        }
    }

    // Event handlers for login system
    private async void OnUserLoggedIn(string twitterUsername, string walletAddress)
    {
        Debug.Log($"User logged in: @{twitterUsername} with wallet: {walletAddress}");
        // Ensure game state is properly set
        // IsLoggedIn = true;
        Time.timeScale = 1f;
        
        Debug.Log($"GameManager: Login confirmed - Time.timeScale set to {Time.timeScale}");
        Debug.Log($"GameManager: IsLoggedIn is now {IsLoggedIn}");
        
        // Disable the Menu GameObject after successful login (safety check)
        if (menuGameObject != null)
        {
            menuGameObject.SetActive(false);
            Debug.Log("Menu GameObject disabled by GameManager after successful login");
        }

        // Enable the SUI GameObject after menu is disabled
        if (suiGameObject != null)
        {
            suiGameObject.SetActive(true);
            Debug.Log("SUI GameObject enabled after successful login");
        }
        else
        {
            Debug.LogWarning("SUI GameObject not assigned in GameManager");
        }

        // You can add additional game initialization here
        // For example, load user-specific data, achievements, etc.

        //Playfab Login
        await PlayfabManager.Instance.InitLogin(twitterUsername);
        //Save Player address
        // if (!string.IsNullOrEmpty(walletAddress) && !string.IsNullOrWhiteSpace(walletAddress))
        // {
        //     PlayfabManager.Instance.SavePlayerWallet(walletAddress);
        // }
    }

    private void OnUserLoggedOut()
    {
        Debug.Log("User logged out");
        
        // Pause game
        Time.timeScale = 0f;
        IsLoggedIn = false;
        
        // Re-enable the Menu GameObject on logout (safety check)
        if (menuGameObject != null)
        {
            menuGameObject.SetActive(true);
            Debug.Log("Menu GameObject re-enabled by GameManager on logout");
        }
        
        // Disable the SUI GameObject on logout
        if (suiGameObject != null)
        {
            suiGameObject.SetActive(false);
            Debug.Log("SUI GameObject disabled by GameManager on logout");
        }
        
        // You can add cleanup logic here
        // For example, save current progress, reset game state, etc.
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");

        // Reset game state
        gameOver = false;
        score = 0;
        level = 1;
        remainingForNextLevel = 25;
        howmany2048 = 0;

        // Clear all PlayerPrefs
        PlayerPrefs.DeleteAll();

        // Reset UI elements
        if (scoreText != null)
            scoreText.text = remainingForNextLevel.ToString();
        if (currentScoreText != null)
            currentScoreText.text = "0";
        if (LevelText != null)
            LevelText.text = level.ToString();
        if (NextLevelText != null)
            NextLevelText.text = (level + 1).ToString();
        if (scoreSlider != null)
        {
            scoreSlider.maxValue = remainingForNextLevel;
            scoreSlider.value = 0;
        }
        if (howmany2048Text != null)
            howmany2048Text.text = "0";
        
        // Disable game over UI
        if (gameOverGameObject != null)
        {
            gameOverGameObject.SetActive(false);
            Debug.Log("Game Over UI disabled");
        }
        
        // Restart the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        Debug.Log("Game restarted successfully");
    }

    /// <summary>
    /// Complete the SUI login process - called after user submits their SUI wallet address
    /// </summary>
    public void CompleteSUILogin()
    {
        Debug.Log("CompleteSUILogin called");
        
        // Ensure game state is set
        IsLoggedIn = true;
        Time.timeScale = 1f;
        instantiater.StartSpawning();
        Debug.Log($"GameManager: SUI Login confirmed - Time.timeScale set to {Time.timeScale}");
        Debug.Log($"GameManager: IsLoggedIn is now {IsLoggedIn}");
        
        // Disable the Menu GameObject after successful SUI login
        if (menuGameObject != null)
        {
            menuGameObject.SetActive(false);
            Debug.Log("Menu GameObject disabled by GameManager after SUI login");
        }
        
        // Enable the SUI GameObject after menu is disabled
        if (suiGameObject != null)
        {
            suiGameObject.SetActive(true);
            Debug.Log("SUI GameObject enabled after SUI login");
        }
        else
        {
            Debug.LogWarning("SUI GameObject not assigned in GameManager");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        TwitterOAuth.OnLoginCompleted -= OnUserLoggedIn;
        TwitterOAuth.OnLogoutCompleted -= OnUserLoggedOut;
    }
    
}

