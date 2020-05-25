using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Scripts.Mgr
{
    [RequireComponent(typeof(Score))]
    public class Stage : MonoBehaviour
    {
        [SerializeField] private GameObject winPanel;
        [SerializeField] private Text scoreGoal;
        [SerializeField] private Button loadNextScene;

        private SupplyChain.Level level;

        public void Init(int winScore, string nextScene)
        {
            winPanel.SetActive(false);
            var score = GetComponent<Score>().score;
            scoreGoal.text = winScore.ToString();
            level = new SupplyChain.Level(score, winScore);
            level.Won += (sender, args) =>
            {
                winPanel.SetActive(true);
                Time.timeScale = 0;
            };
            loadNextScene.onClick.AddListener(delegate
            {
                if (!Application.CanStreamedLevelBeLoaded(nextScene))
                {
                    Application.Quit();
                    return;
                }
                
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
            }); 
        }

        private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Time.timeScale = 1;
        }
    }
}