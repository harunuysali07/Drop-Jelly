using Sirenix.OdinInspector;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public LevelData levelData;
    [ShowInInspector, ReadOnly] public int RemainingMoves { get; private set; }
    [ShowInInspector, ReadOnly] public int RemainingMatchCount { get; private set; }

    [Header("Controllers")]
    public GridController gridController;

    public LevelController Initialize(LevelData currentLevelData)
    {
        levelData = currentLevelData;
        
        RemainingMoves = levelData.isInfinityMoves ? int.MaxValue : levelData.totalMoves;
        RemainingMatchCount = levelData.targetMatchCount;

        UIManager.Instance.gamePlay.SetTargetColorData(levelData.targetColorData);
        
        UIManager.Instance.gamePlay.UpdateMoveCount(RemainingMoves);
        UIManager.Instance.gamePlay.UpdateTargetCount(RemainingMatchCount);

        return this;
    }

    public void OnMove()
    {
        if (!levelData.isInfinityMoves)
        {
            RemainingMoves--;
            UIManager.Instance.gamePlay.UpdateMoveCount(RemainingMoves);   
        }

        if (RemainingMoves < 0)
        {
            GameManager.Instance.LevelFinish(false);
        }
    }

    public void OnMatch(ColorData matchedColor)
    {
        if (levelData.targetColorData == null)
        {
            RemainingMatchCount--;
            UIManager.Instance.gamePlay.UpdateTargetCount(RemainingMatchCount);

            if (RemainingMatchCount <= 0)
            {
                GameManager.Instance.LevelFinish(true);
            }
        }
        else if (matchedColor == levelData.targetColorData)
        {
            RemainingMatchCount--;
            UIManager.Instance.gamePlay.UpdateTargetCount(RemainingMatchCount);

            if (RemainingMatchCount <= 0)
            {
                GameManager.Instance.LevelFinish(true);
            }
        }
    }

    private void Start()
    {
        gridController.Initialize();
    }

    public ColorData[] GetNextColors()
    {
        return levelData.GetNextColorToSpawn(levelData.totalMoves - RemainingMoves);
    }
}