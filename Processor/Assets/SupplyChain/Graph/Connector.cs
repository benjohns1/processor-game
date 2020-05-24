using System;

namespace SupplyChain.Graph
{
    public interface IConnector
    {
        INode Upstream { get; }
        INode Downstream { get; }
        bool Delete();

        event EventHandler Deleted;
    }
    
    public class Connector : IConnector
    {
        public INode Upstream { get; }
        public INode Downstream { get; }
        public event EventHandler Deleted;

        public Connector(INode upstream, INode downstream)
        {
            Upstream = upstream ?? throw new Exception("upstream cannot be null");
            Downstream = downstream ?? throw new Exception("downstream cannot be null");
        }

        public override string ToString()
        {
            return $"(u:{Upstream.Id} d:{Downstream.Id}";
        }

        public bool Delete()
        {
            if (!Upstream.RemoveDownstream(this))
            {
                return false;
            }

            if (Downstream.RemoveUpstream(this))
            {
                Deleted?.Invoke(this, EventArgs.Empty);
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