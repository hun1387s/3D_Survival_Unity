using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.4f;

    float timeRate;
    public Vector3 noon; // Á¤¿À Vec 90 0 0

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;

    void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;

        UpdateLight(sun, sunColor, sunIntensity);
        UpdateLight(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    void UpdateLight(Light light, Gradient gradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);
        light.transform.eulerAngles = (time - (light == sun ? 0.25f : 0.75f)) * noon * 4f;
        light.color = gradient.Evaluate(time);
        light.intensity = intensity;

        GameObject go = light.gameObject;
        if (light.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (light.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }

}
