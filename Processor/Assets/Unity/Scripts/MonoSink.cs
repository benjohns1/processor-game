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
        [SerializeField] private Rate rate = new Rate();
        [SerializeField] private int maxUpstream = 1;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;

        private Sink sink;

        private void Awake()
        {
            shapeCountText.text = "";
            var ss = packetPrefab.GetSprite(shape);
            shapeIcon.sprite = ss.sprite;
            shapeIcon.transform.localScale = ss.scale * 2;
            
            var ticker = FindObjectOfType<MonoTicker>().ticker;
            var score = FindObjectOfType<MonoScore>().score;

            var filter = SupplyChain.Filter.AllowShapes(shape);
            sink = new Sink(score, filter, rate.Get(), ticker, maxUpstream);
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
