using UnityEngine;

namespace Unity.Scripts.Targetable
{
    
    public class SpriteTarget : Colorable
    {
        [SerializeField] private SpriteRenderer sprite;

        private void Awake()
        {
            OriginalColor = sprite.color;
        }
        
        public override void Select()
        {
            sprite.color = targetColor;
        }

        public override void Deselect()
        {
            sprite.color = OriginalColor;
        }
    }
}