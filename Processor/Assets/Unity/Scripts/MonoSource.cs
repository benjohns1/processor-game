using SupplyChain;
using UnityEngine;

namespace Unity.Scripts
{
    public class MonoSource : MonoBehaviour, IMonoNode
    {
        [SerializeField] private GameObject text;
        [SerializeField] private GameObject icon;
        [SerializeField] private GameObject shapeIcon;
     
        private MonoTicker monoTicker;   
        private Source source;
        private Ticker ticker;
        private TextMesh textMesh;

        private void Awake()
        {
            monoTicker = FindObjectOfType<MonoTicker>();
            textMesh = text.GetComponent<TextMesh>();
            textMesh.text = "";
            ticker = monoTicker.GetComponent<MonoTicker>().ticker;
            source = new Source(Shape.Triangle, new Source.Rate(2, 1), ticker);
            source.BufferUpdated += (sender, args) =>
            {
                textMesh.text = $"{args.Buffer}";
            };
        }

        public INode GetNode()
        {
            return source;
        }
    }
}
