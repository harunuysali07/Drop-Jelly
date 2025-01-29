using TMPro;
using UnityEngine;

public class UIGamePlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moveAmountText;
    [SerializeField] private TextMeshProUGUI targetAmountText;
    
    public void UpdateMoveCount(int moveCount)
    {
        moveAmountText.text = moveCount.ToString();
    }
    
    public void UpdateTargetCount(int matchCount)
    {
        targetAmountText.text = matchCount.ToString();
    }
    
    //∞ 180
}
