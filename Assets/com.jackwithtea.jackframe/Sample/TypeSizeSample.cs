using System;
using UnityEngine;

namespace JackFrame.Sample {

    public static class TypeSizeSample {

        [UnityEditor.MenuItem("JackFrame/Sample/TypeSizeSample")]
        public static void Run() {

            string s1 = "中国a";
            string s2 = "aaa";
            string s3 = "中中国";

            Debug.Log(s1.Length);
            Debug.Log(s2.Length);
            Debug.Log(s3.Length);

        }

    }

}