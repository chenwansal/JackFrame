﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JackFrame {

    /// <summary>
    /// 方法执行工具
    /// 本类可以在不继承MonoBehaviour的类中调用Invoke、StartCoroutine
    /// </summary>
    public static class CorutineHelper {

        private static ToolScript _script;

        private static ToolScript Script {
            get {
                if (_script == null) {
                    GameObject go = new GameObject("toolObj");
                    _script = go.AddComponent<ToolScript>();
                }
                return _script;
            }
        }

        public static void StartCoroutine(IEnumerator enumerator) {
            Script.StartCoroutine(enumerator);
        }

    }

    /// <summary>
    /// 工具脚本类
    /// </summary>
    class ToolScript : MonoBehaviour {

    }

}