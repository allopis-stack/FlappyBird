using UnityEngine;

// Fondo con efecto parallax: se repite en bucle sin cortes y se mueve más
// despacio que las tuberías para dar sensación de profundidad.
[RequireComponent(typeof(SpriteRenderer))]
public class Background : MonoBehaviour
{
    [Tooltip("Velocidad del fondo (u/s). Las tuberías van a 2.5 u/s.")]
    public float speed = 0.5f;

    Transform[] tiles;
    float tileWidth;
    float startX;
    float offset;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.enabled = true;

        // Un pelín menos que el ancho real para que no se vea ninguna línea entre copias.
        tileWidth = sr.bounds.size.x - 0.01f;
        startX = transform.position.x;

        // Tres copias (izquierda, centro, derecha) para cubrir cualquier
        // proporción de pantalla mientras se desplazan.
        tiles = new Transform[3];
        tiles[0] = CreateCopy(sr, "Background_L");
        tiles[1] = CreateCopy(sr, "Background_C");
        tiles[2] = CreateCopy(sr, "Background_R");
        sr.enabled = false; // el original solo sirve de plantilla
        UpdateTiles();
    }

    Transform CreateCopy(SpriteRenderer source, string copyName)
    {
        GameObject go = new GameObject(copyName);
        go.transform.SetParent(transform.parent, false);
        go.transform.localScale = transform.localScale;
        go.transform.position = transform.position;
        go.layer = gameObject.layer;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = source.sprite;
        sr.sharedMaterial = source.sharedMaterial;
        sr.sortingLayerID = source.sortingLayerID;
        sr.sortingOrder = source.sortingOrder;
        sr.color = source.color;
        return go.transform;
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || !gm.IsPlaying) return; // se detiene al acabar la partida

        // El desplazamiento se acumula y se envuelve con el ancho de la imagen:
        // no hay saltos ni acumulación de error.
        offset = Mathf.Repeat(offset + speed * Time.deltaTime, tileWidth);
        UpdateTiles();
    }

    void UpdateTiles()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            Vector3 p = tiles[i].position;
            p.x = startX - offset + (i - 1) * tileWidth;
            tiles[i].position = p;
        }
    }
}
