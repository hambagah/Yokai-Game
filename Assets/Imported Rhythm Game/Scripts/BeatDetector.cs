using UnityEngine;
using System.Collections.Generic;

public class BeatDetector : MonoBehaviour
{
    public AudioSource musicSource; // 背景音乐
    public float sensitivity = 0.5f; // 灵敏度（可调）
    public float minBeatInterval = 0.3f; // 最小鼓点间隔，防止连续误判

    private List<float> detectedBeats = new List<float>();

    void Awake()
    {
        if (!musicSource || !musicSource.clip)
        {
            Debug.LogError("音频源未设置或音频剪辑为空！");
            return;
        }

        AnalyzeBeats();
    }

    private void AnalyzeBeats()
    {
        AudioClip clip = musicSource.clip;
        int sampleRate = clip.frequency; // 获取采样率
        int sampleLength = clip.samples; // 获取采样点数量
        float clipLength = clip.length;  // 获取音频长度

        float[] samples = new float[sampleLength];
        clip.GetData(samples, 0); // 获取音频数据

        float lastBeatTime = 0f;
        for (int i = 0; i < samples.Length; i += sampleRate / 10) // 每 0.1 秒检测一次
        {
            float sum = 0f;
            for (int j = 0; j < sampleRate / 10; j++) // 计算该时间段的能量
            {
                if (i + j < samples.Length)
                    sum += Mathf.Abs(samples[i + j]);
            }

            float time = (float)i / sampleRate; // 当前时间点
            if (sum > sensitivity && time - lastBeatTime > minBeatInterval)
            {
                detectedBeats.Add(time);
                lastBeatTime = time;
            }
        }

        Debug.Log($"🎵 预处理完成，共检测到 {detectedBeats.Count} 个鼓点！");
    }

    public List<float> GetDetectedBeats()
    {
        return new List<float>(detectedBeats);
    }
}
