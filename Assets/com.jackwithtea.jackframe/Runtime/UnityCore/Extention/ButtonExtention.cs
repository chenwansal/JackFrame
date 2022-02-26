using UnityEngine;
using UnityEngine.UI;

namespace JackFrame {

    public static class ButtonExtention {

        public static void SetText(this Button button, string text) {
            Text t = button.gameObject.GetComponentInChildren<Text>();
            if (t != null) {
                t.text = text;
            }
        }
    }
}