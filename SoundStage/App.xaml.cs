using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SoundStage
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /*KeyboardListener KListener = new KeyboardListener();

        private void Application_Startup(object sender, StartupEventArgs e) {
            KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
        }

        private void KListener_KeyDown(object sender, RawKeyEventArgs args) {
            //play sound here
            if (args.key == Key.K)
                Application.Current.Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e) {
            KListener.Dispose();
        }*/
    }
}
