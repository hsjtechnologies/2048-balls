# 🎯 Sui Wallet Integration Guide

## Overview

This guide shows you how to integrate **Sui Wallet** into your Unity 2048 Balls game after Twitter login. Based on the [official Sui documentation](https://docs.sui.io/guides/developer/app-examples/coin-flip), this integration enables proper blockchain connectivity.

---

## 📋 **What is Sui Wallet?**

**Sui Wallet** is a browser extension (like MetaMask for Ethereum) that allows users to:
- Store and manage their Sui cryptocurrency
- Connect to Sui dApps
- Sign blockchain transactions
- Manage Sui NFTs and tokens

**Wallet Address Format:**
- Starts with `0x`
- Contains 64 hexadecimal characters
- Total length: **66 characters**
- Example: `0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef`

**Download Sui Wallet:**
- [Chrome Web Store - Sui Wallet](https://chrome.google.com/webstore/detail/sui-wallet)
- [Firefox Add-ons - Sui Wallet](https://addons.mozilla.org/en-US/firefox/addon/sui-wallet/)

---

## 🔧 **Integration Components**

### **Files Created:**

1. **`Assets/Plugins/JavaScript/SuiWallet.jslib`**
   - JavaScript bridge for WebGL
   - Connects to Sui Wallet browser extension
   - Handles wallet connection/disconnection

2. **`Assets/Scripts/SuiWalletManager.cs`**
   - Unity C# manager
   - Wraps JavaScript functions
   - Validates Sui addresses
   - Provides fallback for manual input

3. **Updated `SimpleSignInManager.cs`**
   - Integrates Sui wallet after Twitter login
   - Auto-connects wallet
   - Falls back to manual input if wallet not available

---

## 🚀 **Unity Setup**

### **Step 1: Add SuiWalletManager to Scene**

1. **Create an empty GameObject** in your scene
2. **Name it**: `SuiWalletManager`
3. **Add Component**: `SuiWalletManager` script
4. **Configure in Inspector**:
   - **Debug Mode**: `true` (for testing)
   - **Allow Manual Input**: `true` (fallback if wallet not installed)

### **Step 2: Ensure SimpleSignInManager is Configured**

Your existing `SimpleSignInManager` is already updated to:
1. Complete Twitter login
2. Automatically try to connect Sui Wallet
3. Fall back to manual input if wallet connection fails

---

## 🎮 **User Experience Flow**

### **Scenario 1: Sui Wallet Installed** (Best Experience)

1. **User logs in with Twitter** ✅
2. **Wallet panel appears** 💳
3. **Sui Wallet popup opens** automatically
4. **User approves connection** in wallet popup
5. **Wallet address auto-fills** (66 characters)
6. **Game resumes automatically** 🎮

### **Scenario 2: Sui Wallet Not Installed** (Fallback)

1. **User logs in with Twitter** ✅
2. **Wallet panel appears** 💳
3. **Message shows**: "Wallet not found, please enter manually"
4. **User enters their Sui wallet address** manually
5. **Address validates** (must be 66 characters starting with 0x)
6. **Click Confirm**
7. **Game resumes** 🎮

### **Scenario 3: Saved Wallet** (Returning User)

1. **User logs in with Twitter** ✅
2. **System detects saved wallet** from previous session
3. **Auto-confirms with saved wallet** ⚡
4. **Game resumes immediately** 🎮

---

## 🌐 **WebGL Build Setup**

### **For WebGL Deployment:**

Your game needs to detect the Sui Wallet extension. The extension injects a `window.suiWallet` object into the page.

### **HTML Template (Optional Enhancement)**

If you want to enhance the experience, you can modify your WebGL template to include Sui Wallet detection:

```html
<!DOCTYPE html>
<html>
<head>
    <title>2048 Balls - Sui Game</title>
    <script>
        // Detect Sui Wallet on page load
        window.addEventListener('load', function() {
            if (typeof window.suiWallet !== 'undefined') {
                console.log('Sui Wallet detected!');
            } else {
                console.log('Sui Wallet not found. Please install it.');
            }
        });
    </script>
</head>
<body>
    <!-- Your Unity WebGL content -->
</body>
</html>
```

---

## 🔍 **Testing**

### **In Unity Editor:**

The Sui Wallet Manager will simulate a connection for testing:
- Returns a dummy wallet address
- Allows you to test the flow without a real wallet

### **In WebGL Build:**

1. **Install Sui Wallet extension** in your browser
2. **Create or import a Sui wallet**
3. **Build and deploy your game** to WebGL
4. **Test the complete flow**:
   - Login with Twitter
   - Connect Sui Wallet when prompted
   - Confirm address
   - Game should resume

---

## 💡 **How It Works**

### **Technical Flow:**

```
┌─────────────────────────────────────────────────────────┐
│ 1. User Completes Twitter Login                        │
└─────────────────┬───────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────┐
│ 2. SimpleSignInManager.OnSignInCallback()              │
│    - Stores Twitter user info                          │
│    - Checks for saved wallet                           │
└─────────────────┬───────────────────────────────────────┘
                  │
                  ├─── Saved Wallet? ──→ YES ──→ Auto-confirm ──→ Game Starts
                  │
                  └─── NO ──→ Try Connect Sui Wallet
                               │
                               ▼
                  ┌────────────────────────────────────────┐
                  │ 3. SuiWalletManager.ConnectWallet()   │
                  │    - Calls JavaScript bridge          │
                  │    - Opens Sui Wallet popup           │
                  └─────────────┬──────────────────────────┘
                                │
                    ┌───────────┴───────────┐
                    │                       │
                    ▼                       ▼
          ┌─────────────────┐    ┌─────────────────────┐
          │ Wallet Found    │    │ Wallet Not Found    │
          │ - User approves │    │ - Show manual input │
          │ - Address sent  │    │ - User enters addr  │
          │   to Unity      │    │ - Validates format  │
          └────────┬────────┘    └──────────┬──────────┘
                   │                        │
                   └────────────┬───────────┘
                                │
                                ▼
                  ┌────────────────────────────────────────┐
                  │ 4. Address Validated                   │
                  │    - Must be 0x + 64 hex chars         │
                  │    - Saved to PlayerPrefs              │
                  └─────────────┬──────────────────────────┘
                                │
                                ▼
                  ┌────────────────────────────────────────┐
                  │ 5. Game Resumes                        │
                  │    - GameManager.IsLoggedIn = true     │
                  │    - Time.timeScale = 1f               │
                  │    - Balls start spawning              │
                  └────────────────────────────────────────┘
```

---

## 🛠️ **Advanced: Blockchain Transactions**

If you want to actually interact with the Sui blockchain (not just collect addresses), you would need to:

1. **Use Sui TypeScript SDK** in your JavaScript
2. **Call Move functions** on Sui smart contracts
3. **Handle transaction signing** through the wallet
4. **Track transaction results**

**Example (for future implementation):**
```javascript
// In your JavaScript bridge
async function executeSuiTransaction() {
    const tx = await wallet.signAndExecuteTransactionBlock({
        transactionBlock: /* your transaction */,
    });
    console.log('Transaction digest:', tx.digest);
}
```

---

## 📝 **Troubleshooting**

### **Issue: "Sui Wallet not found"**
**Solution**: 
1. Install Sui Wallet extension from Chrome Web Store
2. Refresh the game page
3. Or use manual input as fallback

### **Issue: "Invalid wallet address"**
**Solution**: 
- Ensure address starts with `0x`
- Must be exactly 66 characters
- Only hexadecimal characters (0-9, a-f)

### **Issue: Wallet connects but game doesn't resume**
**Solution**: 
1. Check Unity Console for errors
2. Verify `SuiWalletManager` is in scene
3. Ensure `SimpleSignInManager` is configured

### **Issue: Works in editor but not WebGL**
**Solution**: 
- WebGL needs the actual Sui Wallet extension
- Editor uses simulated connection
- Test in actual WebGL build with extension installed

---

## ✅ **Advantages of This Integration**

1. **✅ Automatic Wallet Connection**: Seamless UX for users with Sui Wallet
2. **✅ Manual Fallback**: Works even if wallet not installed
3. **✅ Address Validation**: Ensures correct Sui address format
4. **✅ Session Persistence**: Saves wallet for returning users
5. **✅ Ready for Blockchain**: Foundation for future on-chain features

---

## 🎉 **Complete Flow Summary**

**Your game now supports:**

1. **Twitter OAuth Login** (via SimpleSignIn plugin)
2. **Sui Wallet Connection** (via browser extension)
3. **Manual Wallet Input** (fallback option)
4. **Address Validation** (66-character format)
5. **Session Management** (saves wallet address)
6. **Game Integration** (resumes after wallet confirmation)

**Based on best practices from:**
- [Sui Coin Flip Example](https://docs.sui.io/guides/developer/app-examples/coin-flip)
- Sui Wallet Extension API
- Unity WebGL JavaScript bridges

---

## 🚀 **Next Steps**

1. **Build for WebGL** and deploy
2. **Install Sui Wallet** in your browser
3. **Test the complete flow**
4. **(Future) Add blockchain transactions** using Sui TypeScript SDK
5. **(Future) Store scores on-chain** as Sui objects

Your game is now ready for the Sui ecosystem! 🎮✨

