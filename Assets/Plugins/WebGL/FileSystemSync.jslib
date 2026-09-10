mergeInto(LibraryManager.library, {
  SyncWebGLFileSystem: function () {
    if (typeof FS !== 'undefined' && typeof FS.syncfs === 'function') {
      FS.syncfs(false, function (err) {
        if (err) {
          console.error('Failed to sync WebGL persistent data: ' + err);
        }
      });
    }
  }
});
