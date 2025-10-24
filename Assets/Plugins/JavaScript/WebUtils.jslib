mergeInto(LibraryManager.library, {
  OpenURL: function (urlPtr) {
    var url = UTF8ToString(urlPtr);
    window.location.href = url;
  }
});
