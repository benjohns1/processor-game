using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;
using Unity.Scripts.Mgr;
using Score = Unity.Scripts.Mgr.Score;
using Ticker = Unity.Scripts.Mgr.Ticker;

namespace Unity.Scripts
{
    public class Sink : MonoBehaviour, INode
    {
        [SerializeField] private TextMesh shapeCountText;
        [SerializeField] private SpriteRenderer shapeIcon;
        [SerializeField] private Packet packetPrefab;
        [SerializeField] private Shape shape;
        [SerializeField] private Rate rate = new Rate();
        [SerializeField] private int maxUpstream = 1;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;

        private SupplyChain.Sink sink;

        private void Awake()
        {
            shapeCountText.text = "";
            var ss = packetPrefab.GetSprite(shape);
            shapeIcon.sprite = ss.sprite;
            shapeIcon.transform.localScale = ss.scale * 2;
            
            var ticker = FindObjectOfType<Ticker>().ticker;
            var score = FindObjectOfType<Score>().score;

            var filter = SupplyChain.Filter.AllowShapes(shape);
            sink = new SupplyChain.Sink(score, filter, rate.Get(), ticker, maxUpstream);
            sink.Updated += (sender, args) =>
            {
                shapeCountText.text = $"{args.Buffer}";
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

        public SupplyChain.Graph.INode GetNode()
        {
            return sink;
        }
        public bool Init(NodeGraph g)
        {
            return g.AddNode(sink);
        }
    }
}
