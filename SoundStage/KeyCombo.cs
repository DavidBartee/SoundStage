using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundStage {
    class KeyCombo {
        public Keys key { get; }
        public bool Control { get; }
        public bool Alt { get; }
        public bool Shift { get; }

        public KeyCombo(Keys k, bool ctrl, bool alt, bool shift) {
            key = k;
            Control = ctrl;
            Alt = alt;
            Shift = shift;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            if (Control)
                sb.Append("Ctrl + ");
            if (Alt)
                sb.Append("Alt + ");
            if (Shift)
                sb.Append("Shift + ");

            sb.Append(key);
            return sb.ToString();
        }
    }
}
