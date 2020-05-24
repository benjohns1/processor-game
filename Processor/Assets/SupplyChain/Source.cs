using System;
using System.Collections.Generic;
using SupplyChain.Graph;

namespace SupplyChain
{
    [Serializable]
    public class Source : INode, IBuffer
    {
        public Shape shape;
        public Rate rate;
        private readonly Buffer buffer = new Buffer();
        private Node node;

        public Source(Shape shape, Rate rate, Ticker ticker, int maxDownstream)
        {
            this.shape = shape;
            this.rate = rate;
            ticker.Tick += (sender, args) => Tick(args.Tick);
            node = new Node(0, maxDownstream);
        }

        private void Tick(uint tick)
        {
             var add = rate.GetAmount(tick);
             if (add == 0)
             {
                 return;
             }

             buffer.Add(new Packet(shape, add));
        }
        
        public override string ToString() => $"{base.ToString()}:{node}";

        Guid INode.Id => node.Id;
        bool INode.IsEntry => node.IsEntry;
        bool INode.IsFinal => node.IsFinal;
        bool INode.AddUpstream(IConnector connector) => node.AddUpstream(connector);
        bool INode.AddDownstream(IConnector connector) => node.AddDownstream(connector);
        bool INode.RemoveUpstream(IConnector connector) => node.RemoveUpstream(connector);
        bool INode.RemoveDownstream(IConnector connector) => node.RemoveDownstream(connector);
        public bool Disconnect() => node.Disconnect();

        public event EventHandler<Buffer.UpdatedArgs> Updated
        {
            add => buffer.Updated += value;
            remove => buffer.Updated -= value;
        }
        int IBuffer.Add(Packet packet) => buffer.Add(packet);
        ICollection<Packet> IBuffer.Remove(Filter filter, int amount) => buffer.Remove(filter, amount);
    }
}