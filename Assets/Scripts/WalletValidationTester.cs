using UnityEngine;

/// <summary>
/// Simple test script to verify wallet address validation
/// Attach this to any GameObject and use the Context Menu to test
/// </summary>
public class WalletValidationTester : MonoBehaviour
{
    [ContextMenu("Test Wallet Validation")]
    public void TestWalletValidation()
    {
        Debug.Log("=== WALLET VALIDATION TEST ===");
        
        // Test valid addresses
        TestAddress("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef", "Valid address");
        TestAddress("0x0000000000000000000000000000000000000000000000000000000000000000", "Valid address (all zeros)");
        
        // Test invalid addresses
        TestAddress("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcde", "Too short (65 chars)");
        TestAddress("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef1", "Too long (67 chars)");
        TestAddress("1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef", "Missing 0x prefix");
        TestAddress("0X1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef", "Wrong prefix (0X instead of 0x)");
        TestAddress("", "Empty string");
        TestAddress("0x", "Just prefix");
        TestAddress("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdefg", "Invalid character (g)");
        
        Debug.Log("=== END WALLET VALIDATION TEST ===");
    }
    
    private void TestAddress(string address, string description)
    {
        bool isValid = IsValidSuiAddress(address);
        string result = isValid ? "✅ VALID" : "❌ INVALID";
        Debug.Log($"{result} - {description}: '{address}' (Length: {address.Length})");
    }
    
    /// <summary>
    /// Simple wallet validation - just checks format (0x + 66 characters total)
    /// </summary>
    private bool IsValidSuiAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            return false;
        
        // Simple validation: just check format (0x + 66 characters total)
        return address.StartsWith("0x") && address.Length == 66;
    }
}
