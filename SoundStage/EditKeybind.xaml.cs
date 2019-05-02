using System;
using System.Collections.Generic;
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

namespace SoundStage
{
    /// <summary>
    /// Interaction logic for EditKeybind.xaml
    /// </summary>
    public partial class EditKeybind : Window
    {
        SoundStageDataModel soundData = new SoundStageDataModel();
        public KeyCombo keysToBind { get; set; }
        public int bindIDToChange { get; set; }

        public EditKeybind(int id, KeyCombo keys)
        {
            InitializeComponent();
            keysToBind = keys;
            bindIDToChange = id;
            textBoxEditBinding.Text = keysToBind.ToString();
            textBoxEditBinding.Focus();
        }

        void CheckIfReadyToBind() {
            if (keysToBind != null) {
                btnConfirmKeybindEdit.IsEnabled = true;
                btnConfirmKeybindEdit.Visibility = Visibility.Visible;
            } else {
                btnConfirmKeybindEdit.IsEnabled = false;
                btnConfirmKeybindEdit.Visibility = Visibility.Hidden;
            }
        }

        private void textBoxEditBinding_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            e.Handled = true;

            ModifierKeys modifiers = Keyboard.Modifiers;
            Key key = e.Key;

            if (key == Key.System)
                key = e.SystemKey;

            if (((key == Key.Delete || key == Key.Back) && modifiers == ModifierKeys.None)
                || key == Key.S && modifiers.HasFlag(ModifierKeys.Alt) && modifiers.HasFlag(ModifierKeys.Shift)
                 && !modifiers.HasFlag(ModifierKeys.Control)) {
                textBoxEditBinding.Text = "";
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
            textBoxEditBinding.Text = keysToBind.ToString();
            CheckIfReadyToBind();
        }

        private void btnConfirmKeybindEdit_Click(object sender, RoutedEventArgs e) {
            KEYBIND editedBind = (from b in soundData.KEYBINDS
                                 where b.bindID == bindIDToChange
                                 select b).First();
            editedBind.keys = keysToBind.ToString();
            soundData.SaveChanges();
            DialogResult = true;
        }
    }
}
