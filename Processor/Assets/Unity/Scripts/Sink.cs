using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;
using Buffer = Unity.Scripts.UI.Buffer;
using Score = Unity.Scripts.Mgr.Score;
using Ticker = Unity.Scripts.Mgr.Ticker;

namespace Unity.Scripts
{
    public class Sink : MonoBehaviour, INode
    {
        [SerializeField] private Buffer buffer;
        [SerializeField] private Shape shape;
        [SerializeField] private Rate rate = new Rate();
        [SerializeField] private int maxUpstream = 1;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;

        private SupplyChain.Sink sink;

        private void Awake()
        {
            var ticker = FindObjectOfType<Ticker>().ticker;
            var score = FindObjectOfType<Score>().score;

            var filter = SupplyChain.Filter.AllowShapes(shape);
            sink = new SupplyChain.Sink(score, filter, rate.Get(), ticker, maxUpstream);
            sink.Updated += (sender, args) =>
            {
                buffer.Set(args.Packet.Amount, shape);
            };
            sink.Activated += (sender, args) =>
            {
                icon.color = activeColor;
            };
            sink.Deactivated += (sender, args) =>
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
            return sink;
        }
        public bool Init(NodeGraph g)
        {
            return g.AddNode(sink);
        }
        
        public GameObject GameObject()
        {
            return gameObject;
        }
    }
}
