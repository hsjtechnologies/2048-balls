using UnityEngine;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// Manages Sui Wallet connection for WebGL builds
/// Integrates with Sui Wallet browser extension
/// Based on: https://docs.sui.io/guides/developer/app-examples/coin-flip
/// </summary>
public class SuiWalletManager : MonoBehaviour
{
    public static SuiWalletManager Instance { get; private set; }

    [Header("Settings")]
    public bool debugMode = true;
    public bool allowManualInput = true; // Allow manual address input if wallet not available

    // Events
    public static event Action<string> OnWalletConnected;
    public static event Action<string> OnWalletConnectionFailed;
    public static event Action OnWalletDisconnected;

    private string connectedWalletAddress = "";
    private bool isConnected = false;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void ConnectSuiWallet();

    [DllImport("__Internal")]
    private static extern string GetSuiWalletAddress();

    [DllImport("__Internal")]
    private static extern void DisconnectSuiWallet();

    [DllImport("__Internal")]
    private static extern int IsSuiWalletInstalled();
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Connect to Sui Wallet (browser extension)
    /// </summary>
    public void ConnectWallet()
    {
        LogDebug("Attempting to connect Sui Wallet...");

#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            if (IsSuiWalletInstalled() == 1)
            {
                ConnectSuiWallet();
            }
            else
            {
                LogError("Sui Wallet not installed");
                OnWalletConnectionFailed?.Invoke("Sui Wallet extension not installed. Please install it from Chrome Web Store.");
            }
        }
        catch (Exception e)
        {
            LogError($"Failed to connect wallet: {e.Message}");
            OnWalletConnectionFailed?.Invoke($"Connection error: {e.Message}");
        }
#else
        // In editor or non-WebGL, simulate connection for testing
        LogDebug("Running in Editor - simulating wallet connection");
        OnSuiWalletConnected("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
#endif
    }

    /// <summary>
    /// Disconnect from Sui Wallet
    /// </summary>
    public void DisconnectWallet()
    {
        LogDebug("Disconnecting Sui Wallet...");

#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            DisconnectSuiWallet();
        }
        catch (Exception e)
        {
            LogError($"Failed to disconnect wallet: {e.Message}");
        }
#endif

        connectedWalletAddress = "";
        isConnected = false;
        OnWalletDisconnected?.Invoke();
    }

    /// <summary>
    /// Get the currently connected wallet address
    /// </summary>
    public string GetConnectedAddress()
    {
        return connectedWalletAddress;
    }

    /// <summary>
    /// Check if wallet is connected
    /// </summary>
    public bool IsWalletConnected()
    {
        return isConnected && !string.IsNullOrEmpty(connectedWalletAddress);
    }

    /// <summary>
    /// Validate Sui wallet address format
    /// Sui addresses: 0x + 64 hex characters = 66 total characters
    /// </summary>
    public bool IsValidSuiAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            return false;

        // Sui address must start with 0x and be 66 characters long
        if (!address.StartsWith("0x") || address.Length != 66)
            return false;

        // Check if all characters after 0x are valid hex
        for (int i = 2; i < address.Length; i++)
        {
            char c = address[i];
            if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Set wallet address manually (fallback if browser extension not available)
    /// </summary>
    public void SetManualAddress(string address)
    {
        if (!allowManualInput)
        {
            LogError("Manual input not allowed");
            OnWalletConnectionFailed?.Invoke("Manual input is disabled");
            return;
        }

        if (IsValidSuiAddress(address))
        {
            connectedWalletAddress = address;
            isConnected = true;
            LogDebug($"Manual wallet address set: {address}");
            OnWalletConnected?.Invoke(address);
        }
        else
        {
            LogError($"Invalid Sui address format: {address}");
            OnWalletConnectionFailed?.Invoke("Invalid Sui wallet address format (must be 0x + 64 hex characters)");
        }
    }

    // Called from JavaScript when wallet connection succeeds
    public void OnSuiWalletConnected(string address)
    {
        LogDebug($"Sui Wallet connected: {address}");

        if (IsValidSuiAddress(address))
        {
            connectedWalletAddress = address;
            isConnected = true;
            OnWalletConnected?.Invoke(address);
        }
        else
        {
            LogError($"Received invalid address from wallet: {address}");
            OnWalletConnectionFailed?.Invoke("Invalid address received from wallet");
        }
    }

    // Called from JavaScript when wallet connection fails
    public void OnSuiWalletFailed(string error)
    {
        LogError($"Sui Wallet connection failed: {error}");
        OnWalletConnectionFailed?.Invoke(error);
    }

    private void LogDebug(string message)
    {
        if (debugMode)
            Debug.Log($"[SuiWalletManager] {message}");
    }

    private void LogError(string message)
    {
        Debug.LogError($"[SuiWalletManager] {message}");
    }

    /// <summary>
    /// Check if Sui Wallet extension is installed
    /// </summary>
    public bool IsSuiWalletAvailable()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            return IsSuiWalletInstalled() == 1;
        }
        catch
        {
            return false;
        }
#else
        // In editor, always return true for testing
        return true;
#endif
    }
}

