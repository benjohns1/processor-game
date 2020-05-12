using System.Collections.Generic;
using System.Linq;

namespace SupplyChain.Processes
{
    public class Complex : IProcess
    {
        public Filter Filter { get; }
        
        public Complex(Filter filter)
        {
            this.Filter = filter;
        }
        
        public IEnumerable<Packet> Run(IEnumerable<Packet> packets)
        {
            return packets.Select(Process);
        }

        private Packet Process(Packet packet)
        {
            switch (packet.Shape)
            {
                case Shape.Triangle:
                    return packet.NewShape(Shape.Square);
                case Shape.Square:
                    return packet;
                default:
                    return packet;
            }
        }
    }
}