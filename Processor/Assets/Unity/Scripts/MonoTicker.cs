using UnityEngine;
using SupplyChain;

namespace Unity.Scripts
{
    public class MonoTicker : MonoBehaviour
    {
        public Ticker ticker = new Ticker();

        private void FixedUpdate() => ticker.FixedUpdate();
    }
}
