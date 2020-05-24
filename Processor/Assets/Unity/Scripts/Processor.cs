using System;
using System.Linq;
using SupplyChain;
using SupplyChain.Graph;
using SupplyChain.Processes;
using UnityEngine;
using Unity.Scripts.Mgr;
using Ticker = Unity.Scripts.Mgr.Ticker;

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
        public Filter inputFilter;
        public Rate rate;
    }
    
    public class Processor : MonoBehaviour, INode
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
         
        private IProcessor processor;
        private SupplyChain.Ticker ticker;

        private void Awake()
        {
            inputText.text = "";
            outputText.text = "";
            ticker = FindObjectOfType<Ticker>().ticker;
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
            processor = new SupplyChain.Processor(p, ticker, maxUpstream, maxDownstream);
            processor.Updated += (sender, args) =>
            {
                outputText.text = $"{args.Packet}";
            };
            processor.InputUpdated += (sender, args) =>
            {
                inputText.text = $"{args.Packet}";
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

        public SupplyChain.Graph.INode GetNode()
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
                    p = new AddOne(cfg.inputFilter.GetFilter(), cfg.rate.Get());
                    break;
                case ProcessType.SubOne:
                    p = new SubOne(cfg.inputFilter.GetFilter(), cfg.rate.Get());
                    break;
                default:
                    throw new Exception($"unhandled process type {processType}");
            }
            CreateProcessor(p);
            
            
            return g.AddNode(processor);
        }
    }
}
