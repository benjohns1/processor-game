using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;
using Unity.Scripts.Mgr;

namespace Unity.Scripts
{
    public class MonoSource : MonoBehaviour, IMonoNode
    {
        [SerializeField] private TextMesh shapeCountText;
        [SerializeField] private SpriteRenderer shapeIcon;
        [SerializeField] private MonoPacket packetPrefab;
        [SerializeField] private Shape shape;
        [SerializeField] private int packetAmount = 1;
        [SerializeField] private int tickRate = 1;
        [SerializeField] private int maxDownstream = 1;


     
        private Source source;
        private void Awake()
        {
            var monoTicker = FindObjectOfType<MonoTicker>();
            var ticker = monoTicker.GetComponent<MonoTicker>().ticker;
            
            shapeCountText.text = "";
            shapeIcon.sprite = packetPrefab.GetSprite(shape);
            
            source = new Source(shape, new Rate(packetAmount, tickRate), ticker, maxDownstream);
            source.Updated += (sender, args) =>
            {
                shapeCountText.text = $"{args.Buffer}";
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
