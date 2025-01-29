using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moveAmountText;
    [SerializeField] private TextMeshProUGUI targetAmountText;

    [SerializeField] private Image targetColorImage;
    [SerializeField] private Sprite allColorsSprite;
    
    public void SetTargetColorData(ColorData targetColorData)
    {
        targetColorImage.sprite = targetColorData == null ? allColorsSprite : targetColorData.colorIcon;
    }
    
    public void UpdateMoveCount(int moveCount)
    {
        moveAmountText.text = moveCount == int.MaxValue ? "<size=180>\u221e</size>" : moveCount.ToString();
    }
    
    public void UpdateTargetCount(int matchCount)
    {
        targetAmountText.text = matchCount.ToString();
    }
    
    //∞ 180
}
