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

        private Ticker ticker;

        private bool isActive;

        public Sink(Score score, Filter f, Rate r, Ticker t, int maxUpstream) : base(new Node(maxUpstream, 0), f, r)
        {
            ticker = t;
            ticker.Tick += Tick;
            score.RegisterSink(this);
        }

        private void Tick(object sender, TickEventArgs args)
        {
            var tick = args.Tick;
            var amount = Rate.GetAmount(tick);
             if (amount == 0)
             {
                 if (!isActive) return;
                 isActive = false;
                 OnDeactivated();
                 return;
             }

             var activated = false;
             foreach (var packet in Buffer.Remove(Filter, amount))
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
        
        public override string ToString() => $"{base.ToString()}:{Node}";

        protected virtual void OnPacketSunk(PacketSunkArgs e)
        {
            PacketSunk?.Invoke(this, e);
        }

        public override bool Delete()
        {
            if (!base.Delete())
            {
                return false;
            }
            
            ticker.Tick -= Tick;
            return true;
        }
    }
}