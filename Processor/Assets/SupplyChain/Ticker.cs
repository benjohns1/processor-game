using System;

namespace SupplyChain
{
    public class TickEventArgs : EventArgs
    {
        public uint Tick = 0;
    }

    [Serializable]
    public class Ticker
    {
        public event EventHandler<TickEventArgs> Tick;
        
        private uint update = 0;
        private uint tick = 0;
        private uint updatesPerTick = 100;
        
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

        public uint GetTick()
        {
            return tick;
        }

        protected virtual void OnTick(TickEventArgs e)
        {
            Tick?.Invoke(this, e);
        }
    }
}