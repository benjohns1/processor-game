using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;

namespace Unity.Scripts
{
    public class MonoProcessor : MonoBehaviour, IMonoNode
    {
        [SerializeField] private bool allShapes;
        [SerializeField] private Shape[] shapes;
     
        private MonoTicker monoTicker;   
        private IProcessor processor;

        private void Awake()
        {
            var ticker = FindObjectOfType<MonoTicker>().ticker;
            var filter = new Filter(allShapes, shapes);
            processor = new Processor(filter, ticker);
            processor.Updated += (sender, args) =>
            {
                Debug.Log(args.Buffer);
                // TODO: update text
            };
        }

        public INode GetNode()
        {
            return processor;
        }
    }
}
