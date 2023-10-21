using System.Collections;
using TMPro;
using UnityEngine;

public class CollectableScore_UI : MonoBehaviour
{
    int uiScore;
    TextMeshProUGUI text;
    Animator anim;

    [SerializeField] float numberIncreaseSpeed = 0.2f;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        anim = GetComponent<Animator>();
    }

    public void RefreshCollectableScore(int currentScore)
    {
        StopAllCoroutines();
        StartCoroutine(IncreaseScoreTextToScore(currentScore));
    }

    private IEnumerator IncreaseScoreTextToScore(int currentScore)
    {
        anim.SetBool("IsOpen", true);

        while (currentScore >= uiScore)
        {
            text.text =("x" + uiScore++.ToString());

            yield return new WaitForSecondsRealtime(numberIncreaseSpeed);
        }

        text.text = ("x" + currentScore.ToString());

        yield return new WaitForSecondsRealtime(2);
        anim.SetBool("IsOpen", false);
    }
}
