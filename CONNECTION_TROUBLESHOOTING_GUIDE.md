# 🔌 CONNECTION ISSUES TROUBLESHOOTING GUIDE

## 🎯 **UNDERSTANDING CONNECTIONS IN YOUR PROJECT**

Your 2048-balls game has **3 main connection types**:

1. **Twitter/X Authentication** (SimpleSignIn plugin)
2. **Sui Wallet Connection** (Browser extension)
3. **Backend Server Communication** (Score logging)

Think of these like **3 different phone lines** - if any one fails, the system can still work with fallbacks.

---

## 🔍 **CONNECTION DIAGNOSIS FLOWCHART**

```
CONNECTION PROBLEM DETECTED
    ↓
Which connection is failing?
    ↓
┌─────────────────┬─────────────────┬─────────────────┐
│   TWITTER       │   SUI WALLET    │   BACKEND       │
│   AUTH          │   CONNECTION    │   SERVER        │
└─────────────────┴─────────────────┴─────────────────┘
    ↓                     ↓                     ↓
Check Console for:    Check Console for:    Check Console for:
- Login errors        - Wallet errors        - Network errors
- OAuth failures     - Extension missing    - Server down
- Plugin issues      - Address invalid      - Timeout errors
    ↓                     ↓                     ↓
Use bypass methods    Use manual input      Use offline mode
```

---

## 🐦 **TWITTER/X AUTHENTICATION ISSUES**

### **Problem: "Twitter Login Button Does Nothing"**

**Symptoms:**
- Clicking login button has no effect
- No Console messages about Twitter
- Login panel stays visible

**Root Causes:**
1. SimpleSignIn plugin not properly configured
2. UI button not connected to script
3. Script not found in scene

**Step-by-Step Fix:**

**Step 1 - Check Console:**
```csharp
// Look for these messages:
[SimpleSignInManager] Login button clicked - starting SimpleSignIn flow
[SimpleSignInManager] Login successful: [username]
```

**Step 2 - Verify SimpleSignInManager:**
```csharp
// In Console, check:
Debug.Log($"SimpleSignInManager found: {FindObjectOfType<SimpleSignInManager>() != null}");
```

**Step 3 - Check Button Connection:**
```csharp
// In Inspector, verify:
// 1. Login button is assigned to SimpleSignInManager
// 2. OnClick() event calls OnLoginButtonClick()
```

**Step 4 - Use Bypass Method:**
```csharp
// If Twitter auth fails, use this:
FindObjectOfType<TwitterOAuth>().BypassLoginAndStartGame();
```

### **Problem: "Twitter OAuth Callback Fails"**

**Symptoms:**
- Twitter login opens but doesn't return to game
- Console shows "OAuth callback timeout"
- User gets stuck on Twitter page

**Root Causes:**
1. Backend server not running
2. Callback URL not configured
3. Network connectivity issues

**Step-by-Step Fix:**

**Step 1 - Check Backend Status:**
```csharp
// Look for these Console messages:
"Backend is working, proceeding with Twitter login"
"Backend is not working, offering bypass option"
```

**Step 2 - Use Manual Login:**
```csharp
// If callback fails, use this:
FindObjectOfType<TwitterOAuth>().ManualLoginSuccess("testuser", "test_token");
```

**Step 3 - Complete Bypass:**
```csharp
// Skip Twitter entirely:
FindObjectOfType<TwitterOAuth>().CompleteBypassAndStartGame();
```

---

## 💰 **SUI WALLET CONNECTION ISSUES**

### **Problem: "Sui Wallet Extension Not Found"**

**Symptoms:**
- Console shows "Sui Wallet not installed"
- Wallet connection fails immediately
- No wallet popup appears

**Root Causes:**
1. Sui Wallet browser extension not installed
2. Extension not enabled
3. Browser compatibility issues

**Step-by-Step Fix:**

**Step 1 - Check Extension Installation:**
```csharp
// In Console, check:
Debug.Log($"Sui Wallet available: {SuiWalletManager.Instance.IsSuiWalletAvailable()}");
```

**Step 2 - Install Sui Wallet:**
- Go to Chrome Web Store
- Search "Sui Wallet"
- Install the official extension
- Refresh the game page

**Step 3 - Use Manual Input:**
```csharp
// If extension fails, use manual input:
SuiWalletManager.Instance.SetManualAddress("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
```

### **Problem: "Invalid Wallet Address Format"**

**Symptoms:**
- Console shows "Invalid Sui wallet address format"
- Confirm button stays disabled
- Address validation fails

**Root Causes:**
1. Wrong address format
2. Missing or extra characters
3. Non-hex characters in address

**Step-by-Step Fix:**

**Step 1 - Check Address Format:**
```csharp
// Valid Sui address format:
// - Must start with "0x"
// - Must be exactly 66 characters total
// - Must contain only hex characters (0-9, a-f, A-F)
// Example: 0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef
```

**Step 2 - Validate Address:**
```csharp
// Test address validation:
string testAddress = "0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef";
Debug.Log($"Address valid: {SuiWalletManager.Instance.IsValidSuiAddress(testAddress)}");
```

**Step 3 - Use Test Address:**
```csharp
// Use this valid test address:
SuiWalletManager.Instance.SetManualAddress("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
```

### **Problem: "Wallet Connection Timeout"**

**Symptoms:**
- Wallet popup appears but never completes
- Console shows connection attempts but no success
- User gets stuck waiting

**Root Causes:**
1. User cancels wallet connection
2. Extension has permission issues
3. Network connectivity problems

**Step-by-Step Fix:**

**Step 1 - Check Connection Status:**
```csharp
// In Console, check:
Debug.Log($"Wallet connected: {SuiWalletManager.Instance.IsWalletConnected()}");
Debug.Log($"Wallet address: {SuiWalletManager.Instance.GetConnectedAddress()}");
```

**Step 2 - Force Manual Input:**
```csharp
// If connection times out, use manual input:
FindObjectOfType<SimpleSignInManager>().ForceShowWalletPanel();
```

**Step 3 - Reset Wallet State:**
```csharp
// Clear wallet state and try again:
SuiWalletManager.Instance.DisconnectWallet();
// Then try manual input
```

---

## 🌐 **BACKEND SERVER CONNECTION ISSUES**

### **Problem: "Backend Server Not Responding"**

**Symptoms:**
- Console shows "Backend is not working"
- Network timeout errors
- Score logging fails

**Root Causes:**
1. Backend server is down
2. Wrong server URL configured
3. Network connectivity issues

**Step-by-Step Fix:**

**Step 1 - Check Server URL:**
```csharp
// In TwitterOAuth Inspector, verify:
backendURL = "https://ball-game-hlvu.onrender.com"
```

**Step 2 - Test Server Connection:**
```csharp
// Look for these Console messages:
"Backend is working, proceeding with Twitter login"
"Backend is not working, offering bypass option"
```

**Step 3 - Use Offline Mode:**
```csharp
// If backend is down, use bypass:
FindObjectOfType<TwitterOAuth>().BypassLoginAndStartGame();
```

### **Problem: "Score Logging Fails"**

**Symptoms:**
- Game works but scores don't save
- Console shows "Failed to log score"
- Google Sheets integration broken

**Root Causes:**
1. Google Sheets webhook not configured
2. Network connectivity issues
3. Invalid score data format

**Step-by-Step Fix:**

**Step 1 - Check Webhook URL:**
```csharp
// In SimpleSignInManager Inspector, verify:
googleSheetsWebhookURL = "[your Google Apps Script webhook URL]"
```

**Step 2 - Test Score Logging:**
```csharp
// Manually test score logging:
FindObjectOfType<SimpleSignInManager>().LogScoreToDatabase(1000, 1);
```

**Step 3 - Check Console for Errors:**
```csharp
// Look for:
"Score logged successfully to Google Sheets"
"Failed to log score: [error message]"
```

---

## 🛠 **CONNECTION TESTING TOOLS**

### **Tool 1: Connection Status Checker**
```csharp
// Add this to any script for comprehensive connection testing:

public void TestAllConnections()
{
    Debug.Log("=== CONNECTION STATUS CHECK ===");
    
    // Check Twitter Auth
    var simpleSignIn = FindObjectOfType<SimpleSignInManager>();
    Debug.Log($"SimpleSignInManager: {simpleSignIn != null}");
    
    // Check Sui Wallet
    var suiWallet = SuiWalletManager.Instance;
    Debug.Log($"SuiWalletManager: {suiWallet != null}");
    Debug.Log($"Wallet Available: {suiWallet?.IsSuiWalletAvailable()}");
    Debug.Log($"Wallet Connected: {suiWallet?.IsWalletConnected()}");
    
    // Check Game State
    Debug.Log($"Game Logged In: {GameManager.IsLoggedIn}");
    Debug.Log($"Time Scale: {Time.timeScale}");
    
    // Check UI
    Debug.Log($"Canvas Count: {FindObjectsOfType<Canvas>().Length}");
    Debug.Log($"Button Count: {FindObjectsOfType<Button>().Length}");
    
    Debug.Log("=== END CONNECTION CHECK ===");
}
```

### **Tool 2: Manual Connection Override**
```csharp
// Use this to force all connections for testing:

public void ForceAllConnections()
{
    Debug.Log("=== FORCING ALL CONNECTIONS ===");
    
    // Force Twitter login
    FindObjectOfType<TwitterOAuth>().CompleteBypassAndStartGame();
    
    // Force wallet connection
    SuiWalletManager.Instance.SetManualAddress("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
    
    // Force game state
    GameManager.IsLoggedIn = true;
    Time.timeScale = 1f;
    
    Debug.Log("=== ALL CONNECTIONS FORCED ===");
}
```

### **Tool 3: Connection Reset**
```csharp
// Use this to reset all connections:

public void ResetAllConnections()
{
    Debug.Log("=== RESETTING ALL CONNECTIONS ===");
    
    // Reset game state
    GameManager.IsLoggedIn = false;
    Time.timeScale = 0f;
    
    // Disconnect wallet
    SuiWalletManager.Instance?.DisconnectWallet();
    
    // Clear saved data
    PlayerPrefs.DeleteAll();
    
    // Reset UI
    var simpleSignIn = FindObjectOfType<SimpleSignInManager>();
    if (simpleSignIn != null)
    {
        // Force show login panel
        simpleSignIn.GetType().GetMethod("ShowLoginPanel", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(simpleSignIn, null);
    }
    
    Debug.Log("=== ALL CONNECTIONS RESET ===");
}
```

---

## 🚨 **EMERGENCY CONNECTION RECOVERY**

### **"Everything is Broken - I Need to Test the Game!"**

**Step 1 - Complete Bypass:**
```csharp
// Paste this in Console:
FindObjectOfType<TwitterOAuth>().CompleteBypassAndStartGame();
```

**Step 2 - Verify Game Works:**
```csharp
// Check these values:
Debug.Log($"IsLoggedIn: {GameManager.IsLoggedIn}"); // Should be true
Debug.Log($"Time.timeScale: {Time.timeScale}"); // Should be 1.0
```

**Step 3 - Test Ball Merging:**
- Spawn some balls
- Check if they merge when they collide
- Verify score updates

### **"Twitter Works But Wallet Fails"**

**Step 1 - Manual Wallet Input:**
```csharp
// Use this valid test address:
SuiWalletManager.Instance.SetManualAddress("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
```

**Step 2 - Force Wallet Confirmation:**
```csharp
// If wallet input doesn't work, force it:
var manager = FindObjectOfType<SimpleSignInManager>();
manager.GetType().GetMethod("OnConfirmWalletClick", 
    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
    ?.Invoke(manager, null);
```

### **"Wallet Works But Twitter Fails"**

**Step 1 - Bypass Twitter:**
```csharp
// Skip Twitter authentication:
FindObjectOfType<TwitterOAuth>().BypassLoginAndStartGame();
```

**Step 2 - Manual Login:**
```csharp
// Set fake Twitter credentials:
FindObjectOfType<TwitterOAuth>().ManualLoginSuccess("testuser", "test_token");
```

---

## 📋 **CONNECTION TROUBLESHOOTING CHECKLIST**

### **Before Debugging Connections:**
- [ ] Open Console window
- [ ] Clear Console messages
- [ ] Check if you're testing in WebGL build (not just Editor)
- [ ] Verify all required scripts are in scene

### **For Twitter Connection Issues:**
- [ ] Check if SimpleSignIn plugin is properly configured
- [ ] Verify login button is connected to script
- [ ] Try bypass login methods
- [ ] Check Console for "[SimpleSignInManager]" messages

### **For Wallet Connection Issues:**
- [ ] Check if Sui Wallet extension is installed
- [ ] Verify wallet address format (0x + 64 hex chars)
- [ ] Test manual wallet input
- [ ] Check Console for "[SuiWalletManager]" messages

### **For Backend Connection Issues:**
- [ ] Check if backend server is running
- [ ] Verify server URL is correct
- [ ] Try offline/bypass modes
- [ ] Check Console for network errors

### **For UI Connection Issues:**
- [ ] Run SetupVerifier diagnostics
- [ ] Check UI reference assignments
- [ ] Enable auto-create UI if missing
- [ ] Verify Canvas and UI components exist

---

## 🎯 **KEY TAKEAWAYS FOR CONNECTION ISSUES**

1. **Always check Console first** - Connection errors show up there
2. **Use bypass methods** - They're designed for when connections fail
3. **Test one connection at a time** - Don't try to fix everything at once
4. **Have fallback methods** - Manual input, bypass login, offline mode
5. **Test in WebGL build** - Some connections only work in builds
6. **Use the debugging tools** - SetupVerifier, Console commands, manual overrides

**Remember:** Your game is designed to be **resilient** - if one connection fails, there are usually multiple ways to get it working!
