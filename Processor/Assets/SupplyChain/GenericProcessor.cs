using System;
using System.Collections.Generic;
using SupplyChain.Graph;
using UnityEngine;

namespace SupplyChain
{
    public interface IProcessor : INode, IBuffer
    {
        Filter Filter { get; }
        Rate Rate { get; }
        event EventHandler Activated;
        event EventHandler Deactivated;
        event EventHandler<Buffer.UpdatedArgs> InputUpdated;
    }

    public abstract class GenericProcessor : IProcessor
    {
        public virtual event EventHandler Activated;
        public virtual event EventHandler Deactivated;

        public virtual event EventHandler<Buffer.UpdatedArgs> InputUpdated;
        
        protected readonly Node Node;
        protected readonly Buffer Buffer;

        protected readonly Ticker Ticker;
        protected bool IsActive;
        
        public virtual Filter Filter { get; }
        public virtual Rate Rate { get; }
        
        Guid INode.Id => Node.Id;
        bool INode.IsEntry => Node.IsEntry;
        bool INode.IsFinal => Node.IsFinal;
        bool INode.AddUpstream(IConnector connector) => Node.AddUpstream(connector);
        bool INode.AddDownstream(IConnector connector) => Node.AddDownstream(connector);
        bool INode.RemoveUpstream(IConnector connector) => Node.RemoveUpstream(connector);
        bool INode.RemoveDownstream(IConnector connector) => Node.RemoveDownstream(connector);

        protected GenericProcessor(Node n, Ticker t, Filter f, Rate r, Buffer b = null)
        {
            Node = n;
            Filter = f;
            Rate = r;
            Buffer = b ?? new Buffer();
            Ticker = t;
            t.Tick += Tick;
        }

        protected virtual void Tick(object sender, TickEventArgs args)
        {
            var tick = args.Tick;
            var amount = Rate.GetAmount(tick);
            if (amount == 0)
            {
                if (!IsActive) return;
                IsActive = false;
                OnDeactivated();
                return;
            }
            
            var activated = Process(amount);
            if (!IsActive && activated)
            {
                IsActive = true;
                OnActivated();
            } else if (IsActive && !activated)
            {
                IsActive = false;
                OnDeactivated();
            }
        }

        protected abstract bool Process(int amount);

        public virtual event EventHandler<Buffer.UpdatedArgs> Updated
        {
            add => Buffer.Updated += value;
            remove => Buffer.Updated -= value;
        }

        public virtual event EventHandler Deleted
        {
            add => Node.Deleted += value;
            remove =>Node.Deleted -= value;
        }
        
        public virtual int Add(Packet packet) => Buffer.Add(packet);

        public ICollection<Packet> Remove(Filter filter, int amount) => Buffer.Remove(filter, amount);
        
        protected void OnActivated()
        {
            Activated?.Invoke(this, EventArgs.Empty);
        }
        
        protected void OnDeactivated()
        {
            Deactivated?.Invoke(this, EventArgs.Empty);
        }

        public virtual bool Delete()
        {
            if (Activated != null)
            {
                foreach (var del in Activated.GetInvocationList())
                {
                    Activated -= del as EventHandler;
                }
            }
            if (Deactivated != null)
            {
                foreach (var del in Deactivated.GetInvocationList())
                {
                    Deactivated -= del as EventHandler;
                }
            }
            if (InputUpdated != null)
            {
                foreach (var del in InputUpdated.GetInvocationList())
                {
                    InputUpdated -= del as EventHandler<Buffer.UpdatedArgs>;
                }
            }
            Ticker.Tick -= Tick;

            return Node.Delete();
        }
    }
}