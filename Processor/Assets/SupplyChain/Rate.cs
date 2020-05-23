namespace SupplyChain
{
    public class Rate
    {
        private readonly int amount;
        private readonly int tickSpan;

        public Rate(int amount, int tickSpan)
        {
            this.amount = amount;
            this.tickSpan = tickSpan;
        }

        public int GetAmount(uint tick) => tickSpan == 0 || tick % tickSpan != 0 ? 0 : amount;
    }
}