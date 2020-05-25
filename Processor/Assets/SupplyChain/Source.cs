using System;
using SupplyChain.Graph;

namespace SupplyChain
{
    [Serializable]
    public class Source : GenericProcessor
    {
        public Shape shape;

        public Source(Shape shape, Rate r, Ticker t, int maxDownstream) : base(new Node(0, maxDownstream), t, Filter.AllowAll(), r)
        {
            this.shape = shape;
        }

        protected override bool Process(int amount)
        {
            return Buffer.Add(new Packet(shape, amount)) != 0;
        }
        
        public override string ToString() => $"{base.ToString()}:{Node}";
    }
}