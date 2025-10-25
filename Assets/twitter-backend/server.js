const express = require("express");
const axios = require("axios");
const querystring = require("querystring");
const crypto = require("crypto");

const app = express();

// TODO: Replace these values with your Twitter app credentials
const CLIENT_ID = "S1A3NkVGWVpTOTcyTzRLTjMzbmg6MTpjaQ";
const CLIENT_SECRET = "rpwUg11V8O6FnctQ99zYgQnhSGrg0WJJ857dEXtVILuq_D11C_";
const REDIRECT_URI = "https://ball-game-hlvu.onrender.com/auth/twitter/callback";

// Validate credentials
if (!CLIENT_ID || !CLIENT_SECRET || CLIENT_ID.includes("TODO") || CLIENT_SECRET.includes("TODO")) {
  console.error("❌ Invalid Twitter API credentials! Please update CLIENT_ID and CLIENT_SECRET in server.js");
}

const SCOPES = "tweet.read users.read offline.access";

// Generate proper PKCE code verifier and challenge
const CODE_VERIFIER = crypto.randomBytes(32).toString('base64url');
const CODE_CHALLENGE = crypto.createHash('sha256').update(CODE_VERIFIER).digest('base64url');

app.get("/auth/twitter", (req, res) => {
  const authUrl = `https://twitter.com/i/oauth2/authorize?response_type=code&client_id=${CLIENT_ID}&redirect_uri=${encodeURIComponent(
    REDIRECT_URI
  )}&scope=${encodeURIComponent(
    SCOPES
  )}&state=state123&code_challenge=${CODE_CHALLENGE}&code_challenge_method=S256`;

  console.log("🔗 Redirecting to Twitter OAuth:", authUrl);
  res.redirect(authUrl);
});

app.get("/auth/twitter/callback", async (req, res) => {
  const { code, state } = req.query;
  
  console.log("📞 OAuth callback received");
  console.log("Code:", code ? "Present" : "Missing");
  console.log("State:", state);

  if (!code) {
    console.error("❌ No authorization code received");
    return res.status(400).send("❌ No authorization code received");
  }

  try {
    // Create proper Base64 encoded credentials
    const credentials = Buffer.from(`${CLIENT_ID}:${CLIENT_SECRET}`).toString("base64");
    console.log("🔑 Using credentials:", credentials.substring(0, 10) + "...");

    const tokenResponse = await axios.post(
      "https://api.twitter.com/2/oauth2/token",
      querystring.stringify({
        code: code,
        grant_type: "authorization_code",
        redirect_uri: REDIRECT_URI,
        code_verifier: CODE_VERIFIER
      }),
      {
        headers: {
          "Authorization": `Basic ${credentials}`,
          "Content-Type": "application/x-www-form-urlencoded"
        }
      }
    );

    console.log("✅ Access token:", tokenResponse.data);
    
    // Get user info from Twitter API
    const accessToken = tokenResponse.data.access_token;
    
    try {
      const userResponse = await axios.get("https://api.twitter.com/2/users/me", {
        headers: {
          "Authorization": `Bearer ${accessToken}`
        }
      });
      
      const username = userResponse.data.data.username;
      console.log("✅ Username:", username);
      
      // Redirect back to Unity game with both token and username
      res.redirect(`https://ball-game-235m.vercel.app?twitter=success&token=${accessToken}&username=${username}`);
      
    } catch (userError) {
      console.error("❌ Error getting user info:", userError.response?.data || userError.message);
      // Fallback: redirect with just token
      res.redirect(`https://ball-game-235m.vercel.app?twitter=success&token=${accessToken}`);
    }

  } catch (error) {
    console.error("❌ FULL OAUTH ERROR:");
    console.error("Status:", error.response?.status);
    console.error("Data:", error.response?.data);
    console.error("Message:", error.message);
    
    if (error.response?.status === 401) {
      res.send("❌ Authentication failed: Invalid credentials or authorization header");
    } else if (error.response?.status === 400) {
      res.send("❌ Bad request: Check code verifier and redirect URI");
    } else {
      res.send("❌ Authentication failed. Check backend console for details.");
    }
  }
});

// Test endpoint
app.get("/", (req, res) => {
  res.send(`
    <h1>Twitter OAuth Backend</h1>
    <p>Server is running!</p>
    <p><a href="/auth/twitter">Test Twitter Login</a></p>
    <p>PKCE Code Verifier: ${CODE_VERIFIER.substring(0, 10)}...</p>
    <p>PKCE Code Challenge: ${CODE_CHALLENGE.substring(0, 10)}...</p>
  `);
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`✅ Server running on port ${PORT}`);
  console.log(`🔗 Test URL: http://localhost:${PORT}`);
  console.log(`🔗 Twitter OAuth: http://localhost:${PORT}/auth/twitter`);
  console.log(`🎯 OAuth redirects to: https://ball-game-235m.vercel.app/`);
});
