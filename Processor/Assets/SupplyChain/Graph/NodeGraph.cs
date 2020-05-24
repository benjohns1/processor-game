using System;
using System.Collections.Generic;
using System.Linq;

namespace SupplyChain.Graph
{
    public class NodeGraph
    {
        private readonly List<INode> nodes = new List<INode>();

        public bool AddNode(INode node)
        {
            if (node == null)
            {
                return false;
            }
            nodes.Add(node);
            
            return true;
        }

        public bool RemoveNode(INode node)
        {
            if (node == null || !nodes.Contains(node))
            {
                return false;
            }
            if (!node.Delete())
            {
                return false;
            }
            
            nodes.Remove(node);
            return true;
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
            var values = nodes.Select(node =>
            {
                if (node.IsEntry)
                {
                    return node.IsFinal ? $"[orphan] {node}" : $"[entry] {node}";
                }
                
                return node.IsFinal ? $"[final] {node}" : $"[inner] {node}";
            });
            return $"{nodes.Count} total nodes\n{string.Join("\n", values)}";
        }
    }
}