using Cinemachine;
using UnityEngine;

namespace Platformer
{
    public class PlatformCameraSettings : MonoBehaviour
    {
        [SerializeField] CameraSettingsObject pcSettings;
        [SerializeField] CameraSettingsObject mobileSettings;
        [Space]
        [SerializeField] bool forceMobileOrthoSize;

        private void Awake()
        {
            if (SystemInfo.deviceType.Equals(DeviceType.Handheld) || forceMobileOrthoSize)
                GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = mobileSettings.OrthoWidth;
            else
                GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = pcSettings.OrthoWidth;

            if (GetComponent<CinemachineConfiner2D>() != null)
            GetComponent<CinemachineConfiner2D>().m_MaxWindowSize = pcSettings.OrthoWidth;
        }
    }
}
