using UnityEngine;
using System.Security.Cryptography;

public class MapData {
    public readonly int width;
    public readonly int height;
    public readonly float tileSize;
    public readonly float outlineSize;

    public readonly bool[,] mapObstacles;
    public readonly float[,] mapObstacleHeights;

    public readonly Color tileColor;
    public readonly Color floorColor;
    public readonly Color[,] mapObstacleColors;

    public MapData(int width, int height, float tileSize, float outlineSize, bool[,] mapObstacles, float[,] mapObstacleHeights, Color tileColor, Color floorColor, Color[,] mapObstacleColors) {
        this.width = width;
        this.height = height;
        this.tileSize = tileSize;
        this.outlineSize = outlineSize;
        this.mapObstacles = mapObstacles;
        this.mapObstacleHeights = mapObstacleHeights;
        this.tileColor = tileColor;
        this.floorColor = floorColor;
        this.mapObstacleColors = mapObstacleColors;
    }

}
