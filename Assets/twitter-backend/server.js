const express = require("express");
const axios = require("axios");
const querystring = require("querystring");
const crypto = require("crypto");

const app = express();

// TODO: Replace these values with your NEW Twitter app credentials
const CLIENT_ID = "RzFjeUtVeHZFSjc4VnlBYkYzcnc6MTpjaQ";
const CLIENT_SECRET = "aptoxPyuO8MTWcCi_41zGoVk6uMiwATWu5vEBFvqxlk8bYBbh4";
const REDIRECT_URI = "https://two048-balls-webgl-build-1.onrender.com";

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
  const { code, state, error, error_description } = req.query;

  console.log("📞 OAuth callback received");
  console.log("Full query params:", req.query);
  console.log("Code:", code ? "Present" : "Missing");
  console.log("State:", state);
  console.log("Error:", error);
  console.log("Error Description:", error_description);

  if (error) {
    console.error("❌ OAuth Error:", error, error_description);
    return res.status(400).send(`❌ OAuth Error: ${error} - ${error_description}`);
  }

  if (!code) {
    console.error("❌ No authorization code received");
    console.error("❌ This usually means:");
    console.error("   1. User cancelled the login");
    console.error("   2. Twitter App callback URL is wrong");
    console.error("   3. Twitter App is not configured for OAuth 2.0");
    return res.status(400).send("❌ Missing code. Did you cancel the login?");
  }

  try {
    // Create proper Base64 encoded credentials
    const credentials = Buffer.from(`${CLIENT_ID}:${CLIENT_SECRET}`).toString("base64");
    console.log("🔑 Using credentials:", credentials.substring(0, 10) + "...");

    // Add a longer delay to prevent rate limiting during testing
    console.log("⏳ Waiting 5 seconds to prevent rate limiting...");
    await new Promise(resolve => setTimeout(resolve, 5000));

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
      res.redirect(`https://app.bratonsui.com?twitter=success&token=${accessToken}&username=${username}`);

    } catch (userError) {
      console.error("❌ Error getting user info:", userError.response?.data || userError.message);
      // Fallback: redirect with just token
      res.redirect(`https://app.bratonsui.com?xtwitter=success&token=${accessToken}`);
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
    } else if (error.response?.status === 429) {
      console.error("⚠️ Rate limit exceeded - Twitter API is throttling requests");

      // Try to get rate limit info from headers
      const retryAfter = error.response?.headers?.['retry-after'] || error.response?.headers?.['x-rate-limit-reset'];
      const resetTime = retryAfter ? `${retryAfter} seconds` : "15-30 minutes";

      res.send(`
        ❌ Twitter API Rate Limit Exceeded

        <p>You've made too many requests to Twitter's API.</p>
        <p><strong>Please wait ${resetTime} before trying again.</strong></p>

        <hr>

        <h3>This is normal during testing!</h3>
        <p>Twitter limits how often you can test the OAuth flow.</p>

        <h3>Quick Fixes:</h3>
        <ul>
          <li>✅ Wait 15-30 minutes (recommended)</li>
          <li>✅ Try a different browser</li>
          <li>✅ Try from incognito/private mode</li>
          <li>✅ Use a different Twitter account</li>
        </ul>

        <p><a href="/auth/twitter">Try Again (if enough time has passed)</a></p>
      `);
    } else {
      res.send("❌ Authentication failed. Check backend console for details.");
    }
  }
});

// Test endpoint
app.get("/", async (req, res) => {
  const { code, state, error, error_description } = req.query;

  console.log("📞 OAuth callback received");
  console.log("Full query params:", req.query);
  console.log("Code:", code ? "Present" : "Missing");
  console.log("State:", state);
  console.log("Error:", error);
  console.log("Error Description:", error_description);

  if (error) {
    console.error("❌ OAuth Error:", error, error_description);
    return res.status(400).send(`❌ OAuth Error: ${error} - ${error_description}`);
  }

  if (!code) {
    console.error("❌ No authorization code received");
    console.error("❌ This usually means:");
    console.error("   1. User cancelled the login");
    console.error("   2. Twitter App callback URL is wrong");
    console.error("   3. Twitter App is not configured for OAuth 2.0");
    return res.status(400).send("❌ Missing code. Did you cancel the login?");
  }

  try {
    // Create proper Base64 encoded credentials
    const credentials = Buffer.from(`${CLIENT_ID}:${CLIENT_SECRET}`).toString("base64");
    console.log("🔑 Using credentials:", credentials.substring(0, 10) + "...");

    // Add a longer delay to prevent rate limiting during testing
    console.log("⏳ Waiting 5 seconds to prevent rate limiting...");
    await new Promise(resolve => setTimeout(resolve, 5000));

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
      res.redirect(`https://app.bratonsui.com?twitter=success&token=${accessToken}&username=${username}`);

    } catch (userError) {
      console.error("❌ Error getting user info:", userError.response?.data || userError.message);
      // Fallback: redirect with just token
      res.redirect(`https://app.bratonsui.com?xtwitter=success&token=${accessToken}`);
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
    } else if (error.response?.status === 429) {
      console.error("⚠️ Rate limit exceeded - Twitter API is throttling requests");

      // Try to get rate limit info from headers
      const retryAfter = error.response?.headers?.['retry-after'] || error.response?.headers?.['x-rate-limit-reset'];
      const resetTime = retryAfter ? `${retryAfter} seconds` : "15-30 minutes";

      res.send(`
        ❌ Twitter API Rate Limit Exceeded

        <p>You've made too many requests to Twitter's API.</p>
        <p><strong>Please wait ${resetTime} before trying again.</strong></p>

        <hr>

        <h3>This is normal during testing!</h3>
        <p>Twitter limits how often you can test the OAuth flow.</p>

        <h3>Quick Fixes:</h3>
        <ul>
          <li>✅ Wait 15-30 minutes (recommended)</li>
          <li>✅ Try a different browser</li>
          <li>✅ Try from incognito/private mode</li>
          <li>✅ Use a different Twitter account</li>
        </ul>

        <p><a href="/auth/twitter">Try Again (if enough time has passed)</a></p>
      `);
    } else {
      res.send("❌ Authentication failed. Check backend console for details.");
    }
  }
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`✅ Server running on port ${PORT}`);
  console.log(`🔗 Test URL: http://localhost:${PORT}`);
  console.log(`🔗 Twitter OAuth: http://localhost:${PORT}/auth/twitter`);
});
