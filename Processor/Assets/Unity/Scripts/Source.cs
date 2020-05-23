using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;
using Unity.Scripts.Mgr;
using Ticker = Unity.Scripts.Mgr.Ticker;

namespace Unity.Scripts
{
    public class Source : MonoBehaviour, INode
    {
        [SerializeField] private TextMesh shapeCountText;
        [SerializeField] private SpriteRenderer shapeIcon;
        [SerializeField] private Packet packetPrefab;
        [SerializeField] private Shape shape;
        [SerializeField] private Rate rate = new Rate();
        [SerializeField] private int maxDownstream = 1;


     
        private SupplyChain.Source source;
        private void Awake()
        {
            var monoTicker = FindObjectOfType<Ticker>();
            var ticker = monoTicker.GetComponent<Ticker>().ticker;
            
            shapeCountText.text = "";
            var ss = packetPrefab.GetSprite(shape);
            shapeIcon.sprite = ss.sprite;
            shapeIcon.transform.localScale = ss.scale * 2;
            
            source = new SupplyChain.Source(shape, rate.Get(), ticker, maxDownstream);
            source.Updated += (sender, args) =>
            {
                shapeCountText.text = $"{args.Buffer}";
            };
        }

        public SupplyChain.Graph.INode GetNode()
        {
            return source;
        }
        public bool Init(NodeGraph g)
        {
            return g.AddNode(source);
        }
    }
}
