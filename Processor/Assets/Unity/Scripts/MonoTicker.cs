using UnityEngine;

namespace Unity.Scripts
{
    public class MonoTicker : MonoBehaviour
    {
        [SerializeField] public SupplyChain.Ticker ticker = new SupplyChain.Ticker();

        private void FixedUpdate()
        {
            ticker.FixedUpdate();
        }
    }
}
