using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SoundStage {
    class SoundManager {
        List<MediaPlayer> mPlayers = new List<MediaPlayer>();
        public ObservableCollection<MediaQueueItem> mediaList = new ObservableCollection<MediaQueueItem>();
        public MediaPlayer currentMedia = new MediaPlayer();
        public bool isPlaying = false;

        public SoundManager() {
            currentMedia.MediaEnded += CurrentMedia_MediaEnded;
        }

        public void PlaySound(string filePath) {
            if (ValidateFile(filePath)) {
                MediaPlayer mp = new MediaPlayer();
                int index = mPlayers.IndexOf(mp);
                mp.Open(new Uri(filePath));
                mp.MediaEnded += (sender, e) => ReleaseSound(sender, e, mp);
                mp.Play();
                mPlayers.Add(mp);
            }
        }

        public bool ValidateFile(string filePath) {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            int numBytes = 4;
            string hexBytes = "";
            for (int i = 0; i < numBytes; i++) {
                hexBytes += string.Format($"{fs.ReadByte():X2}");
            }
            if (hexBytes.Substring(0, 6) == "494433" || hexBytes.Substring(0, 4) == "FFFB" || hexBytes.Substring(0, 8) == "52494646") {
                return true;
            } else
                return false;
        }

        void ReleaseSound(object sender, EventArgs e, MediaPlayer player) {
            mPlayers.Remove(player);
            player.Close();
        }

        void CloseSound(MediaPlayer player) {
            player.Close();
        }

        public void StopAllSounds() {
            foreach (MediaPlayer player in mPlayers) {
                CloseSound(player);
            }
            mPlayers.Clear();
        }

        public void AddToMediaList(string name, string filePath) {
            mediaList.Add(new MediaQueueItem { name = name, filePath = filePath });
        }

        public void RemoveFromMediaList(MediaQueueItem item) {
            if (mediaList.IndexOf(item) == 0) {
                if (isPlaying) {
                    currentMedia.Stop();
                    if (mediaList.Count <= 1) {
                        currentMedia.Close();
                        isPlaying = false;
                    }
                }
            }
            mediaList.Remove(item);
            if (mediaList.Count > 0) {
                currentMedia.Open(new Uri(mediaList.First().filePath));
                if (isPlaying)
                    currentMedia.Play();
            }
        }

        public void PlayOrPauseQueue() {
            if (isPlaying) {
                currentMedia.Pause();
                isPlaying = false;
            } else if (mediaList.Count > 0) {
                if (currentMedia.Source == null) {
                    currentMedia.Open(new Uri(mediaList.First().filePath));
                    currentMedia.Play();
                } else {
                    currentMedia.Play();
                }
                isPlaying = true;
            }
        }

        private void CurrentMedia_MediaEnded(object sender, EventArgs e) {
            MediaQueueItem firstItem = mediaList.First();
            mediaList.RemoveAt(0);
            mediaList.Add(firstItem);
            currentMedia.Close();
            currentMedia.Open(new Uri(mediaList.First().filePath));
            currentMedia.Play();
        }
    }
}
