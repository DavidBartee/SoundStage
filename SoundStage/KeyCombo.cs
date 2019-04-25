using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoundStage {
    class KeyCombo {
        public Key key { get; }
        public ModifierKeys modifiers { get; }

        public KeyCombo(Key k, ModifierKeys m) {
            key = k;
            modifiers = m;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            if (modifiers.HasFlag(ModifierKeys.Control))
                sb.Append("Ctrl + ");
            if (modifiers.HasFlag(ModifierKeys.Alt))
                sb.Append("Alt + ");
            if (modifiers.HasFlag(ModifierKeys.Shift))
                sb.Append("Shift + ");
            if (modifiers.HasFlag(ModifierKeys.Windows))
                sb.Append("Win + ");

            sb.Append(key);
            return sb.ToString();
        }
    }
}
