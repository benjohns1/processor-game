using UnityEngine;
using UnityEngine.UI;

namespace Unity.Scripts.Mgr
{
    public class Score : MonoBehaviour
    {
        [SerializeField] private Text text;

        public SupplyChain.Score score = new SupplyChain.Score();

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
