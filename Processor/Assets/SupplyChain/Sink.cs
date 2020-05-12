using System;
using System.Collections.Generic;
using SupplyChain.Graph;

namespace SupplyChain
{
    [Serializable]
    public class Sink : IProcessor
    {
        public event EventHandler<PacketSunkArgs> PacketSunk;

        public class PacketSunkArgs
        {
            public Packet Packet;
        }

        private Rate rate;
        private readonly Buffer buffer = new Buffer();
        private Node node;

        private bool isActive;

        public Sink(Score score, Filter filter, Rate rate, Ticker ticker, int maxUpstream)
        {
            Filter = filter;
            this.rate = rate;
            ticker.Tick += (sender, args) =>
            {
                Tick(args.Tick);
            };
            node = new Node(maxUpstream, 0);
            score.RegisterSink(this);
        }

        private void Tick(uint tick)
        {
            var amount = rate.GetAmount(tick);
             if (amount == 0)
             {
                 if (!isActive) return;
                 isActive = false;
                 OnDeactivated();
                 return;
             }

             var activated = false;
             foreach (var packet in buffer.Remove(Filter, amount))
             {
                 activated = true;
                 OnPacketSunk(new PacketSunkArgs
                 {
                     Packet = packet,
                 });
             }

             if (!isActive && activated)
             {
                 isActive = true;
                 OnActivated();
             } else if (isActive && !activated)
             {
                 isActive = false;
                 OnDeactivated();
             }
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
        IEnumerable<Packet> IBuffer.Remove(Filter f, int amount) => buffer.Remove(f, amount);

        protected virtual void OnPacketSunk(PacketSunkArgs e)
        {
            PacketSunk?.Invoke(this, e);
        }

        public Filter Filter { get; }
        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event EventHandler<Buffer.UpdatedArgs> InputUpdated
        {
            add => buffer.Updated += value;
            remove => buffer.Updated += value;
        }

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