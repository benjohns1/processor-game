using System;

namespace SupplyChain
{
    public interface IScorer
    {
        event EventHandler<Score.UpdatedArgs> Updated;
    }
    
    public class Score : IScorer
    {
        public event EventHandler<UpdatedArgs> Updated;

        public class UpdatedArgs : EventArgs
        {
            public int Score;
        }
        
        private int score = 0;

        private static int SinkShapeScore(Shape shape)
        {
            switch (shape)
            {
                case Shape.Triangle:
                    return 3;
                case Shape.Square:
                    return 4;
                case Shape.Pentagon:
                    return 5;
                case Shape.Hexagon:
                    return 6;
                default:
                    throw new Exception($"shape {shape} not scored");
            }
        }

        public void RegisterSink(Sink sink)
        {
            sink.PacketSunk += (sender, args) =>
            {
                score += args.Packet.Amount * SinkShapeScore(args.Packet.Shape);
                OnUpdated(new UpdatedArgs
                {
                    Score = score,
                });
            };
        }

        private void OnUpdated(UpdatedArgs e)
        {
            Updated?.Invoke(this, e);
        }
    }
}