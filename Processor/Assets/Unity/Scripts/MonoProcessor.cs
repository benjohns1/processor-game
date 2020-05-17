using System;
using SupplyChain;
using SupplyChain.Graph;
using SupplyChain.Processes;
using UnityEngine;
using Unity.Scripts.Mgr;

namespace Unity.Scripts
{
    public enum Process
    {
        Complex
    }
    
    public class MonoProcessor : MonoBehaviour, IMonoNode
    {
        [SerializeField] private Process process;
        [SerializeField] private bool allShapes;
        [SerializeField] private Shape[] includeShapes;
        [SerializeField] private Shape[] excludeShapes;
        [SerializeField] private TextMesh inputText;
        [SerializeField] private TextMesh outputText;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;
        [SerializeField] private int maxUpstream = 1;
        [SerializeField] private int maxDownstream = 1;
        [SerializeField] private int rate = 1;
        
        private MonoTicker monoTicker;   
        private IProcessor processor;
        private Ticker ticker;

        private void Awake()
        {
            inputText.text = "";
            outputText.text = "";
            ticker = FindObjectOfType<MonoTicker>().ticker;
            var filter = new Filter(allShapes, includeShapes, excludeShapes);
            IProcess p;
            switch (process)
            {
                case Process.Complex:
                    p = new Complex(filter);
                    break;
                default:
                    throw new Exception("unhandled process type");
            }

            Init(p);
        }

        private void Init(IProcess p)
        {
            processor = new Processor(p, rate, ticker, maxUpstream, maxDownstream);
            processor.Updated += (sender, args) =>
            {
                outputText.text = $"{args.Buffer}";
            };
            processor.InputUpdated += (sender, args) =>
            {
                inputText.text = $"{args.Buffer}";
            };
            processor.Activated += (sender, args) =>
            {
                icon.color = activeColor;
            };
            processor.Deactivated += (sender, args) =>
            {
                icon.color = inactiveColor;
            };
        }

        public INode GetNode()
        {
            return processor;
        }

        public bool Init(NodeGraph g)
        {
            return g.AddNode(processor);
        }
    }
}
