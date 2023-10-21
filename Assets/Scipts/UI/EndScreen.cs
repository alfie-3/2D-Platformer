using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class EndScreen : MonoBehaviour
    {
        private float score;

        private float elapsedTime;
        private TimeSpan timer;

        [SerializeField] int totalPossibleScore = 135;

        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI timerText;
        [SerializeField] TextMeshProUGUI rankText;
        [Space]
        [SerializeField] GameObject perfectScoreReward;
        [Space]
        [SerializeField] GameObject[] buttons;

        private void Start()
        {
            DisplayValues();
        }

        public void DisplayValues()
        {
            StartCoroutine(IncreaseValueRoutine());
        }

        private IEnumerator IncreaseValueRoutine()
        {
            while (score < ScoreManager.CollectableScore)
            {
                score++;
                scoreText.text = "SCORE: " + score;
                yield return new WaitForSecondsRealtime(0.01f);
            }

            score = ScoreManager.CollectableScore;
            scoreText.text = "SCORE: " + score;

            yield return new WaitForSecondsRealtime(1f);

            while (elapsedTime < ScoreManager.ElapsedTime)
            {
                elapsedTime++;
                timer = TimeSpan.FromSeconds(elapsedTime);
                timerText.text = "TIME: " + timer.ToString("mm': 'ss'. 'ff");

                yield return new WaitForSecondsRealtime(0.01f);
            }

            elapsedTime = ScoreManager.ElapsedTime;
            timer = TimeSpan.FromSeconds(elapsedTime);
            timerText.text = "TIME: " + timer.ToString("mm': 'ss'. 'ff");

            yield return new WaitForSecondsRealtime(1f);

            ScoreRank rank = CalculateRank();

            rankText.text = "RANK: " + rank.ToString();

            yield return new WaitForSecondsRealtime(1f);

            if (rank == ScoreRank.S)
                perfectScoreReward.SetActive(true);

            yield return new WaitForSecondsRealtime(0.5f);

            buttons[0].SetActive(true);

            yield return new WaitForSecondsRealtime(0.25f);

            buttons[1].SetActive(true);
        }

        private ScoreRank CalculateRank()
        {
            if (ScoreManager.CollectableScore == 0)
                return ScoreRank.F;

            float percent = ((float)ScoreManager.CollectableScore / (float)totalPossibleScore) * 100;

            if (percent >= 100)
                return ScoreRank.S;
            else if (percent > 70)
                return ScoreRank.A;
            else if (percent > 50)
                return ScoreRank.B;
            else return ScoreRank.F;
        }

        public enum ScoreRank
        {
            S,
            A,
            B,
            F
        }

        public void LoadScene(int buildIndex)
        {
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.TransitionScene(buildIndex);
                return;
            }
            else
            {
                SceneManager.LoadScene(buildIndex);
            }
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
