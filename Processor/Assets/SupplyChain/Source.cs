using System;
using System.Collections.Generic;
using SupplyChain.Graph;

namespace SupplyChain
{
    [Serializable]
    public class Source : INode, IBuffer
    {
        [Serializable]
        public class Rate
        {
            private int shape;
            private int ticks;
            private uint lastTick = 0;

            public Rate(int shape, int ticks)
            {
                this.shape = shape;
                this.ticks = ticks;
            }

            public int Produce(uint tick)
            {
                if (ticks == 0)
                {
                    lastTick = tick;
                    return 0;
                }

                var numTicks = (int) (tick - lastTick);
                lastTick = tick;
                var numProductions = numTicks / ticks;
                return numProductions * shape;
            }
        }
        
        public Shape shape;
        public Rate rate;
        private readonly Buffer buffer = new Buffer();
        private Ticker ticker;
        private Node node;

        public Source(Shape shape, Rate rate, Ticker ticker)
        {
            this.shape = shape;
            this.rate = rate;
            this.ticker = ticker;
            ticker.Tick += (sender, args) =>
            {
                Tick(args.Tick);
            };
            node = new Node(0, 1);
        }

        private void Tick(uint tick)
        {
             var add = rate.Produce(tick);
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

        public event EventHandler<Buffer.UpdatedArgs> Updated
        {
            add => buffer.Updated += value;
            remove => buffer.Updated -= value;
        }
        int IBuffer.Add(Packet packet) => buffer.Add(packet);
        IEnumerable<Packet> IBuffer.Remove(Filter filter, int amount) => buffer.Remove(filter, amount);
    }
}