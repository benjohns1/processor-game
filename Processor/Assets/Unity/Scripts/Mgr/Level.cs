using UnityEngine;

namespace Unity.Scripts.Mgr
{
    [RequireComponent(typeof(Score))]
    public class Level : MonoBehaviour
    {
        [SerializeField] private int winScore = 1000;
        [SerializeField] private GameObject display;

        private SupplyChain.Level level;

        private void Awake()
        {
            var score = GetComponent<Score>().score;
            level = new SupplyChain.Level(score, winScore);
            level.Won += (sender, args) =>
            {
                display.SetActive(true);
                Time.timeScale = 0;
            };
        }
    }
}