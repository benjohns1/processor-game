using System;

namespace SupplyChain
{
    [Serializable]
    public class Rate
    {
        private int packetRate;
        private int ticks;
        private uint lastTick;

        public Rate(int packetRate, int ticks)
        {
            this.packetRate = packetRate;
            this.ticks = ticks;
        }

        public int GetAmount(uint tick)
        {
            if (ticks == 0)
            {
                lastTick = tick;
                return 0;
            }

            var numTicks = (int) (tick - lastTick);
            lastTick = tick;
            var numProductions = numTicks / ticks;
            return numProductions * packetRate;
        }
    }
}