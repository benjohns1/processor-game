using System;
using SupplyChain.Graph;
using SupplyChain.Processes;

namespace SupplyChain
{
    [Serializable]
    public class Processor : GenericProcessor
    {
        private readonly Buffer input;
        private readonly Ticker ticker;
        private readonly IProcess process;

        private bool isActive;

        public Processor(IProcess p, Ticker t, int maxUpstream, int maxDownstream) : base(new Node(maxUpstream, maxDownstream), p.Filter, p.Rate)
        {
            input = new Buffer();
            process = p;
            ticker = t;
            t.Tick += Tick;
        }

        private void Tick(object sender, TickEventArgs args)
        {
            var tick = args.Tick;
            var amount = Rate.GetAmount(tick);
            if (amount == 0)
            {
                return;
            }
            
            var wasActive = isActive;
            var activated = false;
            foreach (var packet in process.Run(input.Remove(Filter, amount)))
            {
                activated = true;
                var added = Buffer.Add(packet);
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

        public override bool Disconnect()
        {
            if (!base.Disconnect())
            {
                return false;
            }
            
            ticker.Tick -= Tick;
            return true;
        }
        
        public override event EventHandler<Buffer.UpdatedArgs> InputUpdated
        {
            add => input.Updated += value;
            remove => input.Updated -= value;
        }
        
        public override int Add(Packet packet) => input.Add(packet);
    }
}