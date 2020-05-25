using System;
using System.Linq;
using SupplyChain;
using SupplyChain.Graph;
using SupplyChain.Processes;
using UnityEngine;
using Unity.Scripts.Mgr;
using Buffer = Unity.Scripts.UI.Buffer;
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
    
    public class Processor : MonoBehaviour, INode, IDeletable
    {
        [SerializeField] private ProcessConfig[] processes;

        [SerializeField] private ProcessType processType;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;
        [SerializeField] private int maxUpstream = 1;
        [SerializeField] private int maxDownstream = 1;
        [SerializeField] private Buffer bufferIn;
        [SerializeField] private Buffer bufferOut;
         
        private IProcessor processor;
        private SupplyChain.Ticker ticker;
        private NodeGraph graph;

        private void Awake()
        {
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
                bufferOut.Set(args.Packet.Amount, args.Packet.Shape);
            };
            processor.InputUpdated += (sender, args) =>
            {
                bufferIn.Set(args.Packet.Amount, args.Packet.Shape);
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
            graph = g;
            var cfg = GetProcessConfig(processType);
            icon.sprite = cfg.sprite;
            icon.transform.localScale *= cfg.spriteScale;
            
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
            
            return graph.AddNode(processor);
        }

        public bool Delete()
        {
            if (!graph.RemoveNode(processor))
            {
                return false;
            }
            Destroy(gameObject);
            return true;
        }

        public GameObject GameObject()
        {
            return gameObject;
        }
    }
}
