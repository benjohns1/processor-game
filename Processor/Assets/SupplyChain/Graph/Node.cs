using System;
using System.Collections.Generic;

namespace SupplyChain.Graph
{
    public interface INode
    {
        Guid Id { get; }
        bool IsEntry { get; }
        bool IsFinal { get; }
        bool AddUpstream(IConnector connector);
        bool AddDownstream(IConnector connector);
        bool RemoveUpstream(IConnector connector);
        bool RemoveDownstream(IConnector connector);
    }
    
    public class Node
    {
        public Guid Id { get; }
        public bool IsEntry => upstream.Count == 0;
        public bool IsFinal => downstream.Count == 0;
        
        private readonly List<IConnector> upstream = new List<IConnector>();
        private readonly List<IConnector> downstream = new List<IConnector>();
        private readonly int maxUpstream;
        private readonly int maxDownstream;

        public Node(int maxUpstream, int maxDownstream)
        {
            Id = Guid.NewGuid();
            this.maxUpstream = maxUpstream;
            this.maxDownstream = maxDownstream;
        }

        public bool AddUpstream(IConnector connector)
        {
            if (upstream.Count >= maxUpstream)
            {
                return false;
            }
            upstream.Add(connector);
            return true;
        }

        public bool AddDownstream(IConnector connector)
        {
            if (downstream.Count >= maxDownstream)
            {
                return false;
            }
            downstream.Add(connector);
            return true;
        }

        public bool RemoveUpstream(IConnector connector)
        {
            return upstream.Remove(connector);
        }
        
        public bool RemoveDownstream(IConnector connector)
        {
            return downstream.Remove(connector);
        }

        public override string ToString()
        {
            return $"{Id} u({string.Join("|", upstream)}), d({string.Join("|", downstream)})";
        }
    }
}