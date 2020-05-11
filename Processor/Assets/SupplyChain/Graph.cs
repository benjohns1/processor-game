using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SupplyChain
{
    public class Graph
    {
        private List<INode> nodes = new List<INode>();

        public void AddNode(INode node)
        {
            if (node == null)
            {
                throw new Exception("node cannot be null");
            }
            nodes.Add(node);
        }

        public bool AddConnector(IConnector connector)
        {
            // Connect connector to nodes and add nodes to graph if needed
            if (!connector.Upstream.AddDownstream(connector))
            {
                return false;
            }
            if (!nodes.Contains(connector.Upstream))
            {
                AddNode(connector.Upstream);
            }
            if (connector.Downstream.AddUpstream(connector))
            {
                if (!nodes.Contains(connector.Downstream))
                {
                    AddNode(connector.Downstream);
                }
                Debug.Log(this);
                return true;
            }

            // Couldn't add upstream, compensate by removing previously added downstream
            if (!connector.Upstream.RemoveDownstream(connector))
            {
                throw new Exception("unable to remove downstream");
            }
            
            return false;
        }

        public override string ToString()
        {
            var values = nodes.Select(node => node.IsEntry ? $"entry: {node}" : node.ToString());
            return $"{nodes.Count} total nodes\n{string.Join("\n", values)}";
        }
    }

    public interface INode
    {
        Guid Id { get; }
        bool IsEntry { get; }
        bool AddUpstream(IConnector connector);
        bool AddDownstream(IConnector connector);
        bool RemoveUpstream(IConnector connector);
        bool RemoveDownstream(IConnector connector);
    }

    public class Node
    {
        public Guid Id { get; }
        public bool IsEntry => upstream.Count == 0;
        
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

    public interface IConnector
    {
        INode Upstream { get; }
        INode Downstream { get; }
        int Length();
    }

    public class Connector : IConnector
    {
        public INode Upstream { get; }
        public INode Downstream { get; }
        private readonly int length;

        public Connector(INode upstream, INode downstream, int length)
        {
            Upstream = upstream ?? throw new Exception("upstream cannot be null");
            Downstream = downstream ?? throw new Exception("downstream cannot be null");;
            this.length = length;
        }

        public int Length()
        {
            return length;
        }

        public override string ToString()
        {
            return $"(u:{Upstream.Id} d:{Downstream.Id} length:{length}";
        }
    }
}