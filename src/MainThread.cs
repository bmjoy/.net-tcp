using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace Zeloot.Tcp
{
    public class MainThread : MonoBehaviour
    {
        public static MainThread Instance;
        private static List<Action> actions;
        public static void New()
        {
            var runtime = new GameObject("[RUNTIME] TcpThread");
            runtime.AddComponent<MainThread>();
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                actions = new List<Action>();
            }
            else if (Instance != this) Destroy(gameObject);
        }

        private void Update()
        {
            if (actions.Count > 0)
            {
                actions[0]?.Invoke();
                actions.RemoveAt(0);
            }
        }

        public void Add(Action action)
        {
            if (action == null) return;
            actions.Add(action);
        }
    }
}