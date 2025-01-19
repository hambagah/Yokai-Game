using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light _directionalLight;  // Reference to the Directional Light in the scene
    [SerializeField] private LightingPreset _preset;   // Reference to the Lighting Preset asset
    [SerializeField, Range(0, 24)] private float _timeOfDay = 12f;  // Time of day variable (0 - 24)
    [SerializeField, Range(0.01f, 1f)] private float timeMultiplier = 0.02f;  // Speed of time progression
    [SerializeField] private float manualTimeStep = 0.5f;  // Step size for manual time adjustment

    private void Update()
    {
        if (_preset == null)
        {
            UnityEngine.Debug.LogWarning("LightingPreset is not assigned.");
            return;
        }

        // Manual time adjustment using keyboard input (R to increase, T to decrease)
        if (Input.GetKey(KeyCode.R))
        {
            _timeOfDay += manualTimeStep * Time.deltaTime * 10;  // Increase time smoothly
            if (_timeOfDay > 24) _timeOfDay -= 24;  // Ensure time stays within 0-24 range
        }
        if (Input.GetKey(KeyCode.T))
        {
            _timeOfDay -= manualTimeStep * Time.deltaTime * 10;  // Decrease time smoothly
            if (_timeOfDay < 0) _timeOfDay += 24;  // Ensure time stays within 0-24 range
        }

        // Automatic time progression when in Play mode
        if (UnityEngine.Application.isPlaying)
        {
            _timeOfDay += Time.deltaTime * timeMultiplier;
            _timeOfDay %= 24;  // Keep time within 0-24 range
        }

        UpdateLighting(_timeOfDay / 24f);
    }

    // Updates lighting settings based on the time of day percentage (0.0 - 1.0)
    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = _preset.AmbientColor.Evaluate(timePercent);  // Set ambient light color
        RenderSettings.fogColor = _preset.FogColor.Evaluate(timePercent);  // Set fog color

        if (_directionalLight != null)
        {
            _directionalLight.color = _preset.DirectionalColor.Evaluate(timePercent);  // Set directional light color
            _directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 90f, 0));  // Rotate the directional light to simulate sun movement
        }
    }

    // Manually set the time of day and update lighting accordingly
    public void SetTimeOfDay(float newTime)
    {
        _timeOfDay = Mathf.Clamp(newTime, 0, 24);  // Ensure the input time is within range
        UpdateLighting(_timeOfDay / 24f);
    }

    // Automatically find a directional light in the scene if not assigned
    private void OnValidate()
    {
        if (_directionalLight == null)
        {
            _directionalLight = RenderSettings.sun ?? FindObjectOfType<Light>();
        }
    }
}
