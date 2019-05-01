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
using System.Collections.Specialized;

namespace SoundStage {

    public class KeyBindWithSounds {
        public KeyCombo keyCombo;
        public List<string> sounds = new List<string>();
    }
    public class ListedSoundWithID {
        public ListBoxItem listItem;
        public int soundID;
    }

    public partial class MainWindow : Window {

        SoundStageDataModel soundData = new SoundStageDataModel();
        SoundManager soundManager = new SoundManager();
        private KeyboardHook _hook;
        List<KeyBindWithSounds> allBinds = new List<KeyBindWithSounds>();

        Rectangle background;
        Rectangle playHead;
        Timer timerForCanvas = new Timer();

        public MainWindow() {
            InitializeComponent();
            RefreshSoundList();
            RefreshKeyBindList();
            RefreshOnScreenButtons();
            soundManager.mediaList.CollectionChanged += MediaList_Changed;
            _hook = new KeyboardHook();
            _hook.KeyDown += new KeyboardHook.HookEventHandler(OnHookKeyDown);

            canvasCurrentSong.Width = 394;
            canvasCurrentSong.Height = 87;

            background = new Rectangle();
            background.Width = canvasCurrentSong.Width;
            background.Height = canvasCurrentSong.Height;
            background.Stroke = new SolidColorBrush(Color.FromArgb(255, 100, 0, 200));
            canvasCurrentSong.Children.Add(background);

            playHead = new Rectangle();
            playHead.Width = 4.0;
            playHead.Height = canvasCurrentSong.Height;
            playHead.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            canvasCurrentSong.Children.Add(playHead);

            timerForCanvas.Interval = 4;
            timerForCanvas.Tick += UpdateCanvas;
            timerForCanvas.Start();
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

        void UpdateCanvas(object sender, EventArgs e) {
            double playbackPosition = 0;
            if (soundManager.currentMedia != null && soundManager.currentMedia.Source != null && soundManager.currentMedia.NaturalDuration.HasTimeSpan) {
                playbackPosition = soundManager.currentMedia.Position.TotalSeconds / soundManager.currentMedia.NaturalDuration.TimeSpan.TotalSeconds;
            }
            Canvas.SetLeft(playHead, playbackPosition * canvasCurrentSong.Width);
        }

        public void RefreshSoundList() {
            var sounds = from s in soundData.SOUNDs
                         select s;
            var soundList = sounds.ToList();

            listBoxSounds.Items.Clear();

            foreach (SOUND snd in soundList) {
                AddSound(snd.filePath, false, true, snd.soundID);
            }
        }

        public void RefreshKeyBindList() {
            listViewKeyBinds.Items.Clear();
            allBinds.Clear();
            listViewKeyBinds.Items.Add(new KeyBindListing() { KeyBindID = 0, Keys = "Alt + Shift + S", Sound = "Stop all sounds" });

            var keybinds = from b in soundData.KEYBINDS
                           join sound in soundData.SOUNDs on b.soundID equals sound.soundID
                           select new KeyBindListing { KeyBindID = b.bindID, Keys = b.keys, Sound = sound.name, filePath = sound.filePath };
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

                System.Windows.Controls.ContextMenu buttonMenu = new System.Windows.Controls.ContextMenu();

                System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
                delete.Header = "Delete";
                delete.IsCheckable = false;
                delete.Click += (sender, e) => soundButton_Delete(sender, e, bind.bindID);
                buttonMenu.Items.Add(delete);
                button.ContextMenu = buttonMenu;

                button.Margin = new Thickness(4.0);
                button.Width = 200;
                button.Height = 180;
                wrapPanelSoundButtons.Children.Add(button);
            }
        }

        public void RefreshMediaQueueList() {
            listBoxQueuedSounds.Items.Clear();
            foreach (MediaQueueItem item in soundManager.mediaList) {
                ListBoxItem newItem = new ListBoxItem();
                newItem.Content = item.name;

                System.Windows.Controls.ContextMenu listMenu = new System.Windows.Controls.ContextMenu();

                System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
                delete.Header = "Remove From Queue";
                delete.IsCheckable = false;
                delete.Click += (sender, e) => listItemQueuedSound_RemoveSound(sender, e, item);
                listMenu.Items.Add(delete);

                newItem.ContextMenu = listMenu;
                listBoxQueuedSounds.Items.Add(newItem);
            }
            if (soundManager.isPlaying)
                rectPausePlay.OpacityMask = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/SoundStage;component/images/pause.png")));
            else
                rectPausePlay.OpacityMask = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/SoundStage;component/images/play.png")));
        }

        public void AddSound(string filePath, bool doBackup, bool isInitialLoad, int soundID = -1) {
            if (File.Exists(filePath)) {
                int startAt = filePath.LastIndexOf("\\") + 1;
                int addedSoundID = soundID;
                string soundName = filePath.Substring(startAt, filePath.Length - startAt);
                if (!isInitialLoad) {
                    SOUND newSound = new SOUND();
                    newSound.filePath = filePath;
                    newSound.isBackup = doBackup;
                    newSound.name = soundName;
                    soundData.SOUNDs.Add(newSound);
                    soundData.SaveChanges();
                    addedSoundID = (from s in soundData.SOUNDs
                                    orderby s.soundID descending
                                    select s.soundID).First();
                }

                ListBoxItem newItem = new ListBoxItem();
                newItem.Content = soundName;

                newItem.DataContext = addedSoundID;
                newItem.MouseDoubleClick += (sender, e) => listItem_MouseDoubleClick(sender, e, filePath);
                newItem.PreviewMouseMove += listBoxItemSound_PreviewMouseMoveEvent;

                System.Windows.Controls.ContextMenu listMenu = new System.Windows.Controls.ContextMenu();

                System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
                delete.Header = "Delete";
                delete.IsCheckable = false;
                delete.Click += (sender, e) => listItem_Delete(sender, e, addedSoundID);
                listMenu.Items.Add(delete);

                newItem.ContextMenu = listMenu;
                listBoxSounds.Items.Add(newItem);
            }
        }
        #region Event Handlers
        void listItem_MouseDoubleClick(object sender, MouseButtonEventArgs e, string filePath) {
            if (sender == null || !(sender is ListBoxItem))
                return;
            ListBoxItem item = (ListBoxItem)sender;
            PlaySound(filePath);
        }

        void listItem_Delete(object sender, RoutedEventArgs e, int soundID) {
            SOUND soundToRemove = (from s in soundData.SOUNDs
                                   where s.soundID == soundID
                                   select s).First();
            soundData.SOUNDs.Remove(soundToRemove);
            soundData.SaveChanges();
            RefreshSoundList();
            RefreshKeyBindList();
            RefreshOnScreenButtons();
        }

        void soundButton_Delete(object sender, RoutedEventArgs e, int bindID) {
            BUTTONBIND bindToRemove = (from b in soundData.BUTTONBINDS
                                   where b.bindID == bindID
                                   select b).First();
            soundData.BUTTONBINDS.Remove(bindToRemove);
            soundData.SaveChanges();
            RefreshOnScreenButtons();
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

        private void keyBind_Edit(object sender, RoutedEventArgs e) {
            KeyBindListing itemToEdit = (KeyBindListing)listViewKeyBinds.SelectedItem;
            EditKeybind ekb = new EditKeybind(itemToEdit.KeyBindID, new KeyCombo(itemToEdit.Keys));
            ekb.Owner = this;
            ekb.ShowDialog();
            RefreshKeyBindList();
        }

        private void keyBind_Delete(object sender, RoutedEventArgs e) {
            KeyBindListing itemToDelete = (KeyBindListing)listViewKeyBinds.SelectedItem;
            if (itemToDelete == null || listViewKeyBinds.SelectedIndex < 1) {
                return;
            }
            KEYBIND bindToRemove = (from k in soundData.KEYBINDS
                                   where k.bindID == itemToDelete.KeyBindID
                                   select k).First();
            soundData.KEYBINDS.Remove(bindToRemove);
            soundData.SaveChanges();
            RefreshKeyBindList();
        }

        void listBoxItemSound_PreviewMouseMoveEvent(object sender, System.Windows.Input.MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && sender is ListBoxItem) {
                ListBoxItem draggedItem = sender as ListBoxItem;
                if (sender != null && e != null && draggedItem != null && draggedItem.DataContext != null) {
                    DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, System.Windows.DragDropEffects.Copy);
                    draggedItem.IsSelected = true;
                }
            }
        }

        private void listBoxQueuedSounds_Drop(object sender, System.Windows.DragEventArgs e) {
            if (e.Data.GetFormats().Count() > 0) {
                int searchID = (int)e.Data.GetData(typeof(int));
                SOUND soundToAdd = (from s in soundData.SOUNDs
                                   where s.soundID == searchID
                                   select s).First();
                soundManager.AddToMediaList(soundToAdd.name, soundToAdd.filePath);
                RefreshMediaQueueList();
            }
        }

        void listItemQueuedSound_RemoveSound(object sender, RoutedEventArgs e, MediaQueueItem item) {
            soundManager.RemoveFromMediaList(item);
            RefreshMediaQueueList();
        }

        private void MediaList_Changed(object sender, NotifyCollectionChangedEventArgs e) {
            RefreshMediaQueueList();
        }
        #endregion

        private void btnPausePlay_Click(object sender, RoutedEventArgs e) {
            soundManager.PlayOrPauseQueue();
            if (soundManager.isPlaying)
                rectPausePlay.OpacityMask = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/SoundStage;component/images/pause.png")));
            else
                rectPausePlay.OpacityMask = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/SoundStage;component/images/play.png")));
        }
    }
}
