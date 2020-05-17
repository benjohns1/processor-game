using System;
using SupplyChain;
using UnityEngine;

namespace Unity.Scripts
{
    
    [RequireComponent(typeof(SpriteRenderer))]
    public class MonoPacket : MonoBehaviour
    {
        [SerializeField] private Sprite triangle;
        [SerializeField] private Sprite square;
        private SpriteRenderer icon;
        public Transporter.MovingPacket MovingPacket { get; private set; }

        private void Awake()
        {
            icon = GetComponent<SpriteRenderer>();
        }

        public void Init(Transporter.MovingPacket mp, Vector3 pos)
        {
            transform.position = pos;
            MovingPacket = mp;
            switch (mp.Packet.Shape)
            {
                case Shape.Triangle:
                    icon.sprite = triangle;
                    break;
                case Shape.Square:
                    icon.sprite = square;
                    break;
                default:
                    icon.sprite = null;
                    break;
            }
        }
    }
}