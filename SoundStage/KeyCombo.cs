using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundStage {
    public class KeyCombo : IEquatable<KeyCombo> {
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

        public KeyCombo(string keystring) {
            string[] separators = { " + " };
            string[] parts = keystring.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts) {
                switch (part) {
                    case "Ctrl":
                        Control = true;
                        break;
                    case "Alt":
                        Alt = true;
                        break;
                    case "Shift":
                        Shift = true;
                        break;
                    default:
                        KeysConverter converter = new KeysConverter();
                        key = (Keys)converter.ConvertFromString(part);
                        break;
                }
            }
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

        public bool Equals(KeyCombo other) {
            if (this.ToString() == other.ToString())
                return true;
            else
                return false;
        }
    }
}
