using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Map))]
public class MapGenerator : MonoBehaviour {

    [Header("Map size")]
    public int mapWidth;
    public int mapHeight;

    [Header("Obstacle generation")]
    [Range(0, 1)]
    public float obstaclePercent;
    public float minObstacleHeight;
    public float maxObstacleHeight;
    public int seed;

    [Header("Tile size and outline")]
    public float tileSize;
    [Range(0, 1)]
    public float outlineSize;

    [Header("Color configuration")]
    public Color backgroundColor;
    public Color foregroundColor;
    public ObstacleColorMode obstacleColorMode;
    public Color tileColor;
    public Color floorColor;

    // Internal properties
    Queue<Coord> randomTileCoords;

    public void GenerateAndLoadMapData() {
        Map map = GetComponent<Map>();
        map.LoadMapData(GenerateMapData());
    }

    public MapData GenerateMapData() {
        List<Coord> tileCoords = new List<Coord>();
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                tileCoords.Add(new Coord(x, y));
            }
        }

        System.Random tilePrng = new System.Random(seed);
        randomTileCoords = Shuffle(tileCoords, tilePrng);

        bool[,] mapObstacles = new bool[mapWidth, mapHeight];
        float[,] mapObstacleHeights = new float[mapWidth, mapHeight];
        Color[,] mapObstacleColors = new Color[mapWidth, mapHeight];

        int obstacleCount = (int) (mapWidth * mapHeight * obstaclePercent);
        int currentObstacleCount = 0;

        System.Random obstaclePrng = new System.Random(seed);
        System.Random colorPrng = new System.Random(seed);

        for (int i = 0; i < obstacleCount; i++) {
            Coord randomCoord = nextRandomCoord();
            mapObstacles[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            if (MapIsFullyAccessible(mapObstacles, currentObstacleCount, new Coord(mapWidth / 2, mapHeight / 2))) {
                mapObstacleHeights[randomCoord.x, randomCoord.y] = Mathf.Lerp(minObstacleHeight, maxObstacleHeight, (float) obstaclePrng.NextDouble());
                switch (obstacleColorMode) {
                    case ObstacleColorMode.ALEATORY:
                        mapObstacleColors[randomCoord.x, randomCoord.y] = Color.Lerp(backgroundColor, foregroundColor,
                            (float) colorPrng.NextDouble());
                        break;
                    case ObstacleColorMode.ONLY_BACKGROUND:
                        mapObstacleColors[randomCoord.x, randomCoord.y] = backgroundColor;
                        break;
                    case ObstacleColorMode.ONLY_FOREGROUND:
                        mapObstacleColors[randomCoord.x, randomCoord.y] = foregroundColor;
                        break;
                    case ObstacleColorMode.HORIZONTAL_GRADIENT:
                        mapObstacleColors[randomCoord.x, randomCoord.y] = Color.Lerp(backgroundColor, foregroundColor,
                            (float) randomCoord.y / mapHeight);
                        break;
                    case ObstacleColorMode.VERTICAL_GRADIENT:
                        mapObstacleColors[randomCoord.x, randomCoord.y] = Color.Lerp(backgroundColor, foregroundColor,
                            (float) randomCoord.x / mapWidth);
                        break;
                    case ObstacleColorMode.CIRCULAR_GRADIENT:
                        mapObstacleColors[randomCoord.x, randomCoord.y] = Color.Lerp(backgroundColor, foregroundColor,
                            Vector2.Distance(new Vector2(randomCoord.x, randomCoord.y), new Vector2(mapWidth / 2, mapHeight / 2)) / ((mapWidth + mapHeight) / 2)
                        );
                        break;
                }
            } else {
                mapObstacles[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
        return new MapData(mapObstacles.GetLength(0), mapObstacles.GetLength(1), tileSize, outlineSize, mapObstacles, mapObstacleHeights, tileColor, floorColor, mapObstacleColors);
    }

    Coord nextRandomCoord() {
        Coord coord = randomTileCoords.Dequeue();
        randomTileCoords.Enqueue(coord);
        return coord;
    }

    bool MapIsFullyAccessible(bool[,] mapObstacles, int currentObstacleCount, Coord freePosition) {
        bool[,] mapFlags = new bool[mapObstacles.GetLength(0), mapObstacles.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(freePosition);
        mapFlags[freePosition.x, freePosition.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0) {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0) {
                        if (neighbourX >= 0 && neighbourX < mapObstacles.GetLength(0) && neighbourY >= 0 && neighbourY < mapObstacles.GetLength(1)) {
                            if (!mapFlags[neighbourX, neighbourY] && !mapObstacles[neighbourX, neighbourY]) {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = mapObstacles.GetLength(0) * mapObstacles.GetLength(1) - currentObstacleCount;
        return targetAccessibleTileCount == accessibleTileCount;
    }

    Queue<T> Shuffle<T>(List<T> list, System.Random prng) {
        T[] array = list.ToArray();
        for (int i = 0; i < array.Length - 1; i++) {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        return new Queue<T>(array);
    }

    void OnValidate() {
        mapWidth = Mathf.Clamp(mapWidth, 5, 50);
        mapHeight = Mathf.Clamp(mapHeight, 5, 50);
        minObstacleHeight = Mathf.Clamp(minObstacleHeight, 0, 100);
        maxObstacleHeight = Mathf.Clamp(maxObstacleHeight, 0, 100);
        tileSize = Mathf.Clamp(tileSize, 1, 100);
    }

    public struct Coord {
        public int x;
        public int y;

        public Coord(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }

    public enum ObstacleColorMode {
        ALEATORY,
        ONLY_BACKGROUND,
        ONLY_FOREGROUND,
        HORIZONTAL_GRADIENT,
        VERTICAL_GRADIENT,
        CIRCULAR_GRADIENT
    }
}
