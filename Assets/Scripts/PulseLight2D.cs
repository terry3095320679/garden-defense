using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class PulseLight2D : MonoBehaviour
{
    [SerializeField] private float minimumIntensity = 0.8f;
    [SerializeField] private float maximumIntensity = 1.6f;
    [SerializeField] private float pulseSpeed = 2f;

    private Light2D portalLight;

    private void Awake()
    {
        portalLight = GetComponent<Light2D>();
    }

    private void Update()
    {
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        portalLight.intensity = Mathf.Lerp(minimumIntensity, maximumIntensity, pulse);
    }
}
