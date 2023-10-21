using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace Platformer
{
    public class ScoreManager : MonoBehaviour
    {
        private static ScoreManager _instance;
        public static ScoreManager Instance { get { return _instance; } }

        public static int CollectableScore;

        public static float ElapsedTime;
        private bool timerRunning = false;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }

        }

        private void Start()
        {
            StartTimer();
            CollectableScore = 0;
        }

        public void AddToCollectableScore(int value)
        {
            CollectableScore += value;
            UI_Manager.Instance.RefreshCollectableUI(CollectableScore);
        }

        public void StartTimer()
        {
            timerRunning = true;
            ElapsedTime = 0;

            StartCoroutine(TimerRoutine());
        }

        public void EndTimer()
        {
            timerRunning = false;
            timerRunning = false;
        }

        private IEnumerator TimerRoutine()
        {
            while (timerRunning)
            {
                ElapsedTime += Time.deltaTime;

                yield return null;
            }
        }

        public int GetCollectableScore() => CollectableScore;
    }
}

