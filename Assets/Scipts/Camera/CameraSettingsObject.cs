using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    [CreateAssetMenu(fileName = "New Camera Settings", menuName = "Camera/Camera Settings", order = 1), System.Serializable]
    public class CameraSettingsObject : ScriptableObject
    {
        public float OrthoWidth;

    }
}
