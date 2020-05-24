using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Unity.Scripts.UI
{
    public class Button
    {
        private readonly UnityEngine.UI.Button button;
        private readonly Image img;

        public event EventHandler<EventArgs> Activated;

        public Button(UnityEngine.UI.Button button)
        {
            this.button = button;
            img = button.GetComponent<Image>();
            button.onClick.AddListener(delegate
            {
                Activate();
                OnActivated();
            });
        }

        public void SetGroup(IEnumerable<Button> group)
        {
            foreach (var other in group)
            {
                if (other == this)
                {
                    continue;
                }
                other.Activated += (sender, args) => Deactivate();
            }
        }

        private void Activate()
        {
            img.color = button.colors.pressedColor;
        }

        private void Deactivate()
        {
            img.color = button.colors.normalColor;
        }

        private void OnActivated()
        {
            Activated?.Invoke(this, new EventArgs());
        }
    }
}