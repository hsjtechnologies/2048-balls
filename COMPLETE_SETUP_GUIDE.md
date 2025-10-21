# 🎮 Complete Twitter + Sui Wallet Integration Setup Guide

## 📋 **Overview**
This guide will help you implement a complete login system for your Unity 2048 Balls game with:
- ✅ Twitter/X authentication via backend
- ✅ Sui wallet address input and validation
- ✅ Game access gating until both are complete
- ✅ Score logging to Google Sheets with user data

## 🔧 **Technical Implementation Steps**

### **Step 1: Backend Setup (Twitter OAuth)**

#### 1.1 Twitter Developer Account Setup
1. Go to [Twitter Developer Portal](https://developer.twitter.com/)
2. Create a new app or use existing one
3. Enable OAuth 2.0 and note down:
   - **Client ID**
   - **Client Secret**
   - Set **Callback URL**: `https://your-backend-url.com/auth/twitter/callback`

#### 1.2 Backend Server Configuration
Your backend server (`server.js`) is already configured. Make sure to:
1. Set environment variables on your hosting platform (Render/Vercel):
   ```
   TWITTER_CLIENT_ID=your_client_id_here
   TWITTER_CLIENT_SECRET=your_client_secret_here
   ```
2. Update the callback URL in your backend to match your frontend URL

### **Step 2: Google Sheets Integration Setup**

#### 2.1 Create Google Apps Script Webhook
1. Go to [Google Apps Script](https://script.google.com/)
2. Create a new project
3. Replace the default code with:

```javascript
function doPost(e) {
  try {
    // Parse the incoming data
    const data = JSON.parse(e.postData.contents);
    
    // Open your Google Sheet
    const sheet = SpreadsheetApp.getActiveSheet();
    
    // Add headers if sheet is empty
    if (sheet.getLastRow() === 0) {
      sheet.getRange(1, 1, 1, 6).setValues([
        ['Twitter Username', 'Sui Wallet', 'Score', 'Level', 'Date', 'Game Mode']
      ]);
    }
    
    // Add the new row
    sheet.appendRow([
      data.twitter_username,
      data.sui_wallet,
      data.score,
      data.level,
      data.date,
      data.game_mode
    ]);
    
    return ContentService.createTextOutput(JSON.stringify({
      success: true,
      message: 'Score logged successfully'
    })).setMimeType(ContentService.MimeType.JSON);
    
  } catch (error) {
    return ContentService.createTextOutput(JSON.stringify({
      success: false,
      error: error.toString()
    })).setMimeType(ContentService.MimeType.JSON);
  }
}
```

4. Deploy as web app with execute permissions for "Anyone"
5. Copy the webhook URL and use it in Unity

### **Step 3: Unity Project Setup**

#### 3.1 Scene Setup (Automatic)
1. **Create an empty GameObject** in your scene
2. **Add `TwitterOAuthUISetup` component** to it
3. **Press Play** - the UI will be created automatically!

#### 3.2 Manual Scene Setup (Alternative)
If you prefer manual setup:

1. **Create a Canvas** in your scene
2. **Create an empty GameObject** and add `TwitterOAuth` component
3. **Create UI panels**:
   - Login Panel (with login button and status text)
   - Wallet Panel (with username text, input field, confirm button)
   - Game Panel (with logout button)
4. **Assign UI references** in the TwitterOAuth component

#### 3.3 Configure Components

**TwitterOAuth Component Settings:**
- **Backend URL**: `https://your-backend-url.com`
- **Google Sheets Webhook URL**: Your Google Apps Script webhook URL
- **Debug Mode**: Enable for testing

**GoogleSheetsLogger Component:**
- **Webhook URL**: Same as above
- **Enable Logging**: True
- **Debug Mode**: Enable for testing

### **Step 4: Game Integration**

#### 4.1 GameManager Integration
The GameManager has been updated to:
- Subscribe to login events
- Pause game until login is complete
- Resume game after successful authentication
- Log final scores automatically

#### 4.2 Score Logging
Scores are automatically logged when:
- Game ends (GameOver is called)
- User is logged in
- Google Sheets webhook is configured

### **Step 5: Testing the Complete Flow**

#### 5.1 In Unity Editor
1. **Press Play**
2. **Click "Login with Twitter"**
3. **Complete Twitter OAuth** in browser
4. **Enter Sui wallet address** (format: `0x` + 64 characters)
5. **Click Confirm**
6. **Game should resume automatically**
7. **Play until game over** - score should be logged

#### 5.2 On WebGL Build
1. **Build for WebGL**
2. **Deploy to your hosting platform**
3. **Test the complete flow**
4. **Check Google Sheets** for logged scores

## 🎯 **Detailed Unity Engineering Setup**

### **Required Scripts Overview**

1. **`TwitterOAuth.cs`** - Main login controller
   - Handles Twitter OAuth flow
   - Manages Sui wallet input
   - Controls game state
   - Logs scores to Google Sheets

2. **`GoogleSheetsLogger.cs`** - Score logging system
   - Sends score data to Google Sheets
   - Handles retries and error logging
   - Singleton pattern for easy access

3. **`TwitterOAuthUISetup.cs`** - Automatic UI generator
   - Creates all necessary UI elements
   - Handles styling and layout
   - One-click setup

4. **`GameManager.cs`** - Updated game controller
   - Integrates with login system
   - Handles game pause/resume
   - Triggers score logging

### **UI Hierarchy Structure**
```
Canvas
└── TwitterOAuthContainer
    ├── LoginPanel
    │   ├── Title (Text)
    │   ├── StatusText (Text)
    │   └── LoginButton (Button)
    ├── WalletPanel
    │   ├── UsernameText (Text)
    │   ├── WalletInputField (InputField)
    │   └── ConfirmButton (Button)
    └── GamePanel
        └── LogoutButton (Button)
```

### **Component Dependencies**
- **TwitterOAuth** requires UI references (auto-assigned by setup script)
- **GameManager** subscribes to TwitterOAuth events
- **GoogleSheetsLogger** is a singleton, accessible from anywhere

### **Data Flow**
1. **User clicks login** → TwitterOAuth opens backend URL
2. **Backend redirects** → User completes Twitter OAuth
3. **Callback received** → TwitterOAuth extracts username/token
4. **Wallet input shown** → User enters Sui wallet address
5. **Validation passed** → Game resumes, login complete
6. **Game ends** → Score logged to Google Sheets with user data

## 🔍 **Troubleshooting**

### **Common Issues**

1. **"Failed to initialize Twitter authentication"**
   - Check backend URL is correct
   - Verify backend server is running
   - Check Twitter app configuration

2. **"Invalid Sui wallet address format"**
   - Sui addresses must start with `0x`
   - Must be exactly 66 characters total
   - Example: `0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef`

3. **"Score logging failed"**
   - Check Google Sheets webhook URL
   - Verify Google Apps Script is deployed
   - Check webhook permissions

4. **Game doesn't resume after login**
   - Check GameManager.IsLoggedIn is being set
   - Verify Time.timeScale is set to 1f
   - Check for errors in console

### **Debug Mode**
Enable debug mode in both TwitterOAuth and GoogleSheetsLogger for detailed logs:
```csharp
twitterAuth.debugMode = true;
googleSheetsLogger.debugMode = true;
```

## 📊 **Google Sheets Data Structure**

Your Google Sheet will contain:
| Twitter Username | Sui Wallet | Score | Level | Date | Game Mode |
|------------------|------------|-------|-------|------|-----------|
| @username123     | 0x1234...  | 1250  | 3     | 2024-01-15 10:30:00 | 2048 Balls |

## 🚀 **Production Deployment**

### **Security Considerations**
1. **Never commit credentials** to version control
2. **Use environment variables** for all sensitive data
3. **Validate Sui addresses** on backend for additional security
4. **Rate limit** Google Sheets requests to avoid quotas

### **Performance Optimization**
1. **Cache login sessions** using PlayerPrefs
2. **Batch score logging** for high-frequency games
3. **Implement retry logic** for failed requests
4. **Use async operations** to avoid blocking

## 📝 **Next Steps**

1. **Test the complete flow** in Unity Editor
2. **Deploy to WebGL** and test on live site
3. **Monitor Google Sheets** for logged scores
4. **Add additional features** like leaderboards, achievements
5. **Implement user profiles** and game statistics

## 🆘 **Support**

If you encounter issues:
1. **Check Unity Console** for error messages
2. **Enable debug mode** for detailed logging
3. **Verify all URLs** are correct and accessible
4. **Test each component** individually
5. **Check browser network tab** for failed requests

---

**🎉 Your complete Twitter + Sui Wallet integration is now ready!**
