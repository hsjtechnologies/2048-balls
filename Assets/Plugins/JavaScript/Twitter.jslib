// mergeInto(LibraryManager.library, {
  // OpenTwitterAuth: function () {
    // Open Twitter login or share URL
    // window.open("https://twitter.com/login", "_blank");
//  }
//});

mergeInto(LibraryManager.library, {
  TwitterSignIn: function() {
    // Example pseudo-code (depends on plugin)
    console.log("Starting Twitter login...");

    // Simulate success after login flow
    setTimeout(() => {
      // send back username or token
      SendMessage('GameManager', 'OnTwitterLoginSuccess', 'user123');
    }, 2000);
  },
  
  GetURLParameters: function() {
    // Get URL parameters from browser
    var urlParams = window.location.search;
    var result = UTF8ToString(AllocateStringFromUTF8(urlParams));
    return result;
  }
});

