using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Automatic UI Setup for TwitterOAuth system
/// This script creates all necessary UI elements for the login system
/// </summary>
public class TwitterOAuthUISetup : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool autoSetupOnStart = true;
    public bool createCanvasIfMissing = true;
    
    [Header("UI Styling")]
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    public Color buttonColor = new Color(0.2f, 0.6f, 1f, 1f);
    public Color successColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color errorColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    
    [Header("Font Settings")]
    public int titleFontSize = 36;
    public int normalFontSize = 18;
    public int smallFontSize = 14;

    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupLoginUI();
        }
    }

    [ContextMenu("Setup Login UI")]
    public void SetupLoginUI()
    {
        Debug.Log("Setting up TwitterOAuth Login UI...");
        
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null && createCanvasIfMissing)
        {
            canvas = CreateCanvas();
        }

        if (canvas == null)
        {
            Debug.LogError("No Canvas found! Please create a Canvas first or enable 'createCanvasIfMissing'");
            return;
        }

        // Create main container
        GameObject mainContainer = CreateMainContainer(canvas);
        
        // Add TwitterOAuth component
        TwitterOAuth twitterAuth = mainContainer.GetComponent<TwitterOAuth>();
        if (twitterAuth == null)
        {
            twitterAuth = mainContainer.AddComponent<TwitterOAuth>();
        }

        // Create UI panels
        CreateLoginPanel(mainContainer, twitterAuth);
        CreateWalletPanel(mainContainer, twitterAuth);
        CreateGamePanel(mainContainer, twitterAuth);

        Debug.Log("TwitterOAuth UI setup complete!");
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private GameObject CreateMainContainer(Canvas canvas)
    {
        GameObject container = new GameObject("TwitterOAuthContainer");
        container.transform.SetParent(canvas.transform, false);
        
        RectTransform rect = container.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        return container;
    }

    private void CreateLoginPanel(GameObject parent, TwitterOAuth twitterAuth)
    {
        GameObject loginPanel = CreatePanel(parent, "LoginPanel", backgroundColor);
        twitterAuth.loginPanel = loginPanel;

        // Title
        CreateText(loginPanel, "Title", "2048 Balls Game", 
            new Vector2(0.5f, 0.8f), new Vector2(400, 60), titleFontSize, Color.white);

        // Status Text
        TextMeshProUGUI statusText = CreateText(loginPanel, "StatusText", "Ready to login with Twitter", 
            new Vector2(0.5f, 0.6f), new Vector2(600, 40), normalFontSize, Color.yellow);
        twitterAuth.statusText = statusText;

        // Login Button
        Button loginButton = CreateButton(loginPanel, "LoginButton", "Login with Twitter", 
            new Vector2(0.5f, 0.4f), new Vector2(200, 60), buttonColor, normalFontSize);
        twitterAuth.loginButton = loginButton;
    }

    private void CreateWalletPanel(GameObject parent, TwitterOAuth twitterAuth)
    {
        GameObject walletPanel = CreatePanel(parent, "WalletPanel", backgroundColor);
        walletPanel.SetActive(false);
        twitterAuth.walletPanel = walletPanel;

        // Username Text
        TextMeshProUGUI usernameText = CreateText(walletPanel, "UsernameText", "Welcome!", 
            new Vector2(0.5f, 0.7f), new Vector2(600, 40), normalFontSize + 6, Color.white);
        twitterAuth.usernameText = usernameText;

        // Wallet Input Field
        TMP_InputField walletInput = CreateInputField(walletPanel, "WalletInputField", 
            new Vector2(0.5f, 0.5f), new Vector2(500, 50));
        twitterAuth.walletAddressInput = walletInput;

        // Confirm Button
        Button confirmButton = CreateButton(walletPanel, "ConfirmButton", "Confirm Wallet", 
            new Vector2(0.5f, 0.3f), new Vector2(150, 50), successColor, normalFontSize);
        twitterAuth.confirmWalletButton = confirmButton;
    }

    private void CreateGamePanel(GameObject parent, TwitterOAuth twitterAuth)
    {
        GameObject gamePanel = CreatePanel(parent, "GamePanel", Color.clear);
        gamePanel.SetActive(false);
        twitterAuth.gamePanel = gamePanel;

        // Logout Button (top right)
        Button logoutButton = CreateButton(gamePanel, "LogoutButton", "Logout", 
            new Vector2(1f, 1f), new Vector2(100, 40), errorColor, smallFontSize);
        logoutButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-60, -30);
        twitterAuth.logoutButton = logoutButton;
    }

    private GameObject CreatePanel(GameObject parent, string name, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent.transform, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        if (color.a > 0)
        {
            Image image = panel.AddComponent<Image>();
            image.color = color;
        }

        return panel;
    }

    private TextMeshProUGUI CreateText(GameObject parent, string name, string text, 
        Vector2 anchorPos, Vector2 size, int fontSize, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.alignment = TextAlignmentOptions.Center;

        return textComponent;
    }

    private Button CreateButton(GameObject parent, string name, string text, 
        Vector2 anchorPos, Vector2 size, Color color, int fontSize)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;

        Image image = buttonObj.AddComponent<Image>();
        image.color = color;
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = image;

        // Button Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;

        return button;
    }

    private TMP_InputField CreateInputField(GameObject parent, string name, Vector2 anchorPos, Vector2 size)
    {
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent.transform, false);
        
        RectTransform rect = inputObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;

        Image image = inputObj.AddComponent<Image>();
        image.color = Color.white;
        
        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();

        // Input Field Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.fontSize = normalFontSize;
        textComponent.color = Color.black;
        textComponent.alignment = TextAlignmentOptions.Left;

        // Input Field Placeholder
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(inputObj.transform, false);
        
        RectTransform placeholderRect = placeholderObj.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.sizeDelta = Vector2.zero;
        placeholderRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
        placeholderText.text = "Enter your Sui wallet address (0x...)";
        placeholderText.fontSize = normalFontSize;
        placeholderText.color = Color.gray;
        placeholderText.alignment = TextAlignmentOptions.Left;

        inputField.textComponent = textComponent;
        inputField.placeholder = placeholderText;

        return inputField;
    }
}
