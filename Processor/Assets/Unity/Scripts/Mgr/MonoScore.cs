using SupplyChain;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Scripts.Mgr
{
    public class MonoScore : MonoBehaviour
    {
        [SerializeField] private Text text;

        public Score score = new Score();

        private void Awake()
        {
            text.text = "0";
            
            score.Updated += (sender, args) =>
            {
                text.text = $"{args.Score}";
            };
        }
    }
}
