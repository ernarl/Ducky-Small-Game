// Browsers keep playing sounds that already started even when the game stops (in a background tab, or when another
// window or the page around the game is clicked), so this silences the game's audio until it's focused again
mergeInto(LibraryManager.library, {
  PauseAudioWhenUnfocused__deps: ['$WEBAudio'],
  PauseAudioWhenUnfocused: function () {
    function update() {
      var context = WEBAudio.audioContext;
      if (!context) {
        return;
      }

      if (document.hidden || !document.hasFocus()) {
        if (context.state === 'running') {
          context.suspend();
        }
      } else if (context.state === 'suspended') {
        context.resume();
      }
    }

    document.addEventListener('visibilitychange', update);
    window.addEventListener('blur', update);
    window.addEventListener('focus', update);
  }
});
