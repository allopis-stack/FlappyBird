using UnityEngine;

// Pareja de tuberías. La velocidad y el tamaño del hueco los fija el PipeSpawner
// al crearla, así todos los parámetros de jugabilidad se ajustan en un solo sitio.
public class Pipe : MonoBehaviour
{
    [Tooltip("Velocidad horizontal en unidades por segundo.")]
    public float speed = 2.5f;
    [Tooltip("Altura del hueco entre la tubería de arriba y la de abajo (unidades).")]
    public float gapSize = 2.8f;

    [Header("Partes del prefab")]
    public Transform top;
    public Transform bottom;
    public BoxCollider2D scoreTrigger;

    Rigidbody2D rb;
    float destroyX = -20f;
    bool scored;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void Setup(float newSpeed, float newGap, float newDestroyX)
    {
        speed = newSpeed;
        gapSize = newGap;
        destroyX = newDestroyX;
        LayoutGap();
    }

    void Start()
    {
        LayoutGap();
    }

    // Coloca cada tubería justo en el borde del hueco y ajusta el trigger
    // de puntuación para que cubra solo el hueco. El centro del hueco es
    // la posición del objeto Pipe.
    void LayoutGap()
    {
        float half = gapSize * 0.5f;
        if (top != null) top.localPosition = new Vector3(0f, half + HalfHeight(top), 0f);
        if (bottom != null) bottom.localPosition = new Vector3(0f, -half - HalfHeight(bottom), 0f);
        if (scoreTrigger != null)
        {
            scoreTrigger.transform.localPosition = Vector3.zero;
            scoreTrigger.transform.localScale = Vector3.one;
            scoreTrigger.offset = Vector2.zero;
            scoreTrigger.size = new Vector2(0.3f, gapSize);
        }
    }

    static float HalfHeight(Transform t)
    {
        SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return 0f;
        return sr.sprite.bounds.size.y * Mathf.Abs(t.localScale.y) * 0.5f;
    }

    void FixedUpdate()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || !gm.IsPlaying) return; // al morir, todo se detiene

        // Movimiento en unidades/segundo: no depende de los FPS.
        rb.MovePosition(rb.position + Vector2.left * speed * Time.fixedDeltaTime);

        if (rb.position.x < destroyX) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Cada pareja suma exactamente un punto, y solo si la partida sigue.
        if (scored || !other.CompareTag("Player")) return;
        GameManager gm = GameManager.Instance;
        if (gm == null || !gm.IsPlaying) return;
        scored = true;
        gm.AddScore();
    }
}
