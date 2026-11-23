using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Física")]
    [Tooltip("Velocidad vertical (u/s) que recibe el pájaro en cada impulso. Se asigna directamente, así cada toque da el mismo salto.")]
    public float flapVelocity = 6.5f;
    [Tooltip("Multiplicador de la gravedad de Unity (-9.81). Más alto = caída más rápida y juego más exigente.")]
    public float gravityScale = 2.5f;
    [Tooltip("Velocidad máxima de caída (u/s), para que la caída sea progresiva pero controlable.")]
    public float maxFallSpeed = 9f;

    [Header("Inclinación")]
    public float maxUpAngle = 25f;
    public float maxDownAngle = -80f;
    [Tooltip("Rapidez con la que el pájaro gira hacia el ángulo objetivo.")]
    public float rotationSmoothing = 10f;

    [Header("Espera antes de empezar")]
    public float idleBobAmplitude = 0.15f;
    public float idleBobFrequency = 2f;

    Rigidbody2D rb;
    Animator anim;
    float startY;
    bool flying;
    bool dead;

    static readonly int DeadHash = Animator.StringToHash("Dead");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // La rotación la controlamos nosotros (inclinación) con MoveRotation;
        // anulamos cualquier giro que provoquen los choques.
        rb.freezeRotation = false;
        rb.angularVelocity = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0f; // flota hasta que empieza la partida
        rb.linearVelocity = Vector2.zero;
        rb.rotation = 0f;

        startY = rb.position.y;
    }

    // Se llama al empezar la partida (botón Start o primer toque).
    public void StartFlying()
    {
        flying = true;
        rb.gravityScale = gravityScale;
        Flap(); // primer impulso para que no caiga en seco
    }

    // Impulso inmediato y siempre igual: se fija la velocidad vertical
    // en lugar de sumar una fuerza que dependería de la velocidad actual.
    public void Flap()
    {
        if (dead) return;
        rb.linearVelocity = new Vector2(0f, flapVelocity);
    }

    public void Die()
    {
        if (dead) return;
        dead = true;
        if (anim != null) anim.SetTrigger(DeadHash);
        // Si iba subiendo, corta la subida para que caiga enseguida.
        if (rb.linearVelocity.y > 0f) rb.linearVelocity = new Vector2(0f, 0f);
        rb.gravityScale = gravityScale;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        if (!flying)
        {
            // Pequeño balanceo mientras espera a que empiece la partida.
            float y = startY + Mathf.Sin(Time.time * idleBobFrequency * Mathf.PI * 2f) * idleBobAmplitude;
            rb.MovePosition(new Vector2(rb.position.x, y));
            return;
        }

        Vector2 v = rb.linearVelocity;
        v.x = 0f; // el pájaro no se mueve en horizontal, se mueve el mundo
        if (v.y < -maxFallSpeed) v.y = -maxFallSpeed;
        rb.linearVelocity = v;

        // Ángulo objetivo según la velocidad vertical:
        // subiendo -> mira hacia arriba, cayendo -> se inclina hacia abajo.
        float target;
        if (dead)
        {
            target = maxDownAngle;
        }
        else
        {
            float t = Mathf.InverseLerp(-maxFallSpeed, flapVelocity, v.y);
            target = Mathf.Lerp(maxDownAngle, maxUpAngle, t);
        }

        // Suavizado independiente de los FPS, con ángulo siempre limitado.
        float k = 1f - Mathf.Exp(-rotationSmoothing * dt);
        float angle = Mathf.LerpAngle(rb.rotation, target, k);
        rb.angularVelocity = 0f;
        rb.MoveRotation(Mathf.Clamp(angle, maxDownAngle, maxUpAngle));
    }
}
