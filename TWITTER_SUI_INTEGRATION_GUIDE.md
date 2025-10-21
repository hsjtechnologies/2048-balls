# Twitter + Sui Wallet Integration Setup Guide

## Overview
This guide will help you integrate Twitter authentication with Sui wallet functionality into your Unity 2048 Balls game using the Simple Sign In plugin.

## Prerequisites
- Unity 2020.3 or later
- Twitter Developer Account
- Sui wallet address

## Step-by-Step Setup

### 1. Twitter Developer Setup

1. **Create Twitter Developer Account**
   - Go to [Twitter Developer Portal](https://developer.twitter.com/)
   - Create a new app or use existing one
   - Note down your **Client ID** and **Client Secret**

2. **Configure OAuth 2.0 Settings**
   - Enable OAuth 2.0 in your Twitter app settings
   - Set **Callback URL**: `ball2048.oauth://oauth2/x`
   - Add scopes: `tweet.read`, `users.read`

### 2. Unity Configuration

#### A. Update XAuth Settings
1. Open `Assets/SimpleSignIn/X/Resources/XAuthSettings.asset`
2. Replace the placeholder values:
   - **ClientId**: Your Twitter Client ID
   - **ClientSecret**: Your Twitter Client Secret
   - **CustomUriScheme**: `ball2048.oauth` (already set)

#### B. Android Manifest (for Android builds)
The Android manifest is already configured at `Assets/Plugins/Android/AndroidManifest.xml` with the correct deep linking scheme.

### 3. Scene Setup

#### Option A: Automatic UI Setup (Recommended)
1. Create an empty GameObject in your scene
2. Add the `TwitterLoginUISetup` component
3. The component will automatically create all necessary UI elements

#### Option B: Manual UI Setup
1. Create a Canvas in your scene
2. Add the `TwitterLoginManager` component to a GameObject
3. Manually assign UI references:
   - Login Panel
   - Wallet Panel  
   - Game Panel
   - Status Text
   - Username Text
   - Wallet Input Field
   - Login Button
   - Confirm Wallet Button
   - Logout Button

### 4. GameManager Integration

The `GameManager` has been updated to work with the new authentication system:
- Game pauses until user logs in
- Automatically resumes when login is complete
- Handles logout events properly

### 5. Testing the Integration

#### In Unity Editor:
1. Press Play
2. Click "Login with Twitter" button
3. Complete Twitter OAuth flow
4. Enter a valid Sui wallet address
5. Game should resume automatically

#### On Device:
1. Build and deploy to device
2. Test the complete flow
3. Verify deep linking works correctly

## Features Included

### Authentication Flow
- ✅ Twitter OAuth 2.0 integration
- ✅ User session persistence
- ✅ Automatic token refresh
- ✅ Secure credential storage

### Sui Wallet Integration
- ✅ Wallet address input validation
- ✅ Address format verification (0x + 64 characters)
- ✅ Per-user wallet storage
- ✅ Wallet address display

### Game Integration
- ✅ Automatic game pause/resume
- ✅ Login state management
- ✅ Event-driven architecture
- ✅ Clean logout functionality

## Troubleshooting

### Common Issues

1. **"Failed to initialize Twitter authentication"**
   - Check if XAuthSettings.asset has correct Client ID/Secret
   - Verify Twitter app is properly configured

2. **Deep linking not working on Android**
   - Ensure AndroidManifest.xml is in `Assets/Plugins/Android/`
   - Check that the scheme matches your CustomUriScheme

3. **"Invalid Sui wallet address format"**
   - Sui addresses must start with "0x" and be 66 characters total
   - Example: `0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef`

4. **Game doesn't resume after login**
   - Check GameManager.IsLoggedIn is being set correctly
   - Verify Time.timeScale is being set to 1f

### Debug Mode
Enable debug mode in `TwitterLoginManager` to see detailed logs:
```csharp
loginManager.debugMode = true;
```

## Security Considerations

1. **Never commit credentials to version control**
   - Use environment variables or secure storage
   - Consider using a backend proxy for production

2. **Validate wallet addresses**
   - Always verify Sui address format
   - Consider additional validation on backend

3. **Token security**
   - Tokens are stored securely using Unity's PlayerPrefs
   - Consider implementing additional encryption for production

## Next Steps

1. **Backend Integration**
   - Connect to your game backend
   - Send user data and wallet address to server
   - Implement user-specific game features

2. **Enhanced Features**
   - Add user profile display
   - Implement achievements tied to Twitter account
   - Add social sharing features

3. **Production Deployment**
   - Set up proper Twitter app for production
   - Configure production callback URLs
   - Test on multiple devices and platforms

## Support

For issues with the Simple Sign In plugin, refer to:
- Plugin documentation
- Twitter Developer documentation
- Unity OAuth best practices

For Sui wallet integration:
- Sui documentation
- Wallet address format specifications
