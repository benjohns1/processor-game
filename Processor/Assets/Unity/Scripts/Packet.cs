using System;
using System.Linq;
using SupplyChain;
using UnityEngine;

namespace Unity.Scripts
{
    [Serializable]
    public class ShapeSprite
    {
        public Shape shape;
        public Sprite sprite;
        public Vector2 scale = Vector2.one;
    }
    
    [RequireComponent(typeof(SpriteRenderer))]
    public class Packet : MonoBehaviour
    {
        [SerializeField] private ShapeSprite[] shapes;

        private SpriteRenderer icon;
        public SupplyChain.Transporter.MovingPacket MovingPacket { get; private set; }

        private void Awake()
        {
            icon = GetComponent<SpriteRenderer>();
        }

        public void Init(SupplyChain.Transporter.MovingPacket mp, Vector3 pos)
        {
            transform.position = pos;
            MovingPacket = mp;
            var ss = GetSprite(mp.Packet.Shape);
            icon.sprite = ss.sprite;
            icon.transform.localScale = ss.scale;
        }

        public ShapeSprite GetSprite(Shape shape)
        {
            return shapes.FirstOrDefault(shapeSprite => shapeSprite.shape == shape);
        }
    }
}