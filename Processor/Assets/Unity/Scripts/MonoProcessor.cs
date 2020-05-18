using System;
using System.Linq;
using SupplyChain;
using SupplyChain.Graph;
using SupplyChain.Processes;
using UnityEngine;
using Unity.Scripts.Mgr;

namespace Unity.Scripts
{
    public enum ProcessType
    {
        AddOne,
        SubOne,
    }
    
    [Serializable]
    public class ProcessConfig
    {
        public ProcessType processType;
        public Sprite sprite;
        public Vector2 spriteScale = Vector2.one;
        public UnityFilter inputFilter;
    }
    
    public class MonoProcessor : MonoBehaviour, IMonoNode
    {
        [SerializeField] private ProcessConfig[] processes;

        [SerializeField] private ProcessType processType;
        [SerializeField] private TextMesh inputText;
        [SerializeField] private TextMesh outputText;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;
        [SerializeField] private int maxUpstream = 1;
        [SerializeField] private int maxDownstream = 1;
        [SerializeField] private int rate = 1;
         
        private IProcessor processor;
        private Ticker ticker;

        private void Awake()
        {
            inputText.text = "";
            outputText.text = "";
            ticker = FindObjectOfType<MonoTicker>().ticker;
        }

        public void SetProcess(ProcessType process)
        {
            processType = process;
        }
        
        private ProcessConfig GetProcessConfig(ProcessType p)
        {
            return processes.FirstOrDefault(ps => ps.processType == p);
        }

        private void CreateProcessor(IProcess p)
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
            var cfg = GetProcessConfig(processType);
            icon.sprite = cfg.sprite;
            icon.transform.localScale = cfg.spriteScale;
            
            IProcess p;
            switch (processType)
            {
                case ProcessType.AddOne:
                    p = new AddOne(cfg.inputFilter.GetFilter());
                    break;
                case ProcessType.SubOne:
                    p = new SubOne(cfg.inputFilter.GetFilter());
                    break;
                default:
                    throw new Exception($"unhandled process type {processType}");
            }
            CreateProcessor(p);
            
            
            return g.AddNode(processor);
        }
    }
}
