using System;
using System.Collections.Generic;
using System.Linq;
using SupplyChain.Graph;
using SupplyChain.Processes;
using Unity.Scripts;

namespace SupplyChain
{
    public interface IProcessor : INode, IBuffer
    {
        Filter Filter { get; }
        event EventHandler Activated;
        event EventHandler Deactivated;
        event EventHandler<Buffer.UpdatedArgs> InputUpdated;
    }
    
    [Serializable]
    public class Processor : IProcessor
    {
        private Node node;
        private readonly Buffer input = new Buffer();
        private readonly Buffer output = new Buffer();
        private readonly IProcess process;

        private bool isActive;
        public event EventHandler Activated;
        public event EventHandler Deactivated;

        public Processor(IProcess process, int rate, Ticker ticker, int maxUpstream, int maxDownstream)
        {
            this.process = process;
            node = new Node(maxUpstream, maxDownstream);
            ticker.Tick += (sender, args) => { ProcessPackets(rate); };
        }

        private void ProcessPackets(int rate)
        {
            var wasActive = isActive;
            var activated = false;
            foreach (var packet in process.Run(input.Remove(Filter, rate)))
            {
                activated = true;
                var added = output.Add(packet);
                if (added != packet.Amount) throw new Exception("error adding packet");
            }

            if (wasActive && !activated)
            {
                isActive = false;
                OnDeactivated();
            } else if (!wasActive && activated)
            {
                isActive = true;
                OnActivated();
            }
        }

        Guid INode.Id => node.Id;
        bool INode.IsEntry => node.IsEntry;
        bool INode.IsFinal => node.IsFinal;
        bool INode.AddUpstream(IConnector connector) => node.AddUpstream(connector);
        bool INode.AddDownstream(IConnector connector) => node.AddDownstream(connector);
        bool INode.RemoveUpstream(IConnector connector) => node.RemoveUpstream(connector);
        bool INode.RemoveDownstream(IConnector connector) => node.RemoveDownstream(connector);

        event EventHandler<Buffer.UpdatedArgs> IBuffer.Updated
        {
            add => output.Updated += value;
            remove => output.Updated -= value;
        }

        public event EventHandler<Buffer.UpdatedArgs> InputUpdated
        {
            add => input.Updated += value;
            remove => input.Updated -= value;
        }

        int IBuffer.Add(Packet packet) => input.Add(packet);
        public IEnumerable<Packet> Remove(Filter filter, int amount) => output.Remove(filter, amount);
        public Filter Filter => process.Filter;

        protected virtual void OnActivated()
        {
            Activated?.Invoke(this, EventArgs.Empty);
        }
        
        protected virtual void OnDeactivated()
        {
            Deactivated?.Invoke(this, EventArgs.Empty);
        }
    }
}