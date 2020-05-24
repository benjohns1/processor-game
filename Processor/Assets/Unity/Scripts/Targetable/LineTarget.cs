using System;
using UnityEngine;

namespace Unity.Scripts.Targetable
{
    public class LineTarget : Colorable
    {
        [SerializeField] private LineRenderer line;
        [SerializeField] private Color endTargetColor;
        private Color endOriginalColor;

        private void Awake()
        {
            OriginalColor = line.startColor;
            endOriginalColor = line.endColor;
        }
        
        public override void Select()
        {
            line.startColor = targetColor;
            line.endColor = endTargetColor;
        }

        public override void Deselect()
        {
            line.startColor = OriginalColor;
            line.endColor = endOriginalColor;
        }
    }
}