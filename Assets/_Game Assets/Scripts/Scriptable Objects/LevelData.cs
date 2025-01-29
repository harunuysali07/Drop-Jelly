using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Level Data_0", menuName = "Scriptable Object / Level Data", order = 101)]
public class LevelData : ScriptableObjectWithID
{
    public bool isInfinityMoves = false;
    [HideIf(nameof(isInfinityMoves))] public int totalMoves;
    public int targetMatchCount;

    [Header("Color Settings")]
    public ColorData targetColorData;
    public List<ColorData> availableColors;

    public List<CubePosition> cubeColorsInGrid;
    public List<CubeColors> cubeColorsToSpawn;

    public ColorData[] GetNextColorToSpawn(int index)
    {
        if (cubeColorsToSpawn == null || index < 0 || cubeColorsToSpawn.Count <= index)
        {
            return GenerateRandomCubeColors();
        }

        var colors = new ColorData[4];

        colors[0] = cubeColorsToSpawn[index].topLeft;
        colors[1] = cubeColorsToSpawn[index].topRight;
        colors[2] = cubeColorsToSpawn[index].bottomLeft;
        colors[3] = cubeColorsToSpawn[index].bottomRight;

        return colors;
    }

    private ColorData[] GenerateRandomCubeColors()
    {
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
    
    [Serializable]
    public class CubePosition
    {
        public Vector2Int position;

        public CubeColors colors;
    }

    [Serializable]
    public class CubeColors
    {
        public ColorData topLeft;
        public ColorData topRight;
        public ColorData bottomLeft;
        public ColorData bottomRight;

        public ColorData[] ToArray()
        {
            var colors = new ColorData[4];

            colors[0] = topLeft;
            colors[1] = topRight;
            colors[2] = bottomLeft;
            colors[3] = bottomRight;

            return colors;
        }
    }
}