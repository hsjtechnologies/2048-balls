const express = require("express");
const axios = require("axios");
const querystring = require("querystring");

const app = express();

// TODO: Replace these values with your Twitter app credentials
const CLIENT_ID = "S1A3NkVGWVpTOTcyTzRLTjMzbmg6MTpjaQ";
const CLIENT_SECRET = "rpwUg11V8O6FnctQ99zYgQnhSGrg0WJJ857dEXtVILuq_D11C_";
const REDIRECT_URI = "https://my-twitter-backend.onrender.com/auth/twitter/callback";

const SCOPES = "tweet.read users.read offline.access";

// PKCE Code Verifier (should be generated, but using simple one for demo)
const CODE_VERIFIER = "challenge123";

app.get("/auth/twitter", (req, res) => {
  const authUrl = `https://twitter.com/i/oauth2/authorize?response_type=code&client_id=${CLIENT_ID}&redirect_uri=${encodeURIComponent(
    REDIRECT_URI
  )}&scope=${encodeURIComponent(
    SCOPES
  )}&state=state123&code_challenge=${CODE_VERIFIER}&code_challenge_method=plain`;

  res.redirect(authUrl);
});

app.get("/auth/twitter/callback", async (req, res) => {
  const { code } = req.query;

  try {
    // FIX 1: Correct Base64 encoding with backticks
    const credentials = Buffer.from(`${CLIENT_ID}:${CLIENT_SECRET}`).toString("base64");

    const tokenResponse = await axios.post(
      "https://api.twitter.com/2/oauth2/token",
      querystring.stringify({
        code: code,
        grant_type: "authorization_code",
        redirect_uri: REDIRECT_URI,
        code_verifier: CODE_VERIFIER  // FIX 2: Use the CODE_VERIFIER variable
      }),
      {
        headers: {
          // FIX 3: Add space after "Basic" and use backticks for string interpolation
          "Authorization": `Basic ${credentials}`,
          "Content-Type": "application/x-www-form-urlencoded"
        }
      }
    );

    console.log("✅ Access token:", tokenResponse.data);
    
    // FIX 4: Send user data back or redirect with token in URL
    const accessToken = tokenResponse.data.access_token;
    res.redirect(`https://ball-game-lilac.vercel.app?twitter=success&token=${accessToken}`);

  } catch (error) {
    console.error("❌ FULL OAUTH ERROR:", error.response?.data || error.message);
    res.send("❌ Authentication failed. Check backend console.");
  }
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`✅ Server running on port ${PORT}`);
});
