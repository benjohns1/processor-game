using System;

namespace SupplyChain
{
    public class Score
    {
        public event EventHandler<UpdatedArgs> Updated;

        public class UpdatedArgs : EventArgs
        {
            public int Score;
            public int ChangeAmount;
        }
        
        private int score = 0;

        public void RegisterSink(Sink sink)
        {
            sink.PacketSunk += (sender, args) =>
            {
                var amount = args.Packet.Amount;
                switch (args.Packet.Shape)
                {
                    case Shape.Triangle:
                        amount *= 3;
                        break;
                    case Shape.Square:
                        amount *= 4;
                        break;
                    default:
                        amount *= 2;
                        break;
                }

                score += amount;

                OnUpdated(new UpdatedArgs
                {
                    Score = score,
                    ChangeAmount = amount,
                });
            };
        }

        protected virtual void OnUpdated(UpdatedArgs e)
        {
            Updated?.Invoke(this, e);
        }
    }
}