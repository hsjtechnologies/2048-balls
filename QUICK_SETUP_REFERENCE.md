# ⚡ Quick Setup Reference - Twitter + Sui Wallet

## 🎯 **What You Have Now**

✅ **SimpleSignIn Plugin** - Professional Twitter OAuth  
✅ **Sui Wallet Integration** - Browser extension connector  
✅ **Manual Fallback** - Works without wallet extension  
✅ **Complete Game Flow** - Login → Wallet → Play

---

## 🚀 **5-Minute Setup**

### **Step 1: Twitter App Setup** (2 minutes)

1. Go to [Twitter Developer Portal](https://developer.twitter.com/)
2. Get your **Client ID** and **Client Secret**
3. Set callback URL: `ball2048.oauth://oauth2/x`

### **Step 2: Unity Configuration** (2 minutes)

1. **Open**: `Assets/SimpleSignIn/X/Resources/XAuthSettings.asset`
2. **Set**:
   - Client ID: `YOUR_CLIENT_ID`
   - Client Secret: `YOUR_CLIENT_SECRET`
   - Custom URI Scheme: `ball2048.oauth` ✅ (already set)

3. **Add to Scene**:
   - Create GameObject → Add `SimpleSignInManager`
   - Create GameObject → Add `SuiWalletManager`
   - Assign UI references in Inspector

### **Step 3: Test** (1 minute)

1. **Press Play**
2. **Click Login** → Twitter OAuth
3. **Wallet connects** → Sui Wallet or manual input
4. **Game starts** → Balls spawn!

---

## 🎮 **Complete Game Flow**

```
Login with Twitter (SimpleSignIn)
         ↓
Connect Sui Wallet (Auto-detect or manual)
         ↓
Confirm Wallet Address (66 characters)
         ↓
Game Starts! (Balls spawn, game active)
```

---

## 📁 **Key Files**

| File | Purpose |
|------|---------|
| `SimpleSignInManager.cs` | Main login controller |
| `SuiWalletManager.cs` | Sui wallet connector |
| `SuiWallet.jslib` | JavaScript bridge for WebGL |
| `GameManager.cs` | Game state management |
| `XAuthSettings.asset` | Twitter OAuth config |

---

## 🔧 **Scene Hierarchy**

```
YourScene
├── SimpleSignInManager
│   └── SimpleSignInManager.cs
├── SuiWalletManager
│   └── SuiWalletManager.cs
├── GameManager
│   └── GameManager.cs
└── UI
    ├── LoginButton (TwitterLoginButton.cs)
    ├── WalletPanel
    │   ├── WalletInput (WalletInputField.cs)
    │   └── ConfirmButton (WalletConfirmButton.cs)
    └── GameCanvas
```

---

## ✅ **Checklist**

- [ ] Twitter app created with Client ID/Secret
- [ ] `XAuthSettings.asset` configured with credentials
- [ ] `SimpleSignInManager` in scene with UI assigned
- [ ] `SuiWalletManager` in scene
- [ ] Helper scripts on buttons/inputs
- [ ] AndroidManifest.xml has deep linking (for Android)
- [ ] Tested in Unity Editor
- [ ] Built and tested in WebGL

---

## 🌐 **For Players**

### **What Players Need:**

1. **Twitter Account** (for login)
2. **Sui Wallet** (optional, for best experience)
   - [Install Sui Wallet](https://chrome.google.com/webstore/detail/sui-wallet)
   - Or enter wallet address manually

### **Sui Wallet Address Format:**
- Must start with `0x`
- Must be exactly **66 characters** long
- Example: `0x1234...abcd` (64 hex characters after 0x)

---

## 🎯 **References**

- **Twitter OAuth**: [SimpleSignIn Setup Guide](SIMPLESIGNIN_SETUP_GUIDE.md)
- **Sui Wallet**: [Sui Wallet Integration Guide](SUI_WALLET_INTEGRATION_GUIDE.md)
- **Sui Docs**: [https://docs.sui.io](https://docs.sui.io/guides/developer/app-examples/coin-flip)

---

## 💡 **Tips**

**For Development:**
- Enable Debug Mode in both managers
- Test in Editor first (simulates wallet)
- Then test in WebGL build

**For Production:**
- Get your own Twitter credentials (not test ones)
- Disable Debug Mode
- Test on multiple browsers
- Add error handling for edge cases

**For Users:**
- Clear instructions on Sui wallet
- Provide manual input option
- Show wallet address after connection
- Allow users to change wallet

---

## 🎉 **You're All Set!**

Your game now has:
- ✅ Professional Twitter login (SimpleSignIn)
- ✅ Sui blockchain wallet integration
- ✅ Automatic or manual wallet input
- ✅ Complete session management
- ✅ Ready for blockchain features

**Happy gaming! 🎮**

