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
        bool Delete();
        event EventHandler Deleted;
    }

    public class NodeComparer : IEqualityComparer<INode>
    {
        public bool Equals(INode x, INode y)
        {
            if (x == null)
            {
                return y == null;
            }

            return y != null && x.Id.Equals(y.Id);
        }

        public int GetHashCode(INode obj)
        {
            return obj.Id.GetHashCode();
        }
    }
    
    public class Node : INode
    {
        public Guid Id { get; }
        public bool IsEntry => upstream.Count == 0;
        public bool IsFinal => downstream.Count == 0;
        
        private readonly List<IConnector> upstream = new List<IConnector>();
        private readonly List<IConnector> downstream = new List<IConnector>();
        private readonly int maxUpstream;
        private readonly int maxDownstream;
        public event EventHandler Deleted;

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

        public bool Delete()
        {
            for (var i = upstream.Count - 1; i >= 0; i--)
            {
                if (!upstream[i].Delete())
                {
                    throw new Exception($"unable to clear upstream {upstream[i]} from {this}");
                }
            }
            if (upstream.Count > 0)
            {
                throw new Exception($"unable to clear upstream {upstream} from {this}");
            }
            
            for (var i = downstream.Count - 1; i >= 0; i--)
            {
                if (!downstream[i].Delete())
                {
                    throw new Exception($"unable to clear downstream {downstream[i]} from {this}");
                }
            }
            if (downstream.Count > 0)
            {
                throw new Exception($"unable to clear upstream {upstream} from {this}");
            }

            Deleted?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public override string ToString()
        {
            return $"{Id} u({string.Join("|", upstream)}), d({string.Join("|", downstream)})";
        }
    }
}