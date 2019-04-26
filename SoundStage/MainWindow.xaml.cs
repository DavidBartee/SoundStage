using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoundStage {
    public partial class MainWindow : Window {
        #region HOTKEY_MANAGEMENT
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int WM_HOTKEY = 0x0312;
        const int MYACTION_HOTKEY_ID = 1;

        private void OnSourceInitialized(object sender, EventArgs e) {
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            if (msg == WM_HOTKEY && wParam.ToInt32() == MYACTION_HOTKEY_ID) {
                var find = from s in soundData.SOUNDs
                           select s.filePath;
                PlaySound(find.First());
            }

            return IntPtr.Zero;
        }
        #endregion
        SoundStageDataModel soundData = new SoundStageDataModel();
        SoundManager soundManager = new SoundManager();

        public MainWindow() {
            InitializeComponent();
            RefreshSoundList();
            RefreshKeyBindList();
            RegisterHotKey(new WindowInteropHelper(this).Handle, MYACTION_HOTKEY_ID, 0, 0x7B);
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

        public void RefreshKeyBindList() {
            listViewKeyBinds.Items.Clear();
            listViewKeyBinds.Items.Add(new KeyBindListing() { KeyBindID = 0, Keys = "Alt+Shift+S", Sound = "Stop all sounds"});

            var keybinds = from b in soundData.KEYBINDS
                           join sound in soundData.SOUNDs on b.soundID equals sound.soundID
                           select new { KeyBindID = b.bindID, Keys = b.keys, Sound = sound.name };
            var bindlist = keybinds.ToList();

            foreach (var k in bindlist) {
                listViewKeyBinds.Items.Add(k);
            }
        }

        public void AddSound(string filePath, bool doBackup, bool isInitialLoad) {
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

        private void listBoxSounds_Drop(object sender, DragEventArgs e) {
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

        private void btnAddSound_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            string pathToSend = "";
            if (dialog.ShowDialog() == true) {
                pathToSend = dialog.FileName;
            }
            if (File.Exists(pathToSend)) {
                AddSound(pathToSend, false, false);
            }
        }

        private void btnNewBinding_Click(object sender, RoutedEventArgs e) {
            NewBindingWindow nbw = new NewBindingWindow();
            nbw.Owner = this;
            nbw.ShowDialog();
            RefreshKeyBindList();
        }
    }
}
