using System;
using System.Collections.Generic;

namespace SupplyChain
{
    public interface IBuffer
    {
        event EventHandler<Buffer.UpdatedArgs> Updated;
        int Add(Packet packet);
        IEnumerable<Packet> Remove(Filter filter, int amount);
    }
    
    [Serializable]
    public class Buffer : IBuffer
    {
        public event EventHandler<UpdatedArgs> Updated;
        
        private Dictionary<Shape, Packet> buffer = new Dictionary<Shape,Packet>();

        public class UpdatedArgs : EventArgs
        {
            public readonly Packet Buffer;
            public readonly int ChangedAmount;

            public UpdatedArgs(Packet buffer, int changedAmount)
            {
                Buffer = buffer;
                ChangedAmount = changedAmount;
            }
        }

        public int Add(Packet packet)
        {
            var added = AddPacket(packet);
            if (added == 0)
            {
                return 0;
            }
            
            OnBufferUpdated(new UpdatedArgs(buffer[packet.Shape], added));
            return added;
        }

        public IEnumerable<Packet> Remove(Filter filter, int amount)
        {
            var ps = new List<Packet>();
            foreach (var shape in buffer.Keys)
            {
                if (!filter.MatchShape(shape)) continue;
                
                var pRemove = buffer[shape].NewAmount(-amount);
                var added = AddPacket(pRemove);
                if (added == 0) continue;
                ps.Add(pRemove.NewAmount(-added));
                amount += added;
                if (amount == 0) return ps;
            }

            return ps;
        }

        private int AddPacket(Packet packet)
        {
            // Add new packet
            if (!buffer.ContainsKey(packet.Shape))
            {
                if (packet.Amount <= 0)
                {
                    return 0;
                }

                buffer.Add(packet.Shape, packet);
                return packet.Amount;
            }

            // Add to existing packet
            var amount = packet.Amount;
            var current = buffer[packet.Shape].Amount + amount;
            if (current < 0)
            {
                amount -= current;
                current = 0;
            }

            buffer[packet.Shape] = new Packet(packet.Shape, current);
            return amount;
        }

        protected virtual void OnBufferUpdated(UpdatedArgs e)
        {
            Updated?.Invoke(this, e);
        }

        public override string ToString()
        {
            return $"buffer: {buffer}";
        }
    }
}