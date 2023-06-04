using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUnderwater : MonoBehaviour
{
    private bool isUnder = false;

    [SerializeField]
    private GameObject waterFx;

    private GameObject cam;

    public float underwaterY = -33.55f;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Update()
    {
        if (cam.transform.position.y < underwaterY)
        {
            if (!isUnder)
            {
                isUnder = true;

                waterFx.gameObject.SetActive(true);
                RenderSettings.fog = true;
            }
        } else
        {
            if (isUnder)
            {
                isUnder = false;

                waterFx.gameObject.SetActive(false);
                RenderSettings.fog = false;
            }
        }
    }
}
