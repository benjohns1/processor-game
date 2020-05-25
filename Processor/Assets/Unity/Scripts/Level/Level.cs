using UnityEngine;

namespace Unity.Scripts.Level
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private int winScore = 1000;
        [SerializeField] private Mgr.Stage stageManager;
        [SerializeField] private string nextScene;

        private void Start()
        {
            stageManager.Init(winScore, nextScene);
        }
    }
}