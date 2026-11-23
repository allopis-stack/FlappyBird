using UnityEngine;
using Random = UnityEngine.Random;

public class PipeSpawner : MonoBehaviour
{
    [Header("Jugabilidad")]
    [Tooltip("Segundos entre parejas de tuberías.")]
    public float timeToSpawn = 1.6f;
    [Tooltip("Velocidad de las tuberías (u/s).")]
    public float pipeSpeed = 2.5f;
    [Tooltip("Tamaño del hueco (u). El pájaro mide unas 0.6 u.")]
    public float gapSize = 2.8f;
    [Tooltip("Diferencia máxima de altura entre un hueco y el siguiente, para que siempre sea alcanzable.")]
    public float maxHeightChange = 2.5f;

    [Header("Referencias")]
    public GameObject pipePrefab;
    [Tooltip("Altura máxima del centro del hueco.")]
    public Transform spawnTop;
    [Tooltip("Altura mínima del centro del hueco.")]
    public Transform spawnBottom;

    float timer;
    float spawnX;
    float destroyX;
    float lastY;

    void Start()
    {
        // Las tuberías aparecen y desaparecen fuera de la cámara, sea cual sea
        // la proporción de pantalla (editor o móvil).
        Camera cam = Camera.main;
        float halfWidth = cam.orthographicSize * cam.aspect;
        spawnX = cam.transform.position.x + halfWidth + 1.5f;
        destroyX = cam.transform.position.x - halfWidth - 1.5f;

        lastY = (spawnTop.position.y + spawnBottom.position.y) * 0.5f;
        timer = timeToSpawn; // la primera pareja sale nada más empezar
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || !gm.IsPlaying) return;

        timer += Time.deltaTime;
        if (timer >= timeToSpawn)
        {
            timer -= timeToSpawn;
            CreateNewPipe();
        }
    }

    void CreateNewPipe()
    {
        float minY = Mathf.Max(spawnBottom.position.y, lastY - maxHeightChange);
        float maxY = Mathf.Min(spawnTop.position.y, lastY + maxHeightChange);
        float randomY = Random.Range(minY, maxY);
        lastY = randomY;

        Vector3 newPos = new Vector3(spawnX, randomY, 0f);
        GameObject o = Instantiate(pipePrefab, newPos, Quaternion.identity, transform);
        Pipe pipe = o.GetComponent<Pipe>();
        if (pipe != null) pipe.Setup(pipeSpeed, gapSize, destroyX);
    }
}
