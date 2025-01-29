using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Color Data_0", menuName = "Scriptable Object / Color Data", order = 101)]
public class ColorData : ScriptableObjectWithID
{
    public Color color;
    
    [PreviewField] public Sprite colorIcon;
}
