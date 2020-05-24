using System;

namespace SupplyChain.Graph
{
    public interface IConnector
    {
        INode Upstream { get; }
        INode Downstream { get; }
        bool Clear();
    }
    
    public class Connector : IConnector
    {
        public INode Upstream { get; }
        public INode Downstream { get; }

        public Connector(INode upstream, INode downstream)
        {
            Upstream = upstream ?? throw new Exception("upstream cannot be null");
            Downstream = downstream ?? throw new Exception("downstream cannot be null");
        }

        public override string ToString()
        {
            return $"(u:{Upstream.Id} d:{Downstream.Id}";
        }

        public bool Clear()
        {
            if (!Upstream.RemoveDownstream(this))
            {
                return false;
            }

            if (Downstream.RemoveUpstream(this))
            {
                return true;
            }
            
            // Couldn't remove upstream, compensate by re-adding previously removed downstream
            if (!Upstream.AddDownstream(this))
            {
                throw new Exception("unable to add downstream");
            }

            return false;
        }
    }
}