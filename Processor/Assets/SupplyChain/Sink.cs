using System;
using SupplyChain.Graph;

namespace SupplyChain
{
    [Serializable]
    public class Sink : GenericProcessor
    {
        public event EventHandler<PacketSunkArgs> PacketSunk;

        public class PacketSunkArgs
        {
            public Packet Packet;
        }

        public Sink(Score score, Filter f, Rate r, Ticker t, int maxUpstream) : base(new Node(maxUpstream, 0), t, f, r)
        {
            score.RegisterSink(this);
        }

        protected override bool Process(int amount)
        {
            var activated = false;
            foreach (var packet in Buffer.Remove(Filter, amount))
            {
                activated = true;
                OnPacketSunk(new PacketSunkArgs
                {
                    Packet = packet,
                });
            }

            return activated;
        }
        
        public override string ToString() => $"{base.ToString()}:{Node}";

        protected virtual void OnPacketSunk(PacketSunkArgs e)
        {
            PacketSunk?.Invoke(this, e);
        }
    }
}