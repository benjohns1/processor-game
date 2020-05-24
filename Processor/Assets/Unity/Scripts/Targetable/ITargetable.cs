using UnityEngine;

namespace Unity.Scripts.Targetable
{
    public interface ITargetable
    {
        void Select();
        void Deselect();

        GameObject GameObject();
    }
}