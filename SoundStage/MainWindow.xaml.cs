using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoundStage
{
    public partial class MainWindow : Window
    {
        SoundStageDataModel soundData = new SoundStageDataModel();
        SoundManager soundManager = new SoundManager();

        public MainWindow()
        {
            InitializeComponent();
            RefreshSoundList();
        }

        public void RefreshSoundList() {
            var sounds = from s in soundData.SOUNDs
                         select s.filePath;
            var soundList = sounds.ToList();
            foreach (string snd in soundList) {
                /*ListBoxItem newItem = new ListBoxItem();
                newItem.Content = snd;
                listBoxSounds.Items.Add(newItem);*/
                AddSound(snd, false, true);
            }
        }

        public void AddSound(string filePath, bool doBackup, bool isInitialLoad)
        {
            if (File.Exists(filePath)) {
                int startAt = filePath.LastIndexOf("\\") + 1;
                string soundName = filePath.Substring(startAt, filePath.Length - startAt);
                if (!isInitialLoad) {
                    SOUND newSound = new SOUND();
                    newSound.filePath = filePath;
                    newSound.isBackup = doBackup;
                    newSound.name = soundName;
                    soundData.SOUNDs.Add(newSound);
                    soundData.SaveChanges();
                }

                ListBoxItem newItem = new ListBoxItem();
                newItem.Content = soundName;
                newItem.MouseDoubleClick += (sender, e) => listItem_MouseDoubleClick(sender, e, filePath);
                listBoxSounds.Items.Add(newItem);
            }
        }

        void listItem_MouseDoubleClick(object sender, MouseButtonEventArgs e, string filePath) {
            //DependencyObject de = (DependencyObject)e.OriginalSource;
            if (sender == null || !(sender is ListBoxItem))
                return;
            ListBoxItem item = (ListBoxItem)sender;
            PlaySound(filePath);
        }

        public void PlaySound(string filePath) {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            int numBytes = 4;
            string hexBytes = "";
            for (int i = 0; i < numBytes; i++) {
                hexBytes += string.Format($"{fs.ReadByte():X2}");
            }
            if (hexBytes.Substring(0, 8) == "52494646") {
                SoundPlayer player = new SoundPlayer(filePath);
                player.Play();
            } else if (hexBytes.Substring(0, 6) == "494433") {
                MediaPlayer mp = new MediaPlayer();
                mp.Open(new Uri(filePath));
                mp.Play();
            }
        }

        private void listBoxSounds_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true)) {
                var fileNames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                foreach (string s in fileNames) {
                    if (!File.Exists(s)) {
                        return;
                    } else {
                        AddSound(s, false, false);
                    }
                }
            }
        }

        private void btnAddSound_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            string pathToSend = "";
            if (dialog.ShowDialog() == true) {
                pathToSend = dialog.FileName;
            }
            if (File.Exists(pathToSend)) {
                AddSound(pathToSend, false, false);
            }
        }
    }
}
