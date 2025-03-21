using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TailSpawner : MonoBehaviour
{
    public GameObject tailPrefab; // Tail 预制体
    public AudioSource musicSource; // 音乐播放器
    public float minX = 0.2f, maxX = 0.8f; // Tail 生成的 X 轴范围
    public float minY = 0.3f, maxY = 0.8f; // Tail 生成的 Y 轴范围
    private List<float> detectedBeats; // 记录鼓点时间
    private int nextTailIndex = 0;

    void Start()
    {
        if (musicSource == null)
        {
            Debug.LogError("TailSpawner: Music Source is not assigned!");
            return;
        }

        // 获取 `BeatDetector` 生成的节奏点数据
        BeatDetector beatDetector = FindObjectOfType<BeatDetector>();
        if (beatDetector != null)
        {
            detectedBeats = beatDetector.GetDetectedBeats();
        }
        
        if (detectedBeats == null || detectedBeats.Count == 0)
        {
            Debug.LogError("TailSpawner: No detected beats found!");
            return;
        }

        StartCoroutine(SpawnTailsWithMusic());
    }

    IEnumerator SpawnTailsWithMusic()
    {
        Debug.Log("TailSpawner: Starting tail generation...");

        while (nextTailIndex < detectedBeats.Count)
        {
            float waitTime = detectedBeats[nextTailIndex] - musicSource.time;
            if (waitTime > 0)
            {
                yield return new WaitForSeconds(waitTime);
            }
            
            SpawnTail();
            nextTailIndex++;
        }
    }
    public float distance = 2;
    void SpawnTail()
    {
        // 计算随机位置，确保 Tail 在可触碰范围
        Vector3 spawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            Camera.main.nearClipPlane + distance // 确保 Tail 处于摄像机前方
        ));

        GameObject newTail = Instantiate(tailPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Spawned Tail at {spawnPosition}");
    }
}
