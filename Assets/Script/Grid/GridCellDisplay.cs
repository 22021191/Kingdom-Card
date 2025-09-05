using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(GridCell))]
public class GridCellDisplay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Color highlightColor = Color.cyan;
    public Color posColor = Color.green;
    public Color negColor = Color.red;
    private Color originalColor;
    public GameObject[] backgrounds;
    private bool setBackground = false;

    public GridCell gridCell;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gridCell = GetComponent<GridCell>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (!setBackground) SetBackground();
    }

    void OnMouseEnter()
    {
        if (!GameManager.Instance.PlayingCard)
        {
            spriteRenderer.color = highlightColor;
        }
        else if (gridCell.cellFull || gridCell.gridIndex.x > 1)
        {
            spriteRenderer.color = negColor;
        }
        else spriteRenderer.color = posColor;
    }

    void OnMouseExit()
    {
        spriteRenderer.color = originalColor;
    }

    private void SetBackground()
    {
        if (gridCell.gridIndex.x % 2 != 0)
        {
            backgrounds[0].SetActive(true);
        }
        if (gridCell.gridIndex.y % 2 != 0)
        {
            backgrounds[1].SetActive(true);
        }
        setBackground = true;
    }
}
