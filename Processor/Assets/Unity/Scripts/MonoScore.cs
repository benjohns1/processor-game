using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Scripts
{
    public class MonoScore : MonoBehaviour
    {
        [SerializeField] private Text text;

        public Score score = new Score();
        private Text t;

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
