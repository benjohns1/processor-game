using System;
using System.Collections.Generic;
using SupplyChain.Graph;

namespace SupplyChain
{
    public interface ITransporter
    {
        IConnector GetConnector();
        event EventHandler<Transporter.PacketsMovedArgs> PacketsMoved;
    }
    
    public class Transporter : ITransporter
    {
        public class MovingPacket
        {
            public Packet Packet;
            public int Location;

            public override string ToString()
            {
                return $"{Packet} at {Location}";
            }
        }
        
        public class PacketsMovedArgs : EventArgs
        {
            public IEnumerable<MovingPacket> Packets;
        }
            
        public event EventHandler<PacketsMovedArgs> PacketsMoved;
        
        private readonly IConnector connector;
        private readonly int length;
        private readonly Rate rate;
        private readonly int speed;
        private readonly Filter filter;
        private readonly List<MovingPacket> packets = new List<MovingPacket>();
        private readonly List<MovingPacket> removePackets = new List<MovingPacket>();

        public Transporter(IConnector connector, Ticker ticker, int length, Rate rate, int speed)
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
            
            ticker.Tick += (sender, args) => Tick(args.Tick, output, input);
        }

        private void Tick(uint tick, IBuffer output, IBuffer input)
        {
            var initialCount = packets.Count;
            
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
            
            // Add new items to transporter and remove from connected input
            var amount = rate.GetAmount(tick);
            if (amount > 0)
            {
                var ps = input.Remove(filter, amount);
                foreach (var packet in ps)
                {
                    packets.Add(new MovingPacket
                    {
                        Packet = packet,
                        Location = 0,
                    });
                }
            }

            if (initialCount > 0 || packets.Count > 0)
            {
                OnPacketsMoved(new PacketsMovedArgs
                {
                    Packets = packets
                });
            }
        }

        public IConnector GetConnector() => connector;

        private void OnPacketsMoved(PacketsMovedArgs e)
        {
            PacketsMoved?.Invoke(this, e);
        }
    }
}