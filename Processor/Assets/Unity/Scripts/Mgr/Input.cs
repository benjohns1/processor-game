using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity.Scripts.Mgr
{
    public class Input : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        private EventSystem es;
        private bool inputHandled;
        
        private readonly Dictionary<KeyCode, Action> toggle = new Dictionary<KeyCode, Action>
        {
            {KeyCode.Mouse0, Action.Primary},
            {KeyCode.Escape, Action.Quit}
        };

        public enum Action
        {
            Primary,
            Quit,
        }

        public enum State
        {
            On,
            Off,
        }

        public event EventHandler<ToggledArgs> Toggled;

        public class ToggledArgs : EventArgs
        {
            public Action Action;
            public State State;
            public bool PointerOverUi;
            
            public bool On(Action action)
            {
                return State == State.On && action == Action;
            }
            
            public bool Off(Action action)
            {
                return State == State.Off && action == Action;
            }
        }

        public Vector2 CursorPosition()
        {
            return cam.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
        }
        
        public Vector3 CursorPosition(float z)
        {
            var pos = cam.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            return new Vector3(pos.x, pos.y, z);
        }

        public void InputHandled()
        {
            inputHandled = true;
        }
        
        private void Awake()
        {
            es = EventSystem.current;
        }
        
        private void Update()
        { 
            CaptureInput();   
        }

        private void CaptureInput()
        {
            if (inputHandled)
            {
                inputHandled = false;
                return;
            }

            var pointerOverUi = es.IsPointerOverGameObject();
            ActionKeys(pointerOverUi);
        }

        private void ActionKeys(bool pointerOverUi)
        {
            foreach (var key in toggle)
            {
                if (UnityEngine.Input.GetKeyDown(key.Key))
                {
                    OnInputReceived(key.Value, State.On, pointerOverUi);
                } else if (UnityEngine.Input.GetKeyUp(KeyCode.Mouse0))
                {
                    OnInputReceived(key.Value, State.Off, pointerOverUi);
                }
            }
        }

        private void OnInputReceived(Action action, State state, bool pointerOverUi)
        {
            Toggled?.Invoke(this, new ToggledArgs
            {
                Action = action,
                State = state,
                PointerOverUi = pointerOverUi
            });
        }
    }
}