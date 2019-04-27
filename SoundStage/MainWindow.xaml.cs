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
using static SoundStage.KeyboardHook;
using System.Windows.Forms;

namespace SoundStage {

    public class KeyBindWithSounds {
        public KeyCombo keyCombo;
        public List<string> sounds = new List<string>();
    }

    public partial class MainWindow : Window {

        SoundStageDataModel soundData = new SoundStageDataModel();
        SoundManager soundManager = new SoundManager();
        private KeyboardHook _hook;
        List<KeyBindWithSounds> allBinds = new List<KeyBindWithSounds>();

        public MainWindow() {
            InitializeComponent();
            RefreshSoundList();
            RefreshKeyBindList();
            RefreshOnScreenButtons();
            _hook = new KeyboardHook();
            _hook.KeyDown += new KeyboardHook.HookEventHandler(OnHookKeyDown);
        }

        void OnHookKeyDown(object sender, HookEventArgs e) {
            KeyCombo hookedCombo = new KeyCombo(e.Key, e.Control, e.Alt, e.Shift);
            if (hookedCombo.key == Keys.S && hookedCombo.Alt && hookedCombo.Shift && !hookedCombo.Control) {
                soundManager.StopAllSounds();
            } else {
                foreach (KeyBindWithSounds bind in allBinds) {
                    if (bind.keyCombo.Equals(hookedCombo)) {
                        foreach (string sound in bind.sounds) {
                            PlaySound(sound);
                        }
                    }
                }
            }
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
            listViewKeyBinds.Items.Add(new KeyBindListing() { KeyBindID = 0, Keys = "Alt + Shift + S", Sound = "Stop all sounds" });

            var keybinds = from b in soundData.KEYBINDS
                           join sound in soundData.SOUNDs on b.soundID equals sound.soundID
                           select new { KeyBindID = b.bindID, Keys = b.keys, Sound = sound.name, sound.filePath };
            var bindlist = keybinds.ToList();

            foreach (var k in bindlist) {
                KeyCombo keys = new KeyCombo(k.Keys);
                int indexToEdit = -1;
                listViewKeyBinds.Items.Add(k);
                foreach (KeyBindWithSounds bind in allBinds) {
                    if (bind.keyCombo == keys) {
                        indexToEdit = allBinds.IndexOf(bind);
                        break;
                    }
                }
                if (indexToEdit == -1) {
                    KeyBindWithSounds newBind = new KeyBindWithSounds();
                    newBind.keyCombo = keys;
                    newBind.sounds.Add(k.filePath);
                    allBinds.Add(newBind);
                } else if (!allBinds[indexToEdit].sounds.Contains(k.filePath)) {
                    allBinds[indexToEdit].sounds.Add(k.filePath);
                }
            }
        }

        public void RefreshOnScreenButtons() {
            wrapPanelSoundButtons.Children.Clear();

            var buttonBinds = from b in soundData.BUTTONBINDS
                              join sound in soundData.SOUNDs on b.soundID equals sound.soundID
                              select new { b.bindID, sound.filePath, sound.name };
            var bindList = buttonBinds.ToList();

            foreach (var bind in bindList) {
                System.Windows.Controls.Button button = new System.Windows.Controls.Button();
                button.Content = bind.name.Substring(0, bind.name.LastIndexOf("."));
                button.Click += (sender, e) => ButtonPlaySound(sender, e, bind.filePath);
                button.Margin = new Thickness(4.0);
                button.Width = 200;
                button.Height = 180;
                wrapPanelSoundButtons.Children.Add(button);
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

                System.Windows.Controls.ContextMenu listMenu = new System.Windows.Controls.ContextMenu();
                listMenu.Name = soundName.Substring(0, soundName.Length - 1 - soundName.LastIndexOf(".")) + "menu";


                System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
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

        public void ButtonPlaySound(object sender, RoutedEventArgs e, string filePath) {
            soundManager.PlaySound(filePath);
        }

        private void listBoxSounds_Drop(object sender, System.Windows.DragEventArgs e) {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true)) {
                var fileNames = e.Data.GetData(System.Windows.DataFormats.FileDrop, true) as string[];
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
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
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
            RefreshOnScreenButtons();
        }

        private void btnStopSounds_Click(object sender, RoutedEventArgs e) {
            soundManager.StopAllSounds();
        }
    }
}
