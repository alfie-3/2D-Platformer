using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Platformer
{
    public class CameraPanOver : MonoBehaviour
    {
        [SerializeField] GameObject cameraPrefab;
        [SerializeField] float hangTime;


        public void StartCameraPanOver()
        {
            StartCoroutine(CameraPanOverCoroutine());
            Debug.Log(transform.position);
        }


        private IEnumerator CameraPanOverCoroutine()
        {
            Vector3 cameraInstantiatePos = new(transform.position.x, transform.position.y, -10);
            GameObject cameraInstance = Instantiate(cameraPrefab, cameraInstantiatePos, Quaternion.identity);
            yield return new WaitForSeconds(hangTime);
            cameraInstance.GetComponent<CinemachineVirtualCamera>().enabled = false;
            yield return new WaitForSeconds(2);
            Destroy(cameraInstance);
            yield return null;
        }
    }
}
