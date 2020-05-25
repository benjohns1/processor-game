using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;
using Buffer = Unity.Scripts.UI.Buffer;
using Ticker = Unity.Scripts.Mgr.Ticker;

namespace Unity.Scripts
{
    public class Source : MonoBehaviour, INode
    {
        [SerializeField] private Buffer buffer;
        [SerializeField] private Shape shape;
        [SerializeField] private Rate rate = new Rate();
        [SerializeField] private int maxDownstream = 1;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;


     
        private SupplyChain.Source source;
        private void Awake()
        {
            var monoTicker = FindObjectOfType<Ticker>();
            var ticker = monoTicker.GetComponent<Ticker>().ticker;
            source = new SupplyChain.Source(shape, rate.Get(), ticker, maxDownstream);
            source.Updated += (sender, args) =>
            {
                buffer.Set(args.Packet.Amount, shape);
            };
            source.Activated += (sender, args) =>
            {
                icon.color = activeColor;
            };
            source.Deactivated += (sender, args) =>
            {
                icon.color = inactiveColor;
            };
        }

        private void Start()
        {
            buffer.Init(true, shape);

        }

        public SupplyChain.Graph.INode GetNode()
        {
            return source;
        }
        public bool Init(NodeGraph g)
        {
            return g.AddNode(source);
        }
        
        public GameObject GameObject()
        {
            return gameObject;
        }
    }
}
