# Sui Wallet Integration Debugging Guide

## Issue: Sui Wallet Panel Not Showing After Twitter Login

### Quick Diagnosis Steps

1. **Check Unity Console for Errors**
   - Open Unity Console (Window → General → Console)
   - Look for ANY red error messages
   - Look for messages starting with `[SimpleSignInManager]` or `[SuiWalletManager]`

2. **Verify Scene Setup**
   - Make sure you have these GameObjects in your scene:
     - A GameObject with `SimpleSignInManager` script attached
     - A GameObject with `SuiWalletManager` script attached
     - Canvas with proper UI elements

3. **Check SimpleSignInManager Inspector**
   Make sure these fields are assigned:
   - ✅ Login Button
   - ✅ Login Panel (the panel containing the login button)
   - ✅ Wallet Panel (the panel with wallet input field)
   - ✅ Game Panel (optional - your main game UI)
   - ✅ Status Text (TextMeshProUGUI for messages)
   - ✅ Username Text (TextMeshProUGUI to show Twitter username)
   - ✅ Wallet Address Input (TMP_InputField for wallet address)
   - ✅ Confirm Wallet Button (Button to confirm wallet)

4. **Check SuiWalletManager Inspector**
   - ✅ Debug Mode should be enabled (checkbox)
   - ✅ Allow Manual Input should be enabled (checkbox)

### Common Issues & Solutions

#### Issue 1: Wallet Panel Not Assigned
**Symptom:** After Twitter login, nothing happens
**Solution:**
1. Select the GameObject with `SimpleSignInManager`
2. In Inspector, find the "Wallet Panel" field
3. Drag your wallet canvas/panel from the Hierarchy

#### Issue 2: SuiWalletManager Not in Scene
**Symptom:** Console shows "SuiWalletManager not found in scene!"
**Solution:**
1. Create an empty GameObject: Right-click in Hierarchy → Create Empty
2. Rename it to "SuiWalletManager"
3. Drag the `SuiWalletManager.cs` script onto it
4. Make sure "Debug Mode" is checked in Inspector

#### Issue 3: UI Elements Not Assigned
**Symptom:** Wallet panel exists but doesn't show
**Solution:**
Check all UI references in `SimpleSignInManager`:
```
LoginPanel     → Your login canvas/panel
WalletPanel    → Your wallet input canvas/panel
GamePanel      → Your game UI (optional)
StatusText     → A TextMeshProUGUI component
UsernameText   → A TextMeshProUGUI component
WalletAddressInput → A TMP_InputField
ConfirmWalletButton → A Button
```

#### Issue 4: Wallet Panel is Disabled
**Symptom:** Login works but wallet panel never appears
**Solution:**
1. Find your Wallet Panel in the Hierarchy
2. Make sure it has at least:
   - A TMP_InputField for wallet address
   - A Button for confirming
3. The panel can start as inactive (disabled) - the script will enable it

### Testing in Unity Editor

1. **Enable Debug Mode**
   - Select GameObject with `SimpleSignInManager`
   - Check "Debug Mode" in Inspector
   - Select GameObject with `SuiWalletManager`
   - Check "Debug Mode" in Inspector

2. **Run the Game**
   - Press Play
   - Click the Twitter login button
   - Watch the Unity Console

3. **Expected Console Messages:**
   ```
   [SimpleSignInManager] XAuth initialized successfully
   [SimpleSignInManager] Login button clicked - starting SimpleSignIn flow
   [SimpleSignInManager] Login successful: UserName (@username)
   [SimpleSignInManager] Status: Connecting to Sui Wallet...
   [SuiWalletManager] Attempting to connect Sui Wallet...
   [SuiWalletManager] Running in Editor - simulating wallet connection
   [SuiWalletManager] Sui Wallet connected: 0x1234...
   [SimpleSignInManager] Sui Wallet connected with address: 0x1234...
   ```

### Manual Testing Setup

If automatic Sui wallet connection doesn't work, you can test manually:

1. After Twitter login, the wallet panel should show
2. Manually enter a test Sui wallet address:
   ```
   0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef
   ```
3. Click the confirm button

### Scene Hierarchy Example

Your scene should look something like this:

```
Scene
├── Canvas (UI)
│   ├── LoginPanel (GameObject)
│   │   ├── LoginButton (Button)
│   │   └── StatusText (TextMeshProUGUI)
│   │
│   ├── WalletPanel (GameObject)
│   │   ├── UsernameText (TextMeshProUGUI)
│   │   ├── WalletAddressInput (TMP_InputField)
│   │   ├── ConfirmButton (Button)
│   │   └── StatusText (TextMeshProUGUI)
│   │
│   └── GamePanel (GameObject) [optional]
│       └── [Your game UI]
│
├── SimpleSignInManager (GameObject)
│   └── SimpleSignInManager (Script)
│
├── SuiWalletManager (GameObject)
│   └── SuiWalletManager (Script)
│
└── [Other game objects...]
```

### Debug Checklist

Before asking for help, please check:

- [ ] Unity Console is clear of red errors
- [ ] `SimpleSignInManager` GameObject exists in scene
- [ ] `SuiWalletManager` GameObject exists in scene
- [ ] All UI references are assigned in `SimpleSignInManager` Inspector
- [ ] Debug Mode is enabled on both managers
- [ ] Wallet Panel has a TMP_InputField and Button
- [ ] XAuthSettings asset is configured (Assets/SimpleSignIn/X/Resources/XAuthSettings)
- [ ] You can see debug messages in Console after clicking login

### WebGL-Specific Notes

If building for WebGL:
1. The Sui Wallet integration requires the browser extension
2. In Unity Editor, it will simulate a connection automatically
3. Make sure `Assets/Plugins/JavaScript/SuiWallet.jslib` exists
4. The jslib file must be in a folder called "Plugins/JavaScript"

### Still Not Working?

If the wallet panel still doesn't show:

1. Take a screenshot of:
   - Your Hierarchy window
   - `SimpleSignInManager` Inspector with all fields visible
   - `SuiWalletManager` Inspector
   - Unity Console after clicking login

2. Copy all console messages that start with:
   - `[SimpleSignInManager]`
   - `[SuiWalletManager]`
   - `[XAuth]`

3. Note exactly what happens:
   - Does Twitter login complete?
   - Does the login button disappear?
   - Do you see any status text changes?
   - Does the wallet panel appear at all?



