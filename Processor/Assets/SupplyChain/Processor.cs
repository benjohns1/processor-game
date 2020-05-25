using System;
using SupplyChain.Graph;
using SupplyChain.Processes;

namespace SupplyChain
{
    [Serializable]
    public class Processor : GenericProcessor
    {
        private readonly Buffer input;
        private readonly IProcess process;

        public Processor(IProcess p, Ticker t, int maxUpstream, int maxDownstream) : base(new Node(maxUpstream, maxDownstream), t, p.Filter, p.Rate)
        {
            input = new Buffer();
            process = p;
        }

        protected override bool Process(int amount)
        {
            var activated = false;
            foreach (var packet in process.Run(input.Remove(Filter, amount)))
            {
                activated = true;
                var added = Buffer.Add(packet);
                if (added != packet.Amount) throw new Exception("error adding packet");
            }

            return activated;
        }
        
        public override event EventHandler<Buffer.UpdatedArgs> InputUpdated
        {
            add => input.Updated += value;
            remove => input.Updated -= value;
        }
        
        public override int Add(Packet packet) => input.Add(packet);
    }
}