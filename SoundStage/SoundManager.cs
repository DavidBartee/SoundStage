using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SoundStage
{
    class SoundManager
    {
        List<MediaPlayer> mPlayers = new List<MediaPlayer>();
        List<SoundPlayer> sPlayers = new List<SoundPlayer>();

        public void PlaySound(string filePath) {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            int numBytes = 4;
            string hexBytes = "";
            for (int i = 0; i < numBytes; i++) {
                hexBytes += string.Format($"{fs.ReadByte():X2}");
            }
            if (hexBytes.Substring(0, 8) == "52494646") {
                SoundPlayer player = new SoundPlayer(filePath);
                sPlayers.Add(player);
                int index = sPlayers.IndexOf(player);
                //player.Dispose();
                sPlayers[index].Play();
            } else if (hexBytes.Substring(0, 6) == "494433" || hexBytes.Substring(0, 4) == "FFFB") {
                MediaPlayer mp = new MediaPlayer();
                mPlayers.Add(mp);
                int index = mPlayers.IndexOf(mp);
                mPlayers[index].Open(new Uri(filePath));
                mPlayers[index].Play();
            }
        }
    }
}
