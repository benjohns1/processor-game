using UnityEngine;

namespace Unity.Scripts
{
    public class Connectable : MonoBehaviour, IConnectable
    {
        [SerializeField] private GameObject connectableObject;

        public GameObject GameObject() => connectableObject;
    }
}