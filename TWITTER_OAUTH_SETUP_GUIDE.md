# Complete Twitter/X OAuth Setup Guide

This guide will help you set up Twitter/X OAuth authentication for your 2048 Balls game.

## 📋 Prerequisites

- Twitter/X Developer Account ([developer.twitter.com](https://developer.twitter.com))
- Render.com account for backend hosting
- Vercel deployment (or similar) for your Unity WebGL game

---

## 🎯 Current Configuration

### Your Setup
- **Game URL:** `https://ball-game-235m.vercel.app/`
- **Backend URL:** `https://ball-game-hlvu.onrender.com`
- **Twitter App:** Using credentials from server.js

---

## 📝 Part 1: Twitter Developer Portal Setup

### Step 1: Access Twitter Developer Portal

1. Go to [developer.twitter.com](https://developer.twitter.com)
2. Sign in with your Twitter/X account
3. If you don't have a developer account, click "Apply" and follow the prompts

### Step 2: Create New App

1. Click **"Projects & Apps"** in the sidebar
2. Click **"Create App"** or **"+ Add App"**
3. Fill in the required information:

#### App Basic Information
- **App Name:** `2048 Balls Game` (or any unique name)
- **App Description:** `OAuth authentication for 2048 Balls Unity game`
- **Organization Name:** Your name or organization
- **Website URL:** `https://ball-game-235m.vercel.app/`
- **Terms of Service URL:** `https://ball-game-235m.vercel.app/` (optional)
- **Privacy Policy URL:** `https://ball-game-235m.vercel.app/` (optional)

### Step 3: Configure Authentication Settings

1. Click on your newly created app
2. Go to **"Authentication settings"** (or **"Settings"** → **"User authentication settings"**)

#### Configure OAuth 2.0 Settings:

1. **Type of App:** Select **"Web App"** or **"Automated App or Bot"**
   - ✅ **Web App** is recommended for your use case

2. **App Permissions:** Select **"Read"**
   - This is the minimum permission needed for OAuth

3. **Callback URLs / Redirect URLs:** Add EXACTLY this URL:
   ```
   https://ball-game-hlvu.onrender.com/auth/twitter/callback
   ```
   - ⚠️ **CRITICAL:** No trailing slash, must be exact
   - ⚠️ Must use HTTPS (not HTTP)
   - This is where Twitter will send users after login

4. **Website URL:** Should be:
   ```
   https://ball-game-235m.vercel.app/
   ```
   - This is your Unity game URL

5. **Bot Permissions:** Leave blank (not needed for OAuth)

### Step 4: Save Settings

1. Click **"Save"** button at the bottom
2. Wait for confirmation message
3. Note: It may take a few minutes for changes to propagate

### Step 5: Get Your Credentials

1. In your app settings, go to **"Keys and Tokens"** tab
2. Find your **"OAuth 2.0 Client ID and Client Secret"**
3. Click **"Generate"** if they don't exist
4. Copy both values:
   - **Client ID:**** `MEFHT3dRajQ0cFFGNTQyZzB5Qzc6MTpjaQ` (your actual ID)
   - **Client Secret:** `xWHiDsz1nU5FPXMEQL89lgd_-2dAJwl-5T3idF0P2shRl_7yrW` (your actual secret)

⚠️ **IMPORTANT:** Keep these credentials secure! Never commit them to public repositories.

---

## 📝 Part 2: Backend Server Configuration

### Your server.js File

The backend server located at `Assets/twitter-backend/server.js` should have:

```javascript
const CLIENT_ID = "MEFHT3dRajQ0cFFGNTQyZzB5Qzc6MTpjaQ";
const CLIENT_SECRET = "xWHiDsz1nU5FPXMEQL89lgd_-2dAJwl-5T3idF0P2shRl_7yrW";
const REDIRECT_URI = "https://ball-game-hlvu.onrender.com/auth/twitter/callback";
```

### Configuration Checklist

✅ **Client ID matches** what's in Twitter Developer Portal  
✅ **Client Secret matches** what's in Twitter Developer Portal  
✅ **Callback URL matches** exactly what's configured in Twitter (no trailing slash)

### Deploy to Render.com

1. **Push your code to GitHub:**
   ```bash
   git add Assets/twitter-backend/server.js
   git commit -m "Configure Twitter OAuth credentials"
   git push
   ```

2. **Render.com Deployment:**
   - Go to your Render.com dashboard
   - Find your backend service (`ball-game-hlvu`)
   - It should auto-deploy from the `xlogin/reskin` branch
   - Wait 2-3 minutes for deployment to complete

---

## 📝 Part 3: Verification Checklist

### Twitter Developer Portal

Check these settings in [developer.twitter.com](https://developer.twitter.com):

| Setting | Expected Value |
|---------|----------------|
| App Type | Web App |
| App Permissions | Read |
| Callback URLs | `https://ball-game-hlvu.onrender.com/auth/twitter/callback` |
| Website URL | `https://ball-game-235m.vercel.app/` |
| Client ID | Matches value in server.js |
| Client Secret | Matches value in server.js |

### Backend Server (Render.com)

Test these endpoints:

1. **Health Check:**
   ```
   https://ball-game-hlvu.onrender.com/
   ```
   Should return: "Twitter OAuth Backend - Server is running!"

2. **OAuth Initiation:**
   ```
   https://ball-game-hlvu.onrender.com/auth/twitter
   ```
   Should redirect to Twitter login page

---

## 🧪 Part 4: Testing the OAuth Flow

### Step-by-Step Test Process

1. **Wait for Deployment** (2-3 minutes after pushing code)

2. **Clear Browser Cache:**
   - Use incognito/private mode
   - Or clear cache completely

3. **Test OAuth Flow:**
   - Go to: `https://ball-game-hlvu.onrender.com/auth/twitter`
   - You should be redirected to Twitter login
   - Complete Twitter authentication
   - You should be redirected back to: 
     `https://ball-game-235m.vercel.app/?twitter=success&username=YOUR_USERNAME&token=YOUR_TOKEN`

### Expected Results

✅ **Success Indicators:**
- Twitter login page appears
- After login, redirects to your game URL
- URL contains `twitter=success` parameter
- URL contains username and token

❌ **Common Errors:**

| Error | Cause | Solution |
|-------|-------|----------|
| "Missing code" | Callback URL mismatch | Verify callback URL in Twitter settings |
| "Invalid credentials" | Wrong Client ID/Secret | Update server.js with correct credentials |
| "Rate limit exceeded" (429) | Too many requests | Wait 15-30 minutes before retrying |
| Wrong redirect URL | Cached OAuth state | Clear browser cache, use incognito mode |

---

## 🔧 Troubleshooting

### Issue: Still Redirecting to Old URL (`ball-game-gamma.vercel.app`)

**Solutions:**
1. ✅ Verify callback URL in Twitter settings matches exactly
2. ✅ Clear browser cache completely
3. ✅ Wait 5-10 minutes for changes to propagate
4. ✅ Use incognito mode for testing
5. ✅ Check Render.com logs for any errors

### Issue: HTTP 429 (Rate Limit)

**Solutions:**
1. ⏳ Wait 15-30 minutes
2. 🔄 Try from different browser/device
3. 🔄 Use different Twitter account for testing
4. ⏰ Space out your test requests

### Issue: "Invalid credentials"

**Solutions:**
1. ✅ Double-check Client ID and Client Secret match
2. ✅ Verify credentials are regenerated if app was recreated
3. ✅ Check for extra spaces or newlines in credentials
4. ✅ Ensure credentials are from the correct Twitter app

### Issue: Callback URL Mismatch

**Solutions:**
1. ✅ URL must be EXACTLY: `https://ball-game-hlvu.onrender.com/auth/twitter/callback`
2. ✅ No trailing slash
3. ✅ Must use HTTPS
4. ✅ Wait a few minutes after saving in Twitter portal

---

## 🎉 Success Checklist

Once everything is configured correctly:

- [ ] Twitter Developer Portal settings are correct
- [ ] Backend is deployed on Render.com
- [ ] Health check endpoint works
- [ ] OAuth endpoint redirects to Twitter
- [ ] After login, redirects to correct game URL
- [ ] Unity game receives OAuth callback with parameters

---

## 📞 Next Steps After Setup

1. **Test from Your Unity Game:**
   - Go to your game: `https://ball-game-235m.vercel.app/`
   - Click the login button
   - Complete Twitter authentication
   - Verify game starts after login

2. **Monitor Backend Logs:**
   - Check Render.com logs for any errors
   - Verify successful OAuth flow

3. **Test with Different Users:**
   - Try with different Twitter accounts
   - Verify tokens are generated correctly

---

## 🔗 Important URLs

| Purpose | URL |
|---------|-----|
| Game URL | `https://ball-game-235m.vercel.app/` |
| Backend URL | `https://ball-game-hlvu.onrender.com` |
| OAuth Endpoint | `https://ball-game-hlvu.onrender.com/auth/twitter` |
| Callback URL | `https://ball-game-hlvu.onrender.com/auth/twitter/callback` |
| Twitter Developer | `https://developer.twitter.com` |

---

## ✅ Current Configuration Summary

Your current setup:

**Twitter App Credentials:**
- Client ID: `MEFHT3dRajQ0cFFGNTQyZzB5Qzc6MTpjaQ`
- Client Secret: `xWHiDsz1nU5FPXMEQL89lgd_-2dAJwl-5T3idF0P2shRl_7yrW`

**URLs:**
- Game: `https://ball-game-235m.vercel.app/`
- Backend: `https://ball-game-hlvu.onrender.com`
- Callback: `https://ball-game-hlvu.onrender.com/auth/twitter/callback`

**Git Branch:**
- `xlogin/reskin` (connected to Render.com)

---

## 🚀 Deployment Status

After pushing changes, wait for:
1. ✅ GitHub to receive the commit (instant)
2. ✅ Render.com to detect changes (1-2 minutes)
3. ✅ Render.com to deploy new version (2-5 minutes)
4. ✅ Changes to be live (total: 3-7 minutes)

You can monitor deployment at: https://dashboard.render.com

---

## 📚 Additional Resources

- [Twitter API Documentation](https://developer.twitter.com/en/docs)
- [OAuth 2.0 Flow](https://developer.twitter.com/en/docs/authentication/oauth-2-0)
- [PKCE Flow](https://developer.twitter.com/en/docs/authentication/oauth-2-0/user-access-token)

---

**Last Updated:** Current date  
**Configuration Version:** 2.0 (with fresh credentials)

