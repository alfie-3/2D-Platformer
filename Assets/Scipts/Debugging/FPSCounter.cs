using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    int currentFps;
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

        InvokeRepeating(nameof(UpdateFPSCounter), 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        currentFps = (int)(1f / Time.unscaledDeltaTime);
    }

    private void UpdateFPSCounter() => text.text = currentFps.ToString();

}
