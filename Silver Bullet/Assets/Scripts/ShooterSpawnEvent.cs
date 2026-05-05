using UnityEngine;

public class ShooterSpawnEvent : MonoBehaviour
{
    [SerializeField] GameObject obstacleToSpawn;
    [SerializeField] Transform spawnPoint;

    public void SpawnShot()
    {
        Instantiate(obstacleToSpawn, spawnPoint.position, spawnPoint.rotation);
    }
}