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

            listBoxSounds.Items.Clear();

            foreach (string snd in soundList) {
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

                ContextMenu listMenu = new ContextMenu();
                listMenu.Name = soundName.Substring(0, soundName.Length - 1 - soundName.LastIndexOf(".")) + "menu";
                

                MenuItem delete = new MenuItem();
                delete.Header = "Delete";
                delete.IsCheckable = false;
                delete.Click += (sender, e) => listItem_Delete(sender, e, filePath);
                listMenu.Items.Add(delete);

                newItem.ContextMenu = listMenu;
                listBoxSounds.Items.Add(newItem);
            }
        }

        void listItem_MouseDoubleClick(object sender, MouseButtonEventArgs e, string filePath) {
            if (sender == null || !(sender is ListBoxItem))
                return;
            ListBoxItem item = (ListBoxItem)sender;
            PlaySound(filePath);
        }

        void listItem_Delete(object sender, RoutedEventArgs e, string filePath) {
            SOUND soundToRemove = (from s in soundData.SOUNDs
                         where s.filePath == filePath
                         select s).First();
            soundData.SOUNDs.Remove(soundToRemove);
            soundData.SaveChanges();
            RefreshSoundList();
        }

        public void PlaySound(string filePath) {
            soundManager.PlaySound(filePath);
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

        private void btnNewBinding_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
