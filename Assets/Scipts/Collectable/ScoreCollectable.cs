using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class ScoreCollectable : CollectableItem
    {
        [Space(10)]
        [SerializeField] private int collectableValue;

        [SerializeField] AudioClip collectionAudio;

        public override void Collect()
        {
            Debug.Log(collectableValue + " added to the score");
            ScoreManager.Instance.AddToCollectableScore(collectableValue);
            AudioTools.CreateTempAudioSource(transform.position, collectionAudio, 0.5f, Random.Range(0.8f, 1.2f));
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, circleCastRadius);
        }
    }
}
