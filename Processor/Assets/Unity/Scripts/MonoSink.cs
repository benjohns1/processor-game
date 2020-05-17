using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;
using Unity.Scripts.Mgr;

namespace Unity.Scripts
{
    public class MonoSink : MonoBehaviour, IMonoNode
    {
        [SerializeField] private TextMesh shapeCountText;
        [SerializeField] private SpriteRenderer shapeIcon;
        [SerializeField] private MonoPacket packetPrefab;
        [SerializeField] private Shape shape;
        [SerializeField] private int packetAmount = 1;
        [SerializeField] private int tickRate = 1;
        [SerializeField] private int maxUpstream = 1;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;

        private Sink sink;

        private void Awake()
        {
            shapeCountText.text = "";
            shapeIcon.sprite = packetPrefab.GetSprite(shape);
            
            var ticker = FindObjectOfType<MonoTicker>().ticker;
            var score = FindObjectOfType<MonoScore>().score;

            var filter = Filter.AllowShapes(shape);
            sink = new Sink(score, filter, new Rate(packetAmount, tickRate), ticker, maxUpstream);
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

        public INode GetNode()
        {
            return sink;
        }
        public bool Init(NodeGraph g)
        {
            return g.AddNode(sink);
        }
    }
}
