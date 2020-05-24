﻿using System;
using UnityEngine;

namespace Unity.Scripts.Targetable
{
    public abstract class Colorable : MonoBehaviour, ITargetable
    {
        [SerializeField] protected GameObject targetObject;
        [SerializeField] protected Color targetColor;
        protected Color OriginalColor;

        public abstract void Select();

        public abstract void Deselect();

        public virtual GameObject GameObject() => targetObject;
    }
}