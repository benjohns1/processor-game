using System;

namespace SupplyChain
{
    public class Level
    {
        public event EventHandler<WonArgs> Won;
        
        public class WonArgs : EventArgs {}
        
        public Level(IScorer score, int winScore)
        {
            score.Updated += (sender, args) =>
            {
                if (args.Score >= winScore)
                {
                    OnWon(new WonArgs());
                }
            };
        }

        private void OnWon(WonArgs e)
        {
            Won?.Invoke(this, e);
        }
    }
}