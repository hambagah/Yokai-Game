using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light _directionalLight;
    [SerializeField] private LightingPreset _preset;
    [SerializeField, Range(0, 24)] private float _timeOfDay;
    [SerializeField, Range(0.01f, 1f)] private float timeMultiplier = 0.02f;

    private void Update()
    {
        if (_preset == null)
        {
            Debug.LogWarning("LightingPreset is not assigned.");
            return;
        }

        if (Application.isPlaying)
        {
            _timeOfDay += Time.deltaTime * timeMultiplier;
            _timeOfDay %= 24;
            UpdateLighting(_timeOfDay / 24f);
        }
        else
        {
            UpdateLighting(_timeOfDay / 24f);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = _preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = _preset.FogColor.Evaluate(timePercent);

        if (_directionalLight != null)
        {
            _directionalLight.color = _preset.DirectionalColor.Evaluate(timePercent);
            _directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 90f, 0));
        }
    }

    public void SetTimeOfDay(float newTime)
    {
        _timeOfDay = Mathf.Clamp(newTime, 0, 24);
        UpdateLighting(_timeOfDay / 24f);
    }

    private void OnValidate()
    {
        if (_directionalLight == null)
        {
            _directionalLight = RenderSettings.sun ?? FindObjectOfType<Light>();
        }
    }
}
