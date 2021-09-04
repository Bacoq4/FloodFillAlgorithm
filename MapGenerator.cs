using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    Vector3 center = new Vector3(0.5f,0,0);

    // TileInformation list from TileInformation class
    List<TileInformation> tileInformations = new List<TileInformation>();

    Dictionary<Vector3, TileInformation> PosToTileInfo = new Dictionary<Vector3, TileInformation>();

    public Transform Obstacle;

    // list of all tile positions
    List<Vector3> tilePositions = new List<Vector3>();

    [Range(0,1000)]
    public int seed;

    public int ObstacleCount = 10;
    [Range(0,1)]
    public float ObstaclePercent;

    //-------------------------------------------------
    public Transform tilePrefab;

    public Vector2Int MapSize;
    int mapFullSize;

    [Range(0,1)]
    public float outlinePercent;

    private void Start() {
        GenerateMap();  
    }

    public void GenerateMap()
    {
        tilePositions.Clear();
        tileInformations.Clear();
        PosToTileInfo.Clear();

        string ObstaclesHolderName = "ObstaclesHolder";

        if (transform.Find(ObstaclesHolderName))
        {
            DestroyImmediate(transform.Find(ObstaclesHolderName).gameObject);
        }

        Transform ObstaclesHolder = new GameObject(ObstaclesHolderName).transform;
        ObstaclesHolder.parent = transform;

        //---------------------------------------------------------------------------
        string TilesHolderName = "TilesHolder";

        if (transform.Find(TilesHolderName))
        {
            DestroyImmediate(transform.Find(TilesHolderName).gameObject);
        }

        Transform TilesHolder = new GameObject(TilesHolderName).transform;
        TilesHolder.parent = transform; 

        for (int y = 0; y < MapSize.y; y++)
        {
            for (int x = 0; x < MapSize.x; x++)
            {
                Vector3 tilePos = new Vector3(-MapSize.x/2+0.5f+x,0,-MapSize.y/2+y);

                Transform TempTile = Instantiate(tilePrefab , tilePos , Quaternion.Euler(90,0,0));

                TempTile.transform.parent = TilesHolder;

                Vector3 TileScales = Vector3.one * (1 - outlinePercent);
                TempTile.transform.localScale = TileScales;

                tilePositions.Add(TempTile.transform.position);
                TileInformation tempInfo = new TileInformation(TempTile.position, TempTile.gameObject, false, false);
                tileInformations.Add(tempInfo);
                PosToTileInfo.Add(TempTile.position,tempInfo);
            }
        }


        GenerateObstaclesRandomly(ObstaclesHolder);
    }

    public void GenerateObstaclesRandomly(Transform parent)
    {
        // percentage of the map is full of obstacles
        ObstacleCount = (int) (MapSize.x * MapSize.y * ObstaclePercent);
        // shuffle list
        tilePositions = Utility.ShuffleList<Vector3>(tilePositions, seed);

        // spawning obstacles
        for (int i = 0; i < ObstacleCount; i++)
        {
            for (int j = 0; j < tileInformations.Count; j++)
            {
                tileInformations[j].isColored = false;
            }
            Vector3 posToSpawn = tilePositions[i];

            if (posToSpawn == center)
            {
                continue;
            }
            PosToTileInfo[posToSpawn].isObstacled = true;

            FloodFillAlgorithm(PosToTileInfo[center]);

            if (!isMapFullyAccessible())
            {
                PosToTileInfo[posToSpawn].isObstacled = false;
                continue;
            }

            PosToTileInfo[posToSpawn].isColored = false;

            posToSpawn += Vector3.up * 0.5f;

            Transform TempObstacle = Instantiate(Obstacle, posToSpawn, Quaternion.identity);
            TempObstacle.parent = parent;
        }   
    }
        

    [System.Serializable]
    public class TileInformation
    {
        public Vector3 pos;
        public GameObject tileObj;
        public bool isObstacled;
        public bool isColored;

        public TileInformation(Vector3 _pos , GameObject _tileObj , bool _isObstacled ,bool _isColored)
        {
            pos = _pos;
            tileObj = _tileObj;
            isObstacled = _isObstacled;
            isColored = _isColored;
        }

        // When necessary
        // public static bool operator ==(TileInformation t1, TileInformation t2)
        // {
        //     return t1.pos == t2.pos && t1.tileObj == t2.tileObj && t1.isObstacled == t2.isObstacled && t1.isColored == t2.isColored;
        // }

        // public static bool operator !=(TileInformation t1, TileInformation t2)
        // {
        //     return !(t1 == t2);
        // }
    }


    public bool isMapFullyAccessible()
    {
        for (int i = 0; i < tileInformations.Count; i++)
        {
            if (!tileInformations[i].isObstacled)
            {
                if (!tileInformations[i].isColored)
                {
                    return false;
                }
            }
        }
        return true;
    }


    // The Main part of floodfillalgorithm
    public void FloodFillAlgorithm(TileInformation tileInfo)
    {
        if (!tileInfo.isColored && !tileInfo.isObstacled)
        {
            tileInfo.isColored = true;
            tileInfo.tileObj.GetComponent<Renderer>().sharedMaterial.color = Color.yellow;
        }
        if (PosToTileInfo.TryGetValue(tileInfo.pos + Vector3.forward , out TileInformation t1))
        {
            if (!PosToTileInfo[tileInfo.pos + Vector3.forward].isObstacled && !PosToTileInfo[tileInfo.pos + Vector3.forward].isColored)
            {
                FloodFillAlgorithm(PosToTileInfo[tileInfo.pos + Vector3.forward]);
            }
        }
        if (PosToTileInfo.TryGetValue(tileInfo.pos + Vector3.back, out TileInformation t2))
        {
            if (!PosToTileInfo[tileInfo.pos + Vector3.back].isObstacled && !PosToTileInfo[tileInfo.pos + Vector3.back].isColored)
            {
                FloodFillAlgorithm(PosToTileInfo[tileInfo.pos + Vector3.back]);
            }
        }
        if (PosToTileInfo.TryGetValue(tileInfo.pos + Vector3.right, out TileInformation t3))
        {
            if (!PosToTileInfo[tileInfo.pos + Vector3.right].isObstacled && !PosToTileInfo[tileInfo.pos + Vector3.right].isColored)
            {
                FloodFillAlgorithm(PosToTileInfo[tileInfo.pos + Vector3.right]);
            }
        }
        if (PosToTileInfo.TryGetValue(tileInfo.pos + Vector3.left, out TileInformation t4))
        {
            if (!PosToTileInfo[tileInfo.pos + Vector3.left].isObstacled && !PosToTileInfo[tileInfo.pos + Vector3.left].isColored)
            {
                FloodFillAlgorithm(PosToTileInfo[tileInfo.pos + Vector3.left]);
            }
        }
        
        return;
    }
}


