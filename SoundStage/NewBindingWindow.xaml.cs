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
                }
            }
        }
    }
}
