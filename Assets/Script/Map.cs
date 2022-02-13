using UnityEngine;

public partial class MapGenerator
{
    [System.Serializable]
    public class Map
    {
        public Transform tilePrefab;
        public Transform tilePrefab2;
        public Transform obtaclePrefab;
        public Coord mapSize;
        [Range(0,1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foreGroundColor;
        public Color backGroundColor;
        [Range(0, 1)]
        public float outLinePercent;
        public Coord MapCentre
        {
            get
            {
                return new Coord(mapSize.x/2, mapSize.y/2);
            }
        }

    }
}
