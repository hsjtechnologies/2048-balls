mergeInto(LibraryManager.library, {
  // Connect to Sui Wallet
  ConnectSuiWallet: function() {
    console.log("Connecting to Sui Wallet...");
    
    // Check if Sui wallet is available
    if (typeof window.suiWallet !== 'undefined') {
      window.suiWallet.connect()
        .then(function(accounts) {
          if (accounts && accounts.length > 0) {
            var address = accounts[0];
            console.log("Sui Wallet connected:", address);
            
            // Send wallet address back to Unity
            SendMessage('SimpleSignInManager', 'OnSuiWalletConnected', address);
          }
        })
        .catch(function(error) {
          console.error("Sui Wallet connection failed:", error);
          SendMessage('SimpleSignInManager', 'OnSuiWalletFailed', error.message || 'Connection failed');
        });
    } else {
      console.error("Sui Wallet not found");
      SendMessage('SimpleSignInManager', 'OnSuiWalletFailed', 'Sui Wallet extension not installed');
    }
  },
  
  // Get current Sui wallet address
  GetSuiWalletAddress: function() {
    if (typeof window.suiWallet !== 'undefined' && window.suiWallet.address) {
      var address = window.suiWallet.address;
      var bufferSize = lengthBytesUTF8(address) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(address, buffer, bufferSize);
      return buffer;
    }
    return null;
  },
  
  // Disconnect Sui Wallet
  DisconnectSuiWallet: function() {
    if (typeof window.suiWallet !== 'undefined' && window.suiWallet.disconnect) {
      window.suiWallet.disconnect();
      console.log("Sui Wallet disconnected");
    }
  },
  
  // Check if Sui Wallet is installed
  IsSuiWalletInstalled: function() {
    return typeof window.suiWallet !== 'undefined' ? 1 : 0;
  }
});
