using System;
using System.Collections.Generic;
using SupplyChain.Graph;

namespace SupplyChain
{
    public interface IProcessor : INode, IBuffer
    {
        Filter Filter { get; }
    }
    
    [Serializable]
    public class Processor : IProcessor
    {
        public Filter inFilter;
        private Ticker ticker;
        private Node node;
        private readonly Buffer buffer = new Buffer();

        public Processor(Filter filter, Ticker ticker)
        {
            inFilter = filter;
            this.ticker = ticker;
            node = new Node(1, 1);
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
            add => buffer.Updated += value;
            remove => buffer.Updated -= value;
        }

        int IBuffer.Add(Packet packet) => buffer.Add(packet);
        public IEnumerable<Packet> Remove(Filter filter, int amount) => buffer.Remove(filter, amount);
        public Filter Filter => inFilter;
    }
}