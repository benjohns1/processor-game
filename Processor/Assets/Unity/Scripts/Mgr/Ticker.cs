using UnityEngine;

namespace Unity.Scripts.Mgr
{
    public class Ticker : MonoBehaviour
    {
        [SerializeField] private uint updatesPerTick = 30;
        public SupplyChain.Ticker ticker = new SupplyChain.Ticker();

        private void Awake()
        {
            ticker.SetTickRate(updatesPerTick);
        }

        private void FixedUpdate() => ticker.FixedUpdate();
    }
}
