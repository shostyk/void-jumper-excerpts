using UnityEngine;
using OlegShostyk;

public class HeavyBallManager : MonoBehaviour
{
    [SerializeField] GameObject heavyBallPrefab;
    [SerializeField] Transform player;
    [SerializeField] Color spawnOnPlatformColor;
    [SerializeField] Color spawnOnPlayerColor;
    private Vector3 offset;
    private float timer;
    private float spawnChanceOnPlayer;
    private const float SPAWN_CHANCE_ON_PLATFORM = 0.4f;
    private const float CHANCE_INCREASE_RATE = 0.8f;
    private const float MIN_COOLDOWN = 0.7f;
    private const float BONUS_TIME = 0.5f;
    private const float BASE_X = 12f;
    private const float BASE_Y = 20f;
    private const float BASE_Z = 3f;

    void Start()
    {
        Platform.OnSpawned += Platform_OnSpawned;

        timer = -BONUS_TIME;
    }

    void OnDestroy()
    {
        Platform.OnSpawned -= Platform_OnSpawned;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > MIN_COOLDOWN)
        {
            if (RandTool.IsChance(spawnChanceOnPlayer))
            {
                timer = 0f;
                SpawnOnPlayer();
            }
            else
            {
                spawnChanceOnPlayer += Time.deltaTime / CHANCE_INCREASE_RATE;
                spawnChanceOnPlayer = Mathf.Clamp(spawnChanceOnPlayer, 0f, 1f);
            }
        }


    }

    private void Platform_OnSpawned(Transform obj)
    {
        timer = -BONUS_TIME;
        spawnChanceOnPlayer = 0f;

        if (RandTool.IsChance(SPAWN_CHANCE_ON_PLATFORM))
            SpawnOnPlatform(obj);
    }

    private void SpawnOnPlatform(Transform obj)
    {
        offset.x = Random.Range(-BASE_X, BASE_X);
        offset.y = Random.Range(0.8f, 1.2f) * BASE_Y;
        offset.z = Random.Range(-BASE_Z, 0f);

        SpawnHeavyBall(obj, spawnOnPlatformColor);
    }

    private void SpawnOnPlayer()
    {
        offset.x = Random.Range(-BASE_X, BASE_X);
        offset.y = Random.Range(0.7f, 1.4f) * BASE_Y;
        offset.z = Random.Range(-BASE_Z, BASE_Z);

        SpawnHeavyBall(player, spawnOnPlayerColor);
    }

    private void SpawnHeavyBall(Transform target, Color color)
    {
        GameObject heavyBall = ObjectsPooler.Instance.Obtain(MyTag.HeavyBall);
        Rigidbody rb = heavyBall.GetComponent<Rigidbody>();
        heavyBall.transform.position = target.position + offset;
        heavyBall.transform.rotation = heavyBallPrefab.transform.rotation;
        // Also nullify kinetic energy.
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        heavyBall.GetComponent<MeshRenderer>().material.color = color;

        heavyBall.SetActive(true);
    }
}
