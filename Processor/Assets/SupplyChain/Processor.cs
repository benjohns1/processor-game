using System;

namespace SupplyChain
{
    public interface IProcessor : INode
    {
    }
    
    [Serializable]
    public class Processor : IProcessor
    {
        public Shape inShape;
        public Shape outShape;
        private Ticker ticker;
        private Node node;

        public Processor(Shape inShape, Shape outShape, Ticker ticker)
        {
            this.inShape = inShape;
            this.outShape = outShape;
            this.ticker = ticker;
            node = new Node(1, 1);
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