using System;
using System.Collections.Generic;
using SupplyChain.Graph;

namespace SupplyChain
{
    public interface ITransporter
    {
        bool Disconnect();
        event EventHandler<Transporter.PacketsMovedArgs> PacketsMoved;
        event EventHandler Disconnected;
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
        public event EventHandler Disconnected;

        private readonly IConnector connector;
        private readonly int length;
        private readonly Rate rate;
        private readonly int speed;
        private readonly Filter filter;
        private readonly List<MovingPacket> packets = new List<MovingPacket>();
        private readonly List<MovingPacket> removePackets = new List<MovingPacket>();
        private readonly IBuffer input;
        private readonly IBuffer output;
        private readonly Ticker ticker;

        public Transporter(IConnector connector, Ticker ticker, int length, Rate rate, int speed)
        {
            this.connector = connector;
            this.length = length;
            this.rate = rate;
            this.speed = speed;
            this.ticker = ticker;
            
            if (!(connector.Downstream is IProcessor p))
            {
                throw new Exception("transporter must connect a downstream IProcessor");
            }
            filter = p.Filter;
            
            if (!(connector.Upstream is IBuffer i) || !(connector.Downstream is IBuffer o))
            {
                throw new Exception("transporter input and output must both be IBuffer");
            }
            input = i;
            output = o;
            
            ticker.Tick += Tick;
            connector.Deleted += Deleted;
        }

        private void Tick(object sender, TickEventArgs e)
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
            var amount = rate.GetAmount(e.Tick);
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

        private void Deleted(object sender, EventArgs e)
        {
            if (PacketsMoved != null)
            {
                foreach (var del in PacketsMoved.GetInvocationList())
                {
                    PacketsMoved -= del as EventHandler<PacketsMovedArgs>;
                }
            }
            ticker.Tick -= Tick;
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public bool Disconnect() => connector.Delete();

        private void OnPacketsMoved(PacketsMovedArgs e)
        {
            PacketsMoved?.Invoke(this, e);
        }
    }
}