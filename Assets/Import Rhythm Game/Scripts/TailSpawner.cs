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

    public List<GameObject> tailList = new();
    public float preEndTime = 3f;
    void Start()
    {
        detectedBeats = BeatDetector.st.GetDetectedBeats();
        StartCoroutine(SpawnTailsWithMusic());
        StartCoroutine(MusicPlay());
    }
    IEnumerator MusicPlay()
    {
        yield return new WaitForSeconds(ScoreManager.Instance.delayTime);
        musicSource.Play();
        yield return new WaitForSeconds(musicSource.clip.length - preEndTime);
        foreach(GameObject item in tailList)
        {
            if(item!= null)
                Destroy(item);
        }
        StopAllCoroutines();
        ScoreManager.Instance.EndGame();
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

    [ContextMenu("Spawn Random Object")]
    void SpawnTail()
    {
        Vector3 spawnPos;
        Quaternion rotation;

        bool isLeft = Random.value < 0.5f;

        if (isLeft)
        {
            // 使用 Transform 位置
            spawnPos = GetRandomPosition(leftCenterTransform.position, leftSize);
            rotation = Quaternion.LookRotation(Vector3.left); // 朝左
        }
        else
        {
            spawnPos = GetRandomPosition(rightCenterTransform.position, rightSize);
            rotation = Quaternion.LookRotation(Vector3.right); // 朝右
        }

        tailList.Add(Instantiate(tailPrefab, spawnPos, rotation));
        /*
        // 计算随机位置，确保 Tail 在可触碰范围
        Vector3 spawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            Camera.main.nearClipPlane + distance // 确保 Tail 处于摄像机前方
        ));

        GameObject newTail = Instantiate(tailPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Spawned Tail at {spawnPosition}");*/
    }
    [Header("Prefab to Spawn")]
    public GameObject objectToSpawn;

    [Header("Left Spawn Area")]
    public Transform leftCenterTransform;
    public Vector3 leftSize = new Vector3(2, 2, 2);

    [Header("Right Spawn Area")]
    public Transform rightCenterTransform;
    public Vector3 rightSize = new Vector3(2, 2, 2);

    private Vector3 GetRandomPosition(Vector3 center, Vector3 size)
    {
        return center + new Vector3(
            Random.Range(-size.x / 2, size.x / 2),
            Random.Range(-size.y / 2, size.y / 2),
            center.z
        );
    }

    private void OnDrawGizmos()
    {
        if (leftCenterTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(leftCenterTransform.position, leftSize);
        }

        if (rightCenterTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(rightCenterTransform.position, rightSize);
        }
    }
}
