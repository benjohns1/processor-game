using System;

namespace Unity.Scripts
{
    [Serializable]
    public class Rate
    {
        public int amount = 1;
        public int tickSpan = 1;

        public SupplyChain.Rate Get()
        {
            return new SupplyChain.Rate(amount, tickSpan);
        }
    }
}