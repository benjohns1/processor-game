using SupplyChain;
using UnityEngine;

namespace Unity.Scripts
{
    public class MonoProcessor : MonoBehaviour, IMonoNode
    {
        [SerializeField] private GameObject icon;
     
        private MonoTicker monoTicker;   
        private IProcessor processor;
        private Ticker ticker;

        private void Awake()
        {
            monoTicker = FindObjectOfType<MonoTicker>();
            ticker = monoTicker.GetComponent<MonoTicker>().ticker;
            processor = new Processor(Shape.Triangle, Shape.Square, ticker);
        }

        public INode GetNode()
        {
            return processor;
        }
    }
}
