# Quick Fix Checklist - Sui Wallet Not Showing

## Step 1: Fix the Typo (DONE ✅)
The typo in `SuiWalletManager.cs` has been fixed.

## Step 2: Verify Scene Setup

### A. Check GameObject Hierarchy

Open Unity and verify these GameObjects exist in your scene:

```
✅ 1. GameObject with "SimpleSignInManager" script
✅ 2. GameObject with "SuiWalletManager" script
✅ 3. Canvas with UI elements
```

**To create missing GameObjects:**
1. Right-click in Hierarchy → Create Empty
2. Rename to "SimpleSignInManager" or "SuiWalletManager"
3. Drag the corresponding script from Assets/Scripts onto it

### B. Verify SimpleSignInManager Setup

1. Select the GameObject with `SimpleSignInManager` in Hierarchy
2. Look at the Inspector panel
3. Check these fields are assigned:

```
UI References:
✅ Login Button         → Drag your login button here
✅ Login Panel          → Drag the panel/canvas containing login button
✅ Wallet Panel         → Drag the panel/canvas for wallet input
✅ Game Panel           → (Optional) Your game UI
✅ Status Text          → TextMeshProUGUI for messages
✅ Username Text        → TextMeshProUGUI for username
✅ Wallet Address Input → TMP_InputField for wallet address
✅ Confirm Wallet Button → Button to confirm wallet

Settings:
✅ Debug Mode → Check this box (enabled)
```

### C. Verify SuiWalletManager Setup

1. Select the GameObject with `SuiWalletManager` in Hierarchy
2. Look at the Inspector panel
3. Check these settings:

```
Settings:
✅ Debug Mode → Check this box (enabled)
✅ Allow Manual Input → Check this box (enabled)
```

## Step 3: Use the Setup Verifier Tool

1. In Unity, create an empty GameObject: Right-click Hierarchy → Create Empty
2. Rename it to "SetupVerifier"
3. Drag `Assets/Scripts/SetupVerifier.cs` onto it
4. Right-click on the SetupVerifier component → "Verify Setup"
5. Check the Console for any ❌ errors

## Step 4: Test the Flow

1. Press Play in Unity
2. Click the Login with Twitter button
3. Watch the Console (Window → General → Console)

**Expected flow:**
```
1. Click login button
2. Console shows: "[SimpleSignInManager] Login button clicked"
3. Twitter login opens in browser
4. After login, Console shows: "[SimpleSignInManager] Login successful"
5. Console shows: "[SimpleSignInManager] Connecting to Sui Wallet..."
6. Console shows: "[SuiWalletManager] Attempting to connect Sui Wallet..."
7. Wallet panel should appear
```

## Step 5: Common Issues

### Issue: "SuiWalletManager not found in scene!"

**Fix:**
1. Create empty GameObject
2. Add SuiWalletManager script to it
3. Make sure it's in the scene (not a prefab)

### Issue: Nothing happens after Twitter login

**Fix:**
1. Check if "Wallet Panel" is assigned in SimpleSignInManager Inspector
2. If not, create a panel with:
   - TMP_InputField (for wallet address)
   - Button (for confirm)
   - Assign these in SimpleSignInManager

### Issue: Wallet panel doesn't show

**Fix:**
1. Select SimpleSignInManager GameObject
2. In Inspector, find "Wallet Panel" field
3. Drag your wallet canvas/panel from Hierarchy
4. Make sure the panel has these components:
   - TMP_InputField
   - Button

## Step 6: Manual Testing

If automatic connection doesn't work, test manually:

1. After Twitter login, wallet panel should show
2. Enter this test address:
   ```
   0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef
   ```
3. Click Confirm button
4. Game should start

## Step 7: Share Debug Info

If still not working, please share:

1. **Screenshot of:**
   - Unity Hierarchy window
   - SimpleSignInManager Inspector (all fields visible)
   - SuiWalletManager Inspector

2. **Console messages:**
   - Copy all messages starting with `[SimpleSignInManager]`
   - Copy all messages starting with `[SuiWalletManager]`

3. **What happens:**
   - Does Twitter login complete? (Yes/No)
   - Does anything appear after login? (Describe)
   - Do you see any red errors in Console? (Copy them)

---

## Need Help Setting Up UI?

If you don't have a Wallet Panel yet:

1. Right-click on your Canvas → UI → Panel
2. Rename it to "WalletPanel"
3. Right-click on WalletPanel → UI → InputField - TextMeshPro
4. Rename it to "WalletAddressInput"
5. Right-click on WalletPanel → UI → Button - TextMeshPro
6. Rename it to "ConfirmWalletButton"
7. Change button text to "Confirm"
8. Now assign these in SimpleSignInManager Inspector:
   - Wallet Panel → WalletPanel
   - Wallet Address Input → WalletAddressInput
   - Confirm Wallet Button → ConfirmWalletButton



