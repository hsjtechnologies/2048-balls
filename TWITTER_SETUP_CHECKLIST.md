# Twitter OAuth Quick Setup Checklist

Use this checklist to verify your Twitter OAuth setup is correct.

---

## ✅ Twitter Developer Portal Checklist

### Basic App Settings

- [ ] App Name: `2048 Balls Game` (or similar)
- [ ] App Type: **Web App**
- [ ] App Permissions: **Read** (minimum)

### URL Configuration

- [ ] **Website URL:** `https://ball-game-235m.vercel.app/`
- [ ] **Callback URLs:** `https://ball-game-hlvu.onrender.com/auth/twitter/callback`
  - ⚠️ No trailing slash
  - ⚠️ Must be EXACT, case-sensitive
- [ ] **Callback URL added and saved**

### Credentials

- [ ] **Client ID:** `MEFHT3dRajQ0cFFGNTQyZzB5Qzc6MTpjaQ`
- [ ] **Client Secret:** `xWHiDsz1nU5FPXMEQL89lgd_-2dAJwl-5T3idF0P2shRl_7yrW`
- [ ] Credentials copied and stored securely

---

## ✅ Backend Server (server.js) Checklist

### File Location
`Assets/twitter-backend/server.js`

### Configuration Values

```javascript
const CLIENT_ID = "MEFHT3dRajQ0cFFGNTQyZzB5Qzc6MTpjaQ";
const CLIENT_SECRET = "xWHiDsz1nU5FPXMEQL89lgd_-2dAJwl-5T3idF0P2shRl_7yrW";
const REDIRECT_URI = "https://ball-game-hlvu.onrender.com/auth/twitter/callback";
```

- [ ] CLIENT_ID matches Twitter Portal
- [ ] CLIENT_SECRET matches Twitter Portal
- [ ] REDIRECT_URI matches callback URL in Twitter Portal exactly

### Deployment

- [ ] Changes committed to git
- [ ] Changes pushed to GitHub
- [ ] Render.com is deploying from branch: `xlogin/reskin`
- [ ] Deployment completed (wait 3-5 minutes)

---

## ✅ Testing Checklist

### Test 1: Backend Health Check

- [ ] Visit: `https://ball-game-hlvu.onrender.com/`
- [ ] See: "Twitter OAuth Backend - Server is running!"
- [ ] No error messages

### Test 2: OAuth Endpoint

- [ ] Visit: `https://ball-game-hlvu.onrender.com/auth/twitter`
- [ ] Redirected to Twitter login page
- [ ] Can see Twitter authorization page

### Test 3: Complete OAuth Flow

- [ ] Click login button in your Unity game
- [ ] Redirected to Twitter
- [ ] Complete Twitter authentication
- [ ] Redirected back to: `https://ball-game-235m.vercel.app/`
- [ ] URL contains: `?twitter=success&username=...&token=...`

### Test 4: Unity Game Integration

- [ ] Game receives OAuth callback
- [ ] Username extracted correctly
- [ ] Token extracted correctly
- [ ] Game starts/resumes after login

---

## ❌ Common Issues & Quick Fixes

### Issue: Rate Limit (429 Error)

**Symptoms:** "Authentication failed: Request failed with status code 429"  
**Fix:** Wait 15-30 minutes, try again

### Issue: Missing Code

**Symptoms:** "❌ Missing code. Did you cancel the login?"  
**Fix:** 
1. Verify callback URL in Twitter settings
2. Wait 5 minutes after updating
3. Clear browser cache
4. Try incognito mode

### Issue: Invalid Credentials

**Symptoms:** "Authentication failed: Invalid credentials"  
**Fix:** 
1. Regenerate Client ID/Secret in Twitter Portal
2. Update server.js with new values
3. Redeploy to Render.com

### Issue: Wrong Redirect URL

**Symptoms:** Redirects to old URL (e.g., `ball-game-gamma.vercel.app`)  
**Fix:**
1. Update callback URL in Twitter Portal
2. Clear browser cache
3. Wait 5-10 minutes
4. Try incognito mode

---

## 🔍 Verification Commands

### Check if credentials match

1. Open Twitter Developer Portal
2. Open server.js file
3. Compare values side-by-side
4. Must match EXACTLY

### Check Render.com deployment

1. Go to: https://dashboard.render.com
2. Find your service: `ball-game-hlvu`
3. Check logs for any errors
4. Verify latest commit is deployed

### Test endpoints

```bash
# Health check
curl https://ball-game-hlvu.onrender.com/

# OAuth initiation
curl -I https://ball-game-hlvu.onrender.com/auth/twitter
# Should return 302 redirect to Twitter
```

---

## 📊 Configuration Summary

| Component | Value |
|-----------|-------|
| Game URL | `https://ball-game-235m.vercel.app/` |
| Backend URL | `https://ball-game-hlvu.onrender.com` |
| OAuth Endpoint | `https://ball-game-hlvu.onrender.com/auth/twitter` |
| Callback URL | `https://ball-game-hlvu.onrender.com/auth/twitter/callback` |
| Client ID | `MEFHT3dRajQ0cFFGNTQyZzB5Qzc6MTpjaQ` |
| Client Secret | `xWHiDsz1nU5FPXMEQL89lgd_-2dAJwl-5T3idF0P2shRl_7yrW` |
| Git Branch | `xlogin/reskin` |

---

## 🎯 Success Criteria

✅ All checkboxes above are checked  
✅ Can complete OAuth flow successfully  
✅ Redirects to correct game URL  
✅ Unity game receives OAuth parameters  
✅ Game starts after login  
✅ No errors in Render.com logs  

---

## 📞 If Still Having Issues

1. Check Render.com logs for detailed error messages
2. Try OAuth flow in incognito mode
3. Wait 5-10 minutes between changes
4. Verify each setting matches this checklist exactly
5. Consider recreating Twitter App if issues persist

---

**Last Updated:** Just now  
**Status:** Awaiting deployment (3-5 minutes)

