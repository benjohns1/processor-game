using UnityEngine;
using SupplyChain;

namespace Unity.Scripts.Mgr
{
    public class MonoTicker : MonoBehaviour
    {
        [SerializeField] private uint updatesPerTick = 10;
        public Ticker ticker = new Ticker();

        private void Awake()
        {
            ticker.SetTickRate(updatesPerTick);
        }

        private void FixedUpdate() => ticker.FixedUpdate();
    }
}
