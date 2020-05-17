using System;

namespace SupplyChain
{
    public class TickEventArgs : EventArgs
    {
        public uint Tick = 0;
    }

    public class Ticker
    {
        public event EventHandler<TickEventArgs> Tick;
        public uint UpdatesPerTick { get; private set; }
        
        private uint update;
        private uint tick;

        public void SetTickRate(uint upt)
        {
            UpdatesPerTick = upt;
        }
        
        public void FixedUpdate()
        {
            update++;
            if (update % UpdatesPerTick != 0)
            {
                return;
            }

            tick++;
            OnTick(new TickEventArgs
            {
                Tick = tick
            });
        }

        protected virtual void OnTick(TickEventArgs e)
        {
            Tick?.Invoke(this, e);
        }
    }
}