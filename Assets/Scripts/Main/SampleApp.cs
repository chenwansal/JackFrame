using UnityEngine;
using JackFrame;

namespace MySampleApp {

    public class SampleApp : MonoBehaviour {

#if JACK_FRAME_DEV
        [UnityEditor.MenuItem("JackFrame/RunSampleApp")]
#endif
        public static void Run() {

        }

    }

}