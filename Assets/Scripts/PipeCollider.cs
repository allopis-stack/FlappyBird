using UnityEngine;

// Va en cada tubería visible: si el pájaro la toca, se acaba la partida.
public class PipeCollider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}
