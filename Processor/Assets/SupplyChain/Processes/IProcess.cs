using System.Collections.Generic;

namespace SupplyChain.Processes
{
    public interface IProcess
    {
        IEnumerable<Packet> Run(IEnumerable<Packet> packets);
        Filter Filter { get; }
    }
}