using SupplyChain;
using UnityEngine;

namespace Unity.Scripts.UI
{
    public class Buffer : MonoBehaviour
    {
        [SerializeField] private TextMesh text;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Packet packetPrefab;

        private bool showZeroIcon;
        private Vector3 initialScale;

        private void Awake()
        {
            text.text = "";
            icon.sprite = null;
            initialScale = icon.transform.localScale;
        }

        public void Init(bool showIconWhenZero, Shape shape)
        {
            showZeroIcon = showIconWhenZero;
            Set(0, shape);
        }
        
        public void Set(int amount, Shape shape)
        {
            if (amount == 0 && !showZeroIcon)
            {
                text.text = "";
                icon.sprite = null;
                return;
            }
            
            text.text = amount.ToString();
            var ss = packetPrefab.GetSprite(shape);
            icon.sprite = ss.sprite;
            icon.transform.localScale = initialScale * ss.scale;
        }
    }
}