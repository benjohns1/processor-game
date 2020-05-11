using System;
using System.Collections.Generic;
using SupplyChain.Graph;

namespace SupplyChain
{
    public interface ITransporter
    {
        IConnector GetConnector();
    }
    
    public class Transporter : ITransporter
    {
        private class MovingPacket
        {
            public Packet Packet;
            public int Location;

            public override string ToString()
            {
                return $"{Packet} at {Location}";
            }
        }
        
        private readonly IConnector connector;
        private readonly int length;
        private readonly int rate;
        private readonly int speed;
        private readonly Filter filter;
        private readonly List<MovingPacket> packets = new List<MovingPacket>();
        private readonly List<MovingPacket> removePackets = new List<MovingPacket>();

        public Transporter(IConnector connector, Ticker ticker, int length, int rate, int speed)
        {
            this.connector = connector;
            this.length = length;
            this.rate = rate;
            this.speed = speed;
            
            if (!(connector.Downstream is IProcessor p))
            {
                throw new Exception("transporter must connect a downstream IProcessor");
            }
            filter = p.Filter;
            
            if (!(connector.Upstream is IBuffer input) || !(connector.Downstream is IBuffer output))
            {
                throw new Exception("transporter input and output must both be IBuffer");
            }
            
            ticker.Tick += transportOnTick(output, input);
        }

        private EventHandler<TickEventArgs> transportOnTick(IBuffer output, IBuffer input)
        {
            return (sender, args) =>
            {
                // Move items along transporter
                foreach (var item in packets)
                {
                    item.Location += speed;
                    if (item.Location <= length) continue;
                    
                    // Output items at end
                    var removed = output.Add(item.Packet);
                    if (removed == item.Packet.Amount)
                    {
                        removePackets.Add(item);
                    }
                    else
                    {
                        item.Packet = item.Packet.NewAmount(item.Packet.Amount - removed);
                    }
                }

                // Remove output items
                foreach (var item in removePackets)
                {
                    packets.Remove(item);
                }
                removePackets.Clear();
                
                // Remove items from connected input
                var ps = input.Remove(filter, rate);
                foreach (var packet in ps)
                {
                    packets.Add(new MovingPacket
                    {
                        Packet = packet,
                        Location = 0,
                    });
                }
            };
        }

        public IConnector GetConnector() => connector;
    }
}