using UnityEngine;

namespace JackFrame {

    public static class KeyCodeExtention {

        public static string ToNonePrefixString(this KeyCode k) {
            return k.ToString().Replace("Alpha", "").Replace("Keypad", "");
        }

    }
}