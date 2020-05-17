using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;
using Unity.Scripts.Mgr;

namespace Unity.Scripts
{
    public class MonoSource : MonoBehaviour, IMonoNode
    {
        [SerializeField] private GameObject text;
        [SerializeField] private Shape shape;
        [SerializeField] private int packetAmount = 1;
        [SerializeField] private int tickRate = 1;
        [SerializeField] private int maxDownstream = 1;


     
        private Source source;
        private TextMesh textMesh;

        private void Awake()
        {
            var monoTicker = FindObjectOfType<MonoTicker>();
            var ticker = monoTicker.GetComponent<MonoTicker>().ticker;
            
            textMesh = text.GetComponent<TextMesh>();
            textMesh.text = "";
            
            source = new Source(shape, new Rate(packetAmount, tickRate), ticker, maxDownstream);
            source.Updated += (sender, args) =>
            {
                textMesh.text = $"{args.Buffer}";
            };
        }

        public INode GetNode()
        {
            return source;
        }
        public bool Init(NodeGraph g)
        {
            return g.AddNode(source);
        }
    }
}
