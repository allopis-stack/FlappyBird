using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,      // Esperando al jugador (botón Start o toque en pantalla)
    Playing,    // Partida en curso
    GameOver    // El pájaro ha chocado
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    const string BestScoreKey = "BestScore";

    [Header("Referencias")]
    public Player player;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public GameObject readyPanel;
    public GameObject gameOverPanel;
    public GameObject newRecordPanel;
    public TextMeshProUGUI newRecordText;

    [Header("Sonidos")]
    public AudioClip flapClip;
    public AudioClip hitClip;
    public AudioClip startClip;
    public AudioClip newRecordClip;
    public AudioClip pointClip;
    [Range(0f, 1f)] public float flapVolume = 0.6f;
    [Range(0f, 1f)] public float hitVolume = 1f;
    [Range(0f, 1f)] public float startVolume = 0.5f;
    [Range(0f, 1f)] public float newRecordVolume = 0.6f;
    [Range(0f, 1f)] public float pointVolume = 0.5f;

    [Header("Fin de partida")]
    [Tooltip("Segundos entre el choque y la aparición del diálogo de nuevo récord.")]
    public float newRecordDelay = 0.7f;

    public GameState State { get; private set; } = GameState.Ready;
    public bool IsPlaying => State == GameState.Playing;
    public int Score { get; private set; }
    public int BestScore { get; private set; }

    AudioSource audioSource;
    bool newRecordShown;

    void Awake()
    {
        Instance = this;

        // Android limita por defecto a 30 FPS; pedimos 60 para que se vea fluido.
        // La física no depende de esto porque va en FixedUpdate.
        Application.targetFrameRate = 60;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // sonido 2D

        if (player == null) player = FindAnyObjectByType<Player>();

        // Si todavía no hay récord guardado, GetInt devuelve 0.
        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        Score = 0;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        State = GameState.Ready;
        newRecordShown = false;
        SetActive(readyPanel, true);
        SetActive(gameOverPanel, false);
        SetActive(newRecordPanel, false);
        UpdateScoreUI();
    }

    void Update()
    {
        if (!PressedThisFrame()) return;

        if (State == GameState.Ready)
        {
            StartGame();
        }
        else if (State == GameState.Playing)
        {
            player.Flap();
            PlaySound(flapClip, flapVolume);
        }
        // En GameOver no se hace nada: el pájaro ya no recibe impulsos.
    }

    // Espacio, clic izquierdo o toque en pantalla (Android).
    static bool PressedThisFrame()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) return true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) return true;
        return false;
    }

    // Llamado por el botón Start o por un toque en pantalla.
    public void StartGame()
    {
        if (State != GameState.Ready) return;

        State = GameState.Playing;
        Score = 0;
        UpdateScoreUI();
        SetActive(readyPanel, false);
        PlaySound(startClip, startVolume);
        player.StartFlying();
    }

    public void AddScore()
    {
        if (State != GameState.Playing) return;

        Score++;
        UpdateScoreUI();
        PlaySound(pointClip, pointVolume);
    }

    // Llamado al chocar contra una tubería o un borde.
    public void GameOver()
    {
        if (State != GameState.Playing) return; // evita dobles choques (tubería + suelo)

        State = GameState.GameOver;
        PlaySound(hitClip, hitVolume);
        player.Die();

        // El pájaro cae hasta el suelo atravesando las tuberías, como en el original.
        foreach (PipeCollider pc in FindObjectsByType<PipeCollider>(FindObjectsSortMode.None))
        {
            Collider2D c = pc.GetComponent<Collider2D>();
            if (c != null) c.enabled = false;
        }

        // Igualar el récord no cuenta: tiene que superarlo.
        bool isNewRecord = Score > BestScore;
        if (isNewRecord)
        {
            BestScore = Score;
            PlayerPrefs.SetInt(BestScoreKey, BestScore);
            PlayerPrefs.Save();
            UpdateScoreUI();
        }

        SetActive(gameOverPanel, true);

        if (isNewRecord && !newRecordShown)
        {
            newRecordShown = true;
            StartCoroutine(ShowNewRecordDialog());
        }
    }

    IEnumerator ShowNewRecordDialog()
    {
        yield return new WaitForSeconds(newRecordDelay);
        if (newRecordText != null) newRecordText.SetText("NEW RECORD!\n" + Score);
        SetActive(newRecordPanel, true);
        PlaySound(newRecordClip, newRecordVolume);
    }

    // Botón OK del diálogo de nuevo récord.
    public void CloseNewRecordDialog()
    {
        SetActive(newRecordPanel, false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.SetText("Score: " + Score);
        if (bestScoreText != null) bestScoreText.SetText("Best: " + BestScore);
    }

    void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null) audioSource.PlayOneShot(clip, volume);
    }

    static void SetActive(GameObject go, bool active)
    {
        if (go != null) go.SetActive(active);
    }
}
