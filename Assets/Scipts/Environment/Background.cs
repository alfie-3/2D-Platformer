using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Tilemaps;

public class Background : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] BackgroundAsset[] backgroundAssets;

    private void Awake()
    {
        cam = Camera.main.gameObject;
    }

    private void Start()
    {
        foreach (var backgroundAsset in backgroundAssets)
        {
            backgroundAsset.StartPos = backgroundAsset.BackgroundObject.transform.position.x;

            if (backgroundAsset.BackgroundObject.GetComponent<SpriteRenderer>() != null)
                backgroundAsset.Length = backgroundAsset.BackgroundObject.GetComponent<SpriteRenderer>().bounds.size.x;

            else if (backgroundAsset.BackgroundObject.GetComponent<TilemapRenderer>() != null)
                backgroundAsset.Length = backgroundAsset.BackgroundObject.GetComponent<TilemapRenderer>().bounds.size.x;

            else
                return;

            //TileBackgroundAssets(backgroundAsset);
        }
    }

    private void LateUpdate()
    {
        foreach (var backgroundAsset in backgroundAssets)
        {
            float temp = (cam.transform.position.x * (1 - backgroundAsset.ParallaxAmount));

            DoParallaxEffect(backgroundAsset);

            MoveAssetToCamera(backgroundAsset, temp);
        }
    }

    private void MoveAssetToCamera(BackgroundAsset backgroundAsset, float temp)
    {
        if (temp > backgroundAsset.StartPos + backgroundAsset.Length)
        {
            backgroundAsset.StartPos += backgroundAsset.Length;
        }
        else if (temp < backgroundAsset.StartPos - backgroundAsset.Length)
        {
            backgroundAsset.StartPos -= backgroundAsset.Length;
        }
    }

    private void DoParallaxEffect(BackgroundAsset backgroundAsset)
    {
        float distance = (cam.transform.position.x * backgroundAsset.ParallaxAmount);

        GameObject backgroundGameObject = backgroundAsset.BackgroundObject;

        backgroundGameObject.transform.position = new Vector3(backgroundAsset.StartPos + distance, backgroundGameObject.transform.position.y, backgroundGameObject.transform.position.z);
    }
}

[System.Serializable]
public class BackgroundAsset
{
    public float StartPos;
    public GameObject BackgroundObject;
    public float Length;
    public float ParallaxAmount;
}