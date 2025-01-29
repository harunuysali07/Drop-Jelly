using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    //TODO: Get Level Settings from Scriptable Object or Create incremental levels
    [Header("Level Settings")]
    [SerializeField] private int totalMoves;

    [SerializeField] private int targetMatchCount;

    [ShowInInspector, ReadOnly] public int RemainingMoves { get; private set; }
    [ShowInInspector, ReadOnly] public int RemainingMatchCount { get; private set; }

    public List<ColorData> availableColors;
    public ColorData targetColorData;

    [Header("Controllers")]
    public GridController gridController;

    [Header("Debug Options")]
    [SerializeField] private ColorData[] debugColors;

    public LevelController Initialize()
    {
        RemainingMoves = totalMoves;
        RemainingMatchCount = targetMatchCount;

        UIManager.Instance.gamePlay.UpdateMoveCount(RemainingMoves);
        UIManager.Instance.gamePlay.UpdateTargetCount(RemainingMatchCount);

        return this;
    }

    public void OnMove()
    {
        RemainingMoves--;
        UIManager.Instance.gamePlay.UpdateMoveCount(RemainingMoves);

        if (RemainingMoves <= 0)
        {
            GameManager.Instance.LevelFinish(false);
        }
    }

    public void OnMatch(ColorData matchedColor)
    {
        if (targetColorData == null)
        {
            RemainingMatchCount--;
            UIManager.Instance.gamePlay.UpdateTargetCount(RemainingMatchCount);

            if (RemainingMatchCount <= 0)
            {
                GameManager.Instance.LevelFinish(true);
            }
        }
        else if (matchedColor == targetColorData)
        {
            RemainingMatchCount--;
            UIManager.Instance.gamePlay.UpdateTargetCount(RemainingMatchCount);

            if (RemainingMatchCount <= 0)
            {
                GameManager.Instance.LevelFinish(true);
            }
        }
    }

    public ColorData[] GetNextColors()
    {
        if (debugColors.Length == 4)
        {
            return debugColors;
        }

        var colors = new ColorData[4];

        //Predefined Types
        switch (Random.Range(0, 3))
        {
            case 0: //Four Different Colors
                var fourColors = new List<ColorData>();

                for (var i = 0; i < 4; i++)
                {
                    fourColors.Add(availableColors.Shuffle().Except(fourColors).ToList()[0]);
                }
                
                colors[0] = fourColors[0];
                colors[1] = fourColors[1];
                colors[2] = fourColors[2];
                colors[3] = fourColors[3];

                break;
            case 1: //Three Different Colors
                var treeColors = new List<ColorData>();

                for (var i = 0; i < 3; i++)
                {
                    treeColors.Add(availableColors.Shuffle().Except(treeColors).ToList()[0]);
                }

                switch (Random.Range(0, 4))
                {
                    case 0:
                        colors[0] = treeColors[0];
                        colors[1] = treeColors[0];
                        colors[2] = treeColors[1];
                        colors[3] = treeColors[2];
                        break;
                    case 1:
                        colors[0] = treeColors[0];
                        colors[1] = treeColors[1];
                        colors[2] = treeColors[2];
                        colors[3] = treeColors[1];
                        break;
                    case 2:
                        colors[0] = treeColors[0];
                        colors[1] = treeColors[1];
                        colors[2] = treeColors[2];
                        colors[3] = treeColors[2];
                        break;
                    case 3:
                        colors[0] = treeColors[0];
                        colors[1] = treeColors[1];
                        colors[2] = treeColors[0];
                        colors[3] = treeColors[2];
                        break;
                }

                break;
            case 2: //Two Different Colors
                var twoColors = new List<ColorData>();

                for (var i = 0; i < 2; i++)
                {
                    twoColors.Add(availableColors.Shuffle().Except(twoColors).ToList()[0]);
                }
                
                switch (Random.Range(0, 2))
                {
                    case 0:
                        colors[0] = twoColors[0];
                        colors[1] = twoColors[0];
                        colors[2] = twoColors[1];
                        colors[3] = twoColors[1];
                        break;
                    case 1:
                        colors[0] = twoColors[0];
                        colors[1] = twoColors[1];
                        colors[2] = twoColors[0];
                        colors[3] = twoColors[1];
                        break;
                }
                
                break;
            case 3: //One Color
                var oneColor = availableColors.Shuffle().ToList()[0];
                colors[0] = oneColor;
                colors[1] = oneColor;
                colors[2] = oneColor;
                colors[3] = oneColor;
                break;
        }

        return colors;
    }

    private void Start()
    {
        gridController.Initialize();
    }
}