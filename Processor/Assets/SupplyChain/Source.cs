using System;

namespace SupplyChain
{
    [Serializable]
    public class Source : INode
    {
        public event EventHandler<BufferUpdatedArgs> BufferUpdated;

        public class BufferUpdatedArgs : EventArgs
        {
            public readonly int Buffer;
            public readonly int ChangedAmount;

            public BufferUpdatedArgs(int buffer, int changedAmount)
            {
                Buffer = buffer;
                ChangedAmount = changedAmount;
            }
        }

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
        private int buffer = 0;
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

             buffer += add;
             OnBufferUpdated(new BufferUpdatedArgs(buffer, add));
        }

        protected virtual void OnBufferUpdated(BufferUpdatedArgs e)
        {
            BufferUpdated?.Invoke(this, e);
        }

        public Guid Id => node.Id;
        public bool IsEntry => node.IsEntry;
        public bool AddUpstream(IConnector connector) => node.AddUpstream(connector);
        public bool AddDownstream(IConnector connector) => node.AddDownstream(connector);
        public bool RemoveUpstream(IConnector connector) => node.RemoveUpstream(connector);
        public bool RemoveDownstream(IConnector connector) => node.RemoveDownstream(connector);
        
        public override string ToString() => $"{base.ToString()}:{node}";
    }
}