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
            icon.sprite = GetSprite(mp.Packet.Shape);
        }

        public Sprite GetSprite(Shape shape)
        {
            switch (shape)
            {
                case Shape.Triangle:
                    return triangle;
                case Shape.Square:
                    return square;
                default:
                    return null;
            }
        }
    }
}