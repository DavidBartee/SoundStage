using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SoundStage {
    /// <summary>
    /// Interaction logic for NewBindingWindow.xaml
    /// </summary>
    public partial class NewBindingWindow : Window {

        SoundStageDataModel soundData = new SoundStageDataModel();
        Dictionary<ListBoxItem, string> soundsAvailable = new Dictionary<ListBoxItem, string>();
        List<string> soundsToBind = new List<string>();
        bool isKeyBinding = true;
        KeyCombo keysToBind;


        public NewBindingWindow() {
            InitializeComponent();

            var sounds = from s in soundData.SOUNDs
                         select s.filePath;
            var soundList = sounds.ToList();

            listBoxChooseSound.Items.Clear();

            foreach (string snd in soundList) {
                if (File.Exists(snd)) {
                    int startAt = snd.LastIndexOf("\\") + 1;
                    string soundName = snd.Substring(startAt, snd.Length - startAt);

                    ListBoxItem newItem = new ListBoxItem();
                    newItem.Content = soundName;
                    //newItem.MouseDoubleClick += (sender, e) => listItem_MouseDoubleClick(sender, e, snd);
                    listBoxChooseSound.Items.Add(newItem);
                    soundsAvailable.Add(newItem, snd);
                }
            }
            CheckIfReadyToBind();
        }

        void CheckIfReadyToBind() {
            if ((keysToBind != null || isKeyBinding == false) && soundsToBind.Count > 0) {
                btnCreateBinding.IsEnabled = true;
                btnCreateBinding.Visibility = Visibility.Visible;
            } else {
                btnCreateBinding.IsEnabled = false;
                btnCreateBinding.Visibility = Visibility.Hidden;
            }
        }

        private void textBoxBinding_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            e.Handled = true;

            ModifierKeys modifiers = Keyboard.Modifiers;
            Key key = e.Key;

            if (key == Key.System)
                key = e.SystemKey;

            if ((key == Key.Delete || key == Key.Back) && modifiers == ModifierKeys.None) {
                textBoxBinding.Text = "";
                keysToBind = null;
                CheckIfReadyToBind();
                return;
            }
            if (key == Key.RightCtrl || key == Key.LeftCtrl || key == Key.LeftShift || key == Key.RightShift
                 || key == Key.LeftAlt || key == Key.RightAlt || key == Key.LWin || key == Key.RWin
                  || key == Key.Clear || key == Key.OemClear || key == Key.Apps) {
                return;
            }

            keysToBind = new KeyCombo((Keys)KeyInterop.VirtualKeyFromKey(key), modifiers.HasFlag(ModifierKeys.Control) ? true : false,
                modifiers.HasFlag(ModifierKeys.Alt) ? true : false,
                modifiers.HasFlag(ModifierKeys.Shift) ? true : false);
            textBoxBinding.Text = keysToBind.ToString();
            CheckIfReadyToBind();
        }

        private void listBoxChooseSound_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            soundsToBind.Clear();
            foreach (KeyValuePair<ListBoxItem, string> item in soundsAvailable) {
                if (e.AddedItems.Contains(item.Key)) {
                    soundsToBind.Add(item.Value);
                }
            }
            CheckIfReadyToBind();
        }

        private void comboBoxBindType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems[0] == null || textBoxBinding == null || labelKeys == null)
                return;
            if ((e.AddedItems[0] as ComboBoxItem).Content as string == "Keyboard") {
                isKeyBinding = true;
                textBoxBinding.Visibility = Visibility.Visible;
                labelKeys.Visibility = Visibility.Visible;
            }
            if ((e.AddedItems[0] as ComboBoxItem).Content as string == "On-Screen Button") {
                isKeyBinding = false;
                keysToBind = null;
                textBoxBinding.Text = "";
                textBoxBinding.Visibility = Visibility.Hidden;
                labelKeys.Visibility = Visibility.Hidden;
            }
            CheckIfReadyToBind();
        }

        private void btnCreateBinding_Click(object sender, RoutedEventArgs e) {
            if (isKeyBinding) {
                foreach (string s in soundsToBind) {
                    KEYBIND newBind = new KEYBIND();
                    newBind.keys = keysToBind.ToString();
                    var foundID = from snd in soundData.SOUNDs
                                  where snd.filePath == s
                                  select snd.soundID;
                    newBind.soundID = foundID.First();
                    soundData.KEYBINDS.Add(newBind);
                }
            } else {
                foreach (string s in soundsToBind) {
                    BUTTONBIND newBtnBind = new BUTTONBIND();
                    var foundID = from snd in soundData.SOUNDs
                                  where snd.filePath == s
                                  select snd.soundID;
                    newBtnBind.soundID = foundID.First();
                    soundData.BUTTONBINDS.Add(newBtnBind);
                }
            }
            soundData.SaveChanges();
            DialogResult = true;
        }
    }
}
