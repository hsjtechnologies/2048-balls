# 🐛 PRACTICAL DEBUGGING GUIDE FOR 2048-BALLS

## 🎯 **UNDERSTANDING THE SYSTEM IN LAYMAN'S TERMS**

Think of your 2048-balls game like a **restaurant with a VIP system**:

### **The Restaurant Analogy:**
- **GameManager** = The Restaurant Manager (controls everything)
- **SimpleSignInManager** = The Host (checks Twitter ID)
- **SuiWalletManager** = The VIP Card Reader (validates wallet)
- **Ball.cs** = The Waiters (serve the game experience)
- **UI Panels** = Different sections (Login Area, VIP Lounge, Dining Room)

**The Flow:**
1. Customer arrives → Host checks Twitter ID → VIP Card Reader validates wallet → Manager allows entry → Waiters serve food (game)

---

## 🔍 **STEP-BY-STEP DEBUGGING PROCESS**

### **STEP 1: DIAGNOSE THE PROBLEM**

**Ask yourself:**
- Is the game completely frozen? (Login issue)
- Is the game running but balls don't merge? (GameManager issue)
- Does login work but wallet fails? (Wallet issue)
- Does everything work but UI is broken? (UI issue)

### **STEP 2: CHECK THE CONSOLE**

**Look for these patterns:**

```
✅ GOOD SIGNS:
[SimpleSignInManager] Login successful: John Doe (@johndoe)
[SuiWalletManager] Wallet connected with address: 0x1234...
GameManager found successfully for ball: 2
Setting GameManager.IsLoggedIn = true
Setting Time.timeScale = 1f

❌ BAD SIGNS:
❌ SimpleSignInManager NOT FOUND in scene!
❌ SuiWalletManager NOT FOUND in scene!
❌ GameManager not found in scene! Ball will not function properly.
❌ Invalid Sui wallet address format
❌ Login failed: [error message]
```

### **STEP 3: USE THE DEBUGGING TOOLS**

**Tool 1: SetupVerifier**
```csharp
// Add this script to any GameObject, then:
// 1. Click "Verify Setup" in Inspector
// 2. Check Console for diagnostics
// 3. Use "Force Show Wallet Panel" if needed
// 4. Use "Test Manual Wallet Input" to test wallet
```

**Tool 2: Console Commands**
```csharp
// Paste these in Console or add to any script:

// Check if everything is working
Debug.Log($"=== SYSTEM STATUS ===");
Debug.Log($"GameManager exists: {FindObjectOfType<GameManager>() != null}");
Debug.Log($"SimpleSignInManager exists: {FindObjectOfType<SimpleSignInManager>() != null}");
Debug.Log($"SuiWalletManager exists: {SuiWalletManager.Instance != null}");
Debug.Log($"IsLoggedIn: {GameManager.IsLoggedIn}");
Debug.Log($"Time.timeScale: {Time.timeScale}");
Debug.Log($"=== END STATUS ===");
```

---

## 🚨 **COMMON PROBLEMS & SOLUTIONS**

### **PROBLEM 1: "Game Won't Start - Everything is Frozen"**

**What you see:** Balls don't move, game is completely paused

**What's happening:** The game is waiting for login but login failed

**How to fix:**

**Method A - Use Bypass (Quick Fix):**
```csharp
// In Console, type:
FindObjectOfType<TwitterOAuth>().BypassLoginAndStartGame();
```

**Method B - Check Login System:**
1. Look for `SimpleSignInManager` in scene
2. Check if UI references are assigned
3. Try clicking "Login with Twitter" button
4. Check Console for login errors

**Method C - Manual Login:**
```csharp
// In Console, type:
FindObjectOfType<TwitterOAuth>().CompleteBypassAndStartGame();
```

### **PROBLEM 2: "Login Works But Wallet Input Doesn't"**

**What you see:** Twitter login succeeds, but wallet panel doesn't work

**What's happening:** Wallet validation or UI issue

**How to fix:**

**Step 1 - Check Wallet Format:**
- Sui addresses must be: `0x` + exactly 64 hex characters = 66 total
- Example: `0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef`

**Step 2 - Test Manual Wallet:**
```csharp
// In Console, type:
SuiWalletManager.Instance.SetManualAddress("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
```

**Step 3 - Check UI References:**
- Use SetupVerifier to check if wallet UI is assigned
- Look for "walletAddressInput is NOT assigned!" errors

### **PROBLEM 3: "Balls Don't Merge When They Collide"**

**What you see:** Balls touch but don't combine into higher numbers

**What's happening:** GameManager not found or ball logic broken

**How to fix:**

**Step 1 - Check GameManager:**
```csharp
// In Console, type:
Debug.Log($"GameManager found: {FindObjectOfType<GameManager>() != null}");
```

**Step 2 - Check Ball Scripts:**
- Look for "GameManager not found in scene!" errors
- Verify each ball prefab has the `Ball.cs` script attached

**Step 3 - Check Ball Prefabs:**
- Open ball prefabs (2, 4, 8, 16, etc.)
- Make sure they have `Ball.cs` script
- Check if `isLocked` is false by default

### **PROBLEM 4: "UI Panels Don't Switch"**

**What you see:** Login panel stays visible, wallet panel never appears

**What's happening:** UI references not assigned or panels missing

**How to fix:**

**Step 1 - Use SetupVerifier:**
1. Add `SetupVerifier` script to any GameObject
2. Click "Verify Setup" in Inspector
3. Check Console for UI diagnostics

**Step 2 - Enable Auto-Create UI:**
```csharp
// In SimpleSignInManager Inspector:
autoCreateUIIfMissing = true
```

**Step 3 - Manual Panel Control:**
```csharp
// In Console, type:
var manager = FindObjectOfType<SimpleSignInManager>();
// Force show wallet panel
manager.GetType().GetField("walletPanel").SetValue(manager, true);
```

---

## 🛠 **QUICK FIXES FOR EMERGENCY DEBUGGING**

### **"I Need to Test the Game Right Now!"**
```csharp
// Paste this in Console:
FindObjectOfType<TwitterOAuth>().CompleteBypassAndStartGame();
```

### **"I Need to Test Wallet Connection!"**
```csharp
// Paste this in Console:
SuiWalletManager.Instance.SetManualAddress("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
Debug.Log($"Wallet connected: {SuiWalletManager.Instance.IsWalletConnected()}");
```

### **"I Need to Check Everything at Once!"**
```csharp
// Paste this in Console:
Debug.Log("=== FULL SYSTEM CHECK ===");
Debug.Log($"GameManager: {FindObjectOfType<GameManager>() != null}");
Debug.Log($"SimpleSignInManager: {FindObjectOfType<SimpleSignInManager>() != null}");
Debug.Log($"SuiWalletManager: {SuiWalletManager.Instance != null}");
Debug.Log($"IsLoggedIn: {GameManager.IsLoggedIn}");
Debug.Log($"Time.timeScale: {Time.timeScale}");
Debug.Log($"Canvas count: {FindObjectsOfType<Canvas>().Length}");
Debug.Log($"Button count: {FindObjectsOfType<Button>().Length}");
Debug.Log("=== END CHECK ===");
```

---

## 📋 **DEBUGGING CHECKLIST**

### **Before You Start Debugging:**
- [ ] Open Console window (Window → General → Console)
- [ ] Clear Console (right-click → Clear)
- [ ] Play the scene
- [ ] Look for red error messages

### **For Login Issues:**
- [ ] Check if `SimpleSignInManager` exists in scene
- [ ] Try bypass login methods
- [ ] Check Console for "[SimpleSignInManager]" messages
- [ ] Verify UI button assignments

### **For Wallet Issues:**
- [ ] Check wallet address format (0x + 64 chars)
- [ ] Test manual wallet input
- [ ] Check `SuiWalletManager.Instance`
- [ ] Verify wallet UI references

### **For Game Issues:**
- [ ] Check `GameManager.IsLoggedIn` is true
- [ ] Check `Time.timeScale` is 1.0
- [ ] Verify GameManager exists in scene
- [ ] Check ball prefab configurations

### **For UI Issues:**
- [ ] Run SetupVerifier diagnostics
- [ ] Check UI reference assignments
- [ ] Enable auto-create UI
- [ ] Verify Canvas exists

---

## 🎯 **UNDERSTANDING ERROR MESSAGES**

### **Common Error Patterns:**

**"NOT FOUND in scene!"**
- **Meaning:** Required component is missing
- **Fix:** Add the missing script to a GameObject

**"is NOT assigned!"**
- **Meaning:** UI reference is missing
- **Fix:** Assign the UI element in Inspector or enable auto-create

**"Invalid Sui wallet address"**
- **Meaning:** Wallet format is wrong
- **Fix:** Use format: 0x + exactly 64 hex characters

**"GameManager not found"**
- **Meaning:** Ball can't find GameManager
- **Fix:** Make sure GameManager exists in scene

**"Login failed"**
- **Meaning:** Twitter authentication failed
- **Fix:** Use bypass methods for testing

---

## 🚀 **PRO TIPS FOR DEBUGGING**

1. **Always check Console first** - 90% of issues show up there
2. **Use bypass methods** - Skip authentication when testing game logic
3. **Test in WebGL build** - Some features only work in builds, not Editor
4. **Use SetupVerifier** - It's your best friend for diagnosing setup issues
5. **Check one thing at a time** - Don't try to fix everything at once
6. **Save your work** - Before making changes, save the scene
7. **Use version control** - Commit working states so you can revert

---

## 🆘 **EMERGENCY RECOVERY**

**If everything is broken:**

1. **Reset Game State:**
```csharp
GameManager.IsLoggedIn = false;
Time.timeScale = 0f;
```

2. **Clear Saved Data:**
```csharp
PlayerPrefs.DeleteAll();
```

3. **Restart Scene:**
```csharp
UnityEngine.SceneManagement.SceneManager.LoadScene(0);
```

4. **Use Complete Bypass:**
```csharp
FindObjectOfType<TwitterOAuth>().CompleteBypassAndStartGame();
```

Remember: **The game is designed to be robust** - if one system fails, there are usually backup methods to get it working!
