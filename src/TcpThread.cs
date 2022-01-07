using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace Zeloot.Tcp
{
    public class TcpThread : MonoBehaviour
    {
        public static TcpThread Instance;
        private static List<Action> actions;
        public static void New()
        {
            var runtime = new GameObject("[RUNTIME] TcpThread");
            runtime.AddComponent<TcpThread>();
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