using System.Collections.Generic;
using System.Linq;

namespace SupplyChain.Processes
{
    public class SubOne : IProcess
    {
        public Filter Filter { get; }
        
        public SubOne(Filter filter)
        {
            Filter = filter;
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
                    return packet;
                case Shape.Square:
                    return packet.NewShape(Shape.Triangle);
                case Shape.Pentagon:
                    return packet.NewShape(Shape.Square);
                case Shape.Hexagon:
                    return packet.NewShape(Shape.Pentagon);
                default:
                    return packet;
            }
        }
    }
}