using System;
using System.IO;
using WMPLib;

namespace Music_Player
{
    // Added functionality for the WindowsMediaPlayer to auto-increment the frequency on play
    public class CustomPlayer : Db
    {
        public WindowsMediaPlayer wplayer;

        public CustomPlayer()
        {
            wplayer = new WindowsMediaPlayer();
            wplayer.settings.autoStart = false;
        }

        // Increment frequency
        public void playIncF()
        {
            wplayer.controls.play();
            String song = Path.GetFileNameWithoutExtension(wplayer.URL);
            incrementFrequency(song);
            
        }
    }

}
