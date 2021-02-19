using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class ShakeTester : MonoBehaviour
{
    public float magnitude,roughness,fadeIn,fadeOut;

    [ContextMenu("Shake Camera")]
    public void ShakeCamera()
    {
        CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
    }
}
