using System.Collections.Generic;
using System.Linq;

namespace SupplyChain.Processes
{
    public class AddOne : IProcess
    {
        public Filter Filter { get; }
        public Rate Rate { get; }
        
        public AddOne(Filter filter, Rate rate)
        {
            Filter = filter;
            Rate = rate;
        }
        
        public IEnumerable<Packet> Run(IEnumerable<Packet> packets)
        {
            return packets.Select(Process);
        }

        private static Packet Process(Packet packet)
        {
            switch (packet.Shape)
            {
                case Shape.Triangle:
                    return packet.NewShape(Shape.Square);
                case Shape.Square:
                    return packet.NewShape(Shape.Pentagon);
                case Shape.Pentagon:
                    return packet.NewShape(Shape.Hexagon);
                case Shape.Hexagon:
                    return packet;
                default:
                    return packet;
            }
        }
    }
}