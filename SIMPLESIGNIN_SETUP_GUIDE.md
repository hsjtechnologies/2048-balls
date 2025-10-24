# 🎯 SimpleSignIn X (Twitter) Integration Guide

## ✅ **Complete Setup for 2048 Balls Game**

This guide shows you how to use the **SimpleSignIn plugin you already paid for** to implement Twitter login with Sui wallet integration.

---

## 📋 **Step 1: Configure Twitter App**

### **A. Go to Twitter Developer Portal**
1. Visit [Twitter Developer Portal](https://developer.twitter.com/)
2. Create a new app or use existing one
3. Go to **Settings** → **User authentication settings**

### **B. Set up OAuth 2.0**
1. **App permissions**: Read
2. **Type of App**: Web App
3. **Callback URL / Redirect URL**: `ball2048.oauth://oauth2/x`
4. **Website URL**: Your game website URL

### **C. Get Your Credentials**
Copy these values (you'll need them):
- **Client ID** (starts with something like `VzZj...`)
- **Client Secret** (long string)

---

## 📋 **Step 2: Configure SimpleSignIn in Unity**

### **A. Open XAuthSettings**
1. **In Unity**, go to `Assets/SimpleSignIn/X/Resources/`
2. **Click on `XAuthSettings.asset`**
3. **In the Inspector**, fill in:
   - **Client Id**: YOUR_TWITTER_CLIENT_ID
   - **Client Secret**: YOUR_TWITTER_CLIENT_SECRET
   - **Custom Uri Scheme**: `ball2048.oauth` (already set)
   - **Access Scopes**: `tweet.read`, `users.read` (already set)

### **B. Android Deep Linking (for Android builds)**
The Android manifest is already configured at `Assets/Plugins/Android/AndroidManifest.xml` with the correct deep linking scheme: `ball2048.oauth`

---

## 📋 **Step 3: Unity Scene Setup**

### **A. Create Login Manager**
1. **Create an empty GameObject** in your scene
2. **Name it**: `SimpleSignInManager`
3. **Add Component**: `SimpleSignInManager` script

### **B. Assign UI References**

**Select the `SimpleSignInManager` GameObject and assign:**

#### **Login UI:**
- **Login Button**: Your Twitter login button
- **Login Panel**: Your login UI panel
- **Status Text**: Your status text (optional)

#### **Wallet UI:**
- **Wallet Panel**: Your `WalletAdd` canvas
- **Wallet Address Input**: Your TMP_InputField in `WalletAdd`
- **Confirm Wallet Button**: Your confirm button in `WalletAdd`
- **Username Text**: Your username display text (optional)

#### **Game UI:**
- **Game Panel**: Your main game canvas (optional)
- **Logout Button**: Your logout button (optional)

### **C. Add Helper Scripts to UI Elements**

#### **1. On your Login Button:**
- **Add Component**: `TwitterLoginButton`

#### **2. On your Wallet Input Field:**
- **Add Component**: `WalletInputField`

#### **3. On your Confirm Wallet Button:**
- **Add Component**: `WalletConfirmButton`

---

## 📋 **Step 4: Test the Integration**

### **A. Initial Test in Editor**
1. **Press Play**
2. **Click your login button**
3. **You should see**: Browser opens with Twitter login
4. **Complete Twitter login**
5. **You should return to Unity** with wallet panel showing

### **B. Complete Flow Test**
1. **After Twitter login**: Wallet panel should appear
2. **Enter a valid Sui wallet address**:
   ```
   0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef
   ```
3. **Click Confirm Wallet**
4. **Game should resume**: Balls should start spawning

---

## 📋 **Step 5: Build for WebGL**

### **A. Build Settings**
1. **File** → **Build Settings**
2. **Select WebGL**
3. **Player Settings** → **Resolution and Presentation**
4. **Set WebGL Template**: Default or your custom template

### **B. Build and Deploy**
1. **Click Build**
2. **Upload to your hosting** (Vercel, Netlify, etc.)
3. **Test the complete flow**

---

## 🎯 **Scene Hierarchy Structure**

```
Scene:
├── SimpleSignInManager (with SimpleSignInManager script)
│   ├── Login Button assigned
│   ├── Wallet Panel assigned (your WalletAdd canvas)
│   └── Game Panel assigned (optional)
│
├── Canvas (Login UI)
│   └── LoginButton (with TwitterLoginButton script)
│
├── WalletAdd (Canvas)
│   ├── WalletInputField (with WalletInputField script)
│   └── ConfirmButton (with WalletConfirmButton script)
│
└── GameCanvas
    └── (Your game UI)
```

---

## 🔍 **Troubleshooting**

### **Issue: "Test settings are in use"**
**Solution**: Replace the test credentials in `XAuthSettings.asset` with your own Twitter app credentials.

### **Issue: Deep linking not working**
**Solution**: 
1. Check that `CustomUriScheme` is `ball2048.oauth`
2. Verify `AndroidManifest.xml` has the correct scheme
3. Make sure your Twitter app callback URL matches: `ball2048.oauth://oauth2/x`

### **Issue: Wallet panel doesn't appear**
**Solution**:
1. Check Unity Console for error messages
2. Verify `Wallet Panel` is assigned in `SimpleSignInManager`
3. Make sure your `WalletAdd` canvas is in the scene

### **Issue: Game doesn't resume after wallet confirmation**
**Solution**:
1. Check that `GameManager.IsLoggedIn` is being set to `true`
2. Verify `Time.timeScale` is being set to `1f`
3. Check Unity Console for debug messages

---

## ✅ **Advantages of Using SimpleSignIn**

1. **✅ Professional Plugin**: You already paid for it, use it!
2. **✅ Better Security**: Handles OAuth 2.0 PKCE properly
3. **✅ Cross-Platform**: Works on WebGL, Android, iOS
4. **✅ Session Management**: Automatic token refresh
5. **✅ Deep Linking**: Proper mobile support
6. **✅ No Backend Needed**: All handled in Unity

---

## 📝 **Final Checklist**

- ✅ Twitter app created and OAuth 2.0 configured
- ✅ Client ID and Client Secret added to `XAuthSettings.asset`
- ✅ `SimpleSignInManager` script added to scene
- ✅ All UI references assigned in Inspector
- ✅ Helper scripts added to buttons and input fields
- ✅ Android manifest configured (for Android builds)
- ✅ Tested in Unity Editor
- ✅ Built and tested on WebGL

---

## 🎉 **You're All Set!**

Your game now has:
- ✅ Professional Twitter OAuth 2.0 login
- ✅ Sui wallet address integration
- ✅ Session persistence
- ✅ Score logging capability
- ✅ Complete user management

**The SimpleSignIn plugin handles all the complex OAuth stuff for you!**

