using System.Collections.Generic;
using UnityEngine;

public partial class MapGenerator : MonoBehaviour
{
    public Map[] map;
    public int mapIndex=0;
    
    public float tileSize;

    List<Coord> allTilesCoord;
    Queue<Coord> suffeledTilesCoord;

    Map currentMap;
    void Start()
    {
        GenerateMap();
    }
    public void GenerateMap()
    {
        currentMap = map[mapIndex];
        var randHeight = new System.Random(currentMap.seed);

        //currentMap.mapSize = new Vector2(mapWidth, mapHeight);
        // storing all coodinate to list
        allTilesCoord = new List<Coord>();
        for (int i = 0; i < currentMap.mapSize.x; i++)
        {
            for (int j = 0; j < currentMap.mapSize.y; j++)
            {
                allTilesCoord.Add(new Coord(i, j));
            }
        }

        // suffeled cordinate
        suffeledTilesCoord = new Queue<Coord>(Utility.SuffeledArray(allTilesCoord, currentMap.seed));
        
        //mapCentre = new Coord((int)currentMap.mapSize.x / 2, (int)currentMap.mapSize.y / 2);




        // filling map with tiles
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        // dynamically creating mapholder gameobject;
        Transform mapholder = new GameObject(holderName).transform;
        mapholder.parent = transform; // try to comment this line and see the result
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int z = 0; z < currentMap.mapSize.y; z++)
            {
                var tilePosition = CoordPosition(x, z);
                var newTile = Instantiate(RandomTile(), tilePosition, Quaternion.Euler(Vector3.right * 90));
                newTile.localScale = (1 - currentMap.outLinePercent) * tileSize * Vector3.one;
                newTile.parent = mapholder;

            }
        }



        // filling map with obstacle
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int obstacleCountNumber = 0;
        for (int i = 0; i < obstacleCount; i++)
        {
            var randomCoord = GetRandomCoord();
            
            // assuming this position for osbtacle
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            obstacleCountNumber++;
            // if this coindition passes then our assumption was right 
            if (Coord.NotEqual(randomCoord, currentMap.MapCentre) && MapIsFullyAccessible(obstacleMap, obstacleCountNumber))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)randHeight.NextDouble());
                var obstaclePosition = CoordPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(currentMap.obtaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity);
                newObstacle.parent = mapholder;
                newObstacle.localScale = new Vector3((1 - currentMap.outLinePercent) * tileSize,obstacleHeight, (1 - currentMap.outLinePercent) * tileSize);
                
                // setting color of the obstacle
                var obstacleRenderer = newObstacle.GetComponent<Renderer>();
                var obstacleMaterial = new Material(obstacleRenderer.material);
                var colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.backGroundColor,currentMap.foreGroundColor,colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;
            }
            // means assumption was wrong and we have to revert back 
            else
            {
                // this position can't be for the obstacle
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                if (obstacleCountNumber <= 0)
                {
                    obstacleCount = 0;
                }
                else
                {
                    obstacleCountNumber--;
                }
            }
        }
    }
    // flood fill algorithm
    // how this work 
    // we look for the number of tile present in the map
    // then we count the  assesiable block
    // then we check if the assesiable area count + number of obstacle present in map == number of tiles
    // if it matches the allow them to instantian the new block other wise not;
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.MapCentre);
        mapFlags[currentMap.MapCentre.x, currentMap.MapCentre.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        // after filling blocks 
        // total map block == accessiable block count + Current obstacle Count
        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }
    private Vector3 CoordPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2 + 0.5f + x, 0, -currentMap.mapSize.y / 2 + 0.5f + y)*tileSize;
    }
    public Coord GetRandomCoord()
    {
        var coord = suffeledTilesCoord.Dequeue();
        suffeledTilesCoord.Enqueue(coord);
        return coord;
    }
    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;
        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static bool IsEqual(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool NotEqual(Coord c1, Coord c2)
        {
            return c1.x != c2.x && c1.y != c2.y;
        }
    }
    public Transform RandomTile()
    {
        //if (Random.Range(1, 10) > 5)
        //{
        //    return tilePrefab;
        //}
        return currentMap.tilePrefab2;
    }
}
