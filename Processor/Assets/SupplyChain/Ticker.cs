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
        
        private uint update;
        private uint tick;
        private uint updatesPerTick;

        public void SetTickRate(uint upt)
        {
            updatesPerTick = upt;
        }
        
        public void FixedUpdate()
        {
            update++;
            if (update % updatesPerTick != 0)
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