# 2048-Balls Unity Project Logic Flow Guide

## 🎯 **PROJECT OVERVIEW**
This is a Unity WebGL game that combines:
- **2048-style ball merging gameplay**
- **Twitter/X authentication** (via SimpleSignIn plugin)
- **Sui blockchain wallet integration**
- **Google Sheets score logging**

---

## 🔄 **MAIN LOGIC FLOW**

```
START GAME
    ↓
GameManager.Start() checks IsLoggedIn
    ↓
If NOT logged in → Time.timeScale = 0 (PAUSE GAME)
    ↓
Show Login Panel (TwitterOAuth/SimpleSignInManager)
    ↓
User clicks "Login with Twitter"
    ↓
SimpleSignIn XAuth.SignIn() → Twitter OAuth flow
    ↓
OnSignInCallback() → Success?
    ↓ YES
Show Wallet Panel → User enters Sui wallet address
    ↓
OnConfirmWalletClick() → Validate address
    ↓
CompleteLogin() → Set IsLoggedIn = true, Time.timeScale = 1
    ↓
GAME STARTS → Ball physics active
    ↓
Ball merging → Score tracking → Level progression
    ↓
Game Over → Log score to Google Sheets
```

---

## 🧩 **KEY COMPONENTS BREAKDOWN**

### 1. **GameManager.cs** - The Central Controller
**What it does:**
- Controls game pause/resume based on login status
- Manages score, level progression, and ball merging
- Listens to login events from authentication systems
- Handles game over and score logging

**Key Variables:**
- `IsLoggedIn` (static) - Controls if game is playable
- `Time.timeScale` - 0 = paused, 1 = running
- `score`, `level`, `remainingForNextLevel` - Game progression

**Debug Points:**
- Check Console for "Game paused - waiting for user login"
- Verify `IsLoggedIn` is true after login
- Check `Time.timeScale` is 1.0 when game should be running

### 2. **SimpleSignInManager.cs** - Modern Twitter Auth
**What it does:**
- Handles Twitter/X authentication using SimpleSignIn plugin
- Manages UI panels (Login → Wallet → Game)
- Integrates with SuiWalletManager for wallet connection
- Saves user data to PlayerPrefs

**Key Flow:**
1. `OnLoginButtonClick()` → Start Twitter OAuth
2. `OnSignInCallback()` → Handle Twitter response
3. `TryConnectSuiWallet()` → Auto-connect wallet
4. `OnConfirmWalletClick()` → Validate and save wallet
5. `CompleteLogin()` → Enable game

**Debug Points:**
- Check Console for "[SimpleSignInManager]" messages
- Verify UI panels are switching correctly
- Check if wallet address validation is working

### 3. **TwitterOAuth.cs** - Legacy Twitter Auth (Backup)
**What it does:**
- Alternative Twitter authentication system
- Has bypass methods for testing
- Manages UI creation at runtime if missing
- Handles backend communication

**Key Methods:**
- `BypassLoginAndStartGame()` - Skip auth for testing
- `CompleteBypassAndStartGame()` - Start game immediately
- `ManualLoginSuccess()` - Manual login for testing

**Debug Points:**
- Use bypass methods if SimpleSignIn fails
- Check backend URL connectivity
- Verify UI auto-creation is working

### 4. **SuiWalletManager.cs** - Blockchain Integration
**What it does:**
- Manages Sui wallet connection (browser extension)
- Validates Sui wallet addresses (0x + 64 hex chars)
- Provides manual address input fallback
- Singleton pattern for global access

**Key Features:**
- WebGL JavaScript integration
- Address validation (66 characters total)
- Manual input fallback
- Event system for connection status

**Debug Points:**
- Check if Sui Wallet extension is installed
- Verify address format validation
- Test manual address input

### 5. **Ball.cs** - Game Physics & Merging
**What it does:**
- Handles ball collision detection
- Manages ball merging logic (2+2=4, 4+4=8, etc.)
- Finds GameManager for score updates
- Handles game over detection

**Key Logic:**
- `OnTriggerStay()` - Detect same-number ball collisions
- `instantiateNew()` - Create next-level ball
- `GM.Merging()` - Update score in GameManager

**Debug Points:**
- Check if GameManager is found
- Verify ball collision detection
- Check ball instantiation logic

---

## 🔍 **DEBUGGING STRATEGIES**

### **Step 1: Check Console Messages**
Look for these key log patterns:
```
[SimpleSignInManager] - Twitter auth messages
[SuiWalletManager] - Wallet connection messages
[TwitterOAuth] - Legacy auth messages
GameManager - Game state messages
```

### **Step 2: Verify Component Setup**
Use `SetupVerifier.cs` script:
1. Add SetupVerifier to any GameObject
2. Click "Verify Setup" in Inspector
3. Check Console for setup diagnostics

### **Step 3: Test Login Flow**
**Method 1 - SimpleSignIn (Recommended):**
1. Click "Login with Twitter" button
2. Complete Twitter OAuth
3. Enter Sui wallet address
4. Click "Confirm Wallet"

**Method 2 - Bypass (Testing):**
```csharp
// In Console or Inspector
FindObjectOfType<TwitterOAuth>().BypassLoginAndStartGame();
// OR
FindObjectOfType<TwitterOAuth>().CompleteBypassAndStartGame();
```

### **Step 4: Check Game State**
Verify these values in Console:
```csharp
Debug.Log($"IsLoggedIn: {GameManager.IsLoggedIn}");
Debug.Log($"Time.timeScale: {Time.timeScale}");
Debug.Log($"GameManager found: {FindObjectOfType<GameManager>() != null}");
```

---

## 🚨 **COMMON ISSUES & SOLUTIONS**

### **Issue 1: Game Won't Start**
**Symptoms:** Game is paused, balls don't move
**Causes:**
- `IsLoggedIn = false`
- `Time.timeScale = 0`
- GameManager not found

**Solutions:**
1. Check Console for login status
2. Use bypass login for testing
3. Verify GameManager exists in scene

### **Issue 2: Twitter Login Fails**
**Symptoms:** Login button does nothing, OAuth errors
**Causes:**
- SimpleSignIn plugin not configured
- Backend server down
- Network connectivity issues

**Solutions:**
1. Use `TwitterOAuth.BypassLoginAndStartGame()`
2. Check SimpleSignIn plugin setup
3. Test with manual login methods

### **Issue 3: Wallet Connection Fails**
**Symptoms:** Wallet input not working, validation errors
**Causes:**
- Sui Wallet extension not installed
- Invalid address format
- SuiWalletManager not in scene

**Solutions:**
1. Use manual address input
2. Check address format (0x + 64 hex chars)
3. Verify SuiWalletManager exists

### **Issue 4: Ball Merging Not Working**
**Symptoms:** Balls collide but don't merge
**Causes:**
- GameManager not found by Ball script
- Ball prefabs not configured correctly
- Physics settings incorrect

**Solutions:**
1. Check Console for "GameManager not found"
2. Verify ball prefabs have correct Ball script
3. Check physics material settings

### **Issue 5: UI Panels Not Switching**
**Symptoms:** Login panel stays visible, wallet panel doesn't show
**Causes:**
- UI references not assigned in Inspector
- Panel GameObjects not found
- Script execution order issues

**Solutions:**
1. Use SetupVerifier to check UI setup
2. Enable `autoCreateUIIfMissing = true`
3. Manually assign UI references in Inspector

---

## 🛠 **TROUBLESHOOTING CHECKLIST**

### **Before Debugging:**
- [ ] Check Console for error messages
- [ ] Verify all required scripts are in scene
- [ ] Test in WebGL build (not just Editor)

### **Login Issues:**
- [ ] Try bypass login methods
- [ ] Check SimpleSignIn plugin configuration
- [ ] Verify backend server is running
- [ ] Test manual login methods

### **Wallet Issues:**
- [ ] Check Sui Wallet extension installation
- [ ] Verify address format (0x + 64 characters)
- [ ] Test manual address input
- [ ] Check SuiWalletManager.Instance

### **Game Issues:**
- [ ] Verify `GameManager.IsLoggedIn = true`
- [ ] Check `Time.timeScale = 1.0`
- [ ] Confirm GameManager exists in scene
- [ ] Test ball physics and collision

### **UI Issues:**
- [ ] Run SetupVerifier diagnostics
- [ ] Check UI reference assignments
- [ ] Enable auto-create UI if missing
- [ ] Verify Canvas and UI components exist

---

## 📋 **QUICK DEBUG COMMANDS**

Add these to any script for quick debugging:

```csharp
// Check login status
Debug.Log($"Login Status: {GameManager.IsLoggedIn}");

// Check game state
Debug.Log($"Time Scale: {Time.timeScale}");

// Check managers
Debug.Log($"GameManager: {FindObjectOfType<GameManager>() != null}");
Debug.Log($"SimpleSignInManager: {FindObjectOfType<SimpleSignInManager>() != null}");
Debug.Log($"SuiWalletManager: {SuiWalletManager.Instance != null}");

// Force login (for testing)
FindObjectOfType<TwitterOAuth>()?.BypassLoginAndStartGame();

// Check wallet connection
if (SuiWalletManager.Instance != null)
    Debug.Log($"Wallet Connected: {SuiWalletManager.Instance.IsWalletConnected()}");
```

---

## 🎯 **KEY TAKEAWAYS**

1. **Game State Control:** `GameManager.IsLoggedIn` and `Time.timeScale` control everything
2. **Two Auth Systems:** SimpleSignInManager (new) and TwitterOAuth (legacy/backup)
3. **Debugging Tools:** SetupVerifier, Console logs, and bypass methods
4. **UI Dependencies:** All UI references must be assigned or auto-created
5. **WebGL Specific:** Sui Wallet integration only works in WebGL builds

Remember: When in doubt, use the bypass methods to test the game functionality independently of the authentication systems!
