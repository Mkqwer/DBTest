using UnityEngine;

public class Cloud : MonoBehaviour
{
    public GameObject cloudPrefab;
    private float spawnInterval = 2f;
    private float minX = -2.0f;
    private float maxX = 2.0f;
    public float yOffset = 5f;
    public Transform player;

    private float nextSpawnY = 0f; 

    void Start()
    {
        nextSpawnY = player.position.y + yOffset;
    }

    void Update()
    {
        if (player.position.y + yOffset > nextSpawnY)
        {
            CreateCloud(); 
            nextSpawnY += spawnInterval; 
        }
    }

    void CreateCloud()
    {
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, nextSpawnY, 0f);
        Instantiate(cloudPrefab, spawnPosition, Quaternion.identity);
    }
}
