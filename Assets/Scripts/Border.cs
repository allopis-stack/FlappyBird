using UnityEngine;

// Suelo y techo: si el pájaro los toca, se acaba la partida.
public class Border : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}
