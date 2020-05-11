using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;

namespace Unity.Scripts
{
    public class MonoSource : MonoBehaviour, IMonoNode
    {
        [SerializeField] private GameObject text;
        [SerializeField] private Shape shape;
     
        private Source source;
        private TextMesh textMesh;

        private void Awake()
        {
            var monoTicker = FindObjectOfType<MonoTicker>();
            var ticker = monoTicker.GetComponent<MonoTicker>().ticker;
            
            textMesh = text.GetComponent<TextMesh>();
            textMesh.text = "";
            
            source = new Source(shape, new Source.Rate(2, 1), ticker);
            source.Updated += (sender, args) =>
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
