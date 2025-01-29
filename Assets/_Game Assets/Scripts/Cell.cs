using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [ReadOnly] public Cube cube;
    [ReadOnly] public Vector2Int gridPosition;
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color32 highlightColor;

    private Color32 _normalColor;
    
    private void Awake()
    {
        _normalColor = spriteRenderer.color;
    }

    public void SetHighlight(bool state)
    {
        spriteRenderer.DOKill();
        spriteRenderer.DOColor(state ? highlightColor : _normalColor, 0.1f);
    }
}
