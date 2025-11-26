using UnityEngine;

public class AuraVisualPulse : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float pulseStrength = 0.07f;

    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float t = Mathf.Sin(Time.time * pulseSpeed) * pulseStrength;
        transform.localScale = startScale + Vector3.one * t;
    }
}
