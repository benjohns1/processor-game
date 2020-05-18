using UnityEngine;
using SupplyChain;
using UnityEngine.UI;

namespace Unity.Scripts.Mgr
{
    [RequireComponent(typeof(MonoScore))]
    public class MonoLevel : MonoBehaviour
    {
        [SerializeField] private int winScore = 1000;
        [SerializeField] private GameObject display;

        private Level level;

        private void Awake()
        {
            var score = GetComponent<MonoScore>().score;
            level = new Level(score, winScore);
            level.Won += (sender, args) =>
            {
                display.SetActive(true);
                Time.timeScale = 0;
            };
        }
    }
}