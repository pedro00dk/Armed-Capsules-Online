using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Map : MonoBehaviour {

    [Header("Map components")]
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform floorPrefab;

    public void LoadMapData(MapData mapData) {

        // Map components holder
        string mapChildName = "Map components";
        Transform oldMapChild = transform.Find(mapChildName);
        if (oldMapChild != null) {
            DestroyImmediate(oldMapChild.gameObject);
        }
        Transform mapChild = new GameObject(mapChildName).transform;
        mapChild.parent = transform;

        // Colors of tiles and floor
        tilePrefab.GetComponent<Renderer>().sharedMaterial.color = mapData.tileColor;
        floorPrefab.GetComponent<Renderer>().sharedMaterial.color = mapData.floorColor;

        // Spawning tiles and obstacles
        for (int x = 0; x < mapData.width; x++) {
            for (int y = 0; y < mapData.height; y++) {

                // Spawning tile
                Vector3 position = MapCoordinateToPosition(x, y, mapData.width, mapData.height, mapData.tileSize);
                Transform tile = Instantiate(tilePrefab, position, tilePrefab.transform.rotation) as Transform;
                tile.localScale = Vector3.one * mapData.tileSize * (1.0f - mapData.outlineSize);
                tile.parent = mapChild;

                // Spawning obstacle if has in this position
                if (mapData.mapObstacles[x, y]) {
                    Transform obstacle = Instantiate(obstaclePrefab, new Vector3(position.x, mapData.mapObstacleHeights[x, y] / 2.0f, position.z), obstaclePrefab.transform.rotation) as Transform;
                    obstacle.localScale = new Vector3(mapData.tileSize, mapData.mapObstacleHeights[x, y], mapData.tileSize);
                    obstacle.parent = mapChild;

                    // Coloring
                    Material obstacleMaterial = obstacle.GetComponent<Renderer>().material;
                    obstacleMaterial.color = mapData.mapObstacleColors[x, y];
                }
            }
        }

        // Instatiating floor in the origin
        Transform floor = Instantiate(floorPrefab, new Vector3(0, -0.01f, 0), floorPrefab.rotation) as Transform;
        floor.localScale = new Vector3(mapData.width * mapData.tileSize, mapData.height * mapData.tileSize, 1);
        floor.parent = mapChild;

        // Seting the map collider
        BoxCollider mapCollider = GetComponent<BoxCollider>();
        mapCollider.size = new Vector3(mapData.width * mapData.tileSize, 0.01f, mapData.height * mapData.tileSize);
    }

    Vector3 MapCoordinateToPosition(int x, int y, int mapWidth, int mapHeight, float tileSize) {
        return new Vector3(x - mapWidth / 2.0f + 0.5f, 0, y - mapHeight / 2.0f + 0.5f) * tileSize;
    }
}
