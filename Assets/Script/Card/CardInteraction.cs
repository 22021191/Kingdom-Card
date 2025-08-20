using UnityEngine;

[RequireComponent(typeof(Card))]
public class CardInteraction : MonoBehaviour
{
    private Card controller;
    private Vector3 startPos;
    private bool isDragging = false;

    void Awake()
    {
        controller = GetComponent<Card>();
    }

    void OnMouseDown()
    {
        if (!controller.CanUse()) return;
        startPos = transform.position;
        isDragging = true;
        
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x, mousePos.y, 0);
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        if (controller.CanUse())
        {
            controller.Use();
        }
        else
        {
            transform.position = startPos; // trả về tay
            
        }
    }
}
