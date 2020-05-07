using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    //stageを読み込めるようになる
    [SerializeField] TextAsset stageFile;
    [SerializeField] GameObject[] prefabs;

    //列挙型
    enum TILE_TYPE
    {
        WALL,
        GROUND,
        BLOCK_pOINT,
        BLOCK,
        PLAYER,
    }

    TILE_TYPE[,] tileTable;
    float tileSize;

    Vector2 centerPostion;

    public PlayerManager player;

    public Dictionary<GameObject, Vector2Int> moveObjPositionOnTile = new Dictionary<GameObject, Vector2Int>();

    // Start is called before the first frame update
    void Start()
    {
        //LoadTileData();
        //CreateStage();
    }

    public void LoadTileData()
    {
        //空の物は入れない
        string[] lines = stageFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        int rows = lines.Length;
        int columns = lines[0].Split(new[] { ',' }).Length;
        tileTable = new TILE_TYPE[columns, rows];

        for (int y = 0; y < rows; y++)
        {
            string[] values = lines[y].Split(new[] { ',' });
            for (int x = 0; x < columns; x++)
            {
                //typeを変換
                tileTable[x, y] = (TILE_TYPE)int.Parse(values[x]);
                //Debug.Log(tileTable[x, y]);
            }
        }

    }
    //tileTableを使ってタイルを生成する
    public void CreateStage()
    {
        tileSize = prefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
        centerPostion.x = (tileTable.GetLength(0) / 2 * tileSize);
        centerPostion.y = (tileTable.GetLength(1) / 2 * tileSize);
        //2次元配列の長さの取得はgetLength()
        for (int y = 0; y < tileTable.GetLength(1); y++)
        {
            for (int x = 0; x < tileTable.GetLength(0); x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                GameObject ground = Instantiate(prefabs[(int)TILE_TYPE.GROUND]);
                ground.transform.position = GetScreenPositionFromTileTable(position);


                //tlieTypeを取得
                TILE_TYPE tileType = tileTable[x, y];
                GameObject obj = Instantiate(prefabs[(int)tileType]);
                obj.transform.position = GetScreenPositionFromTileTable(position);
                if (tileType == TILE_TYPE.PLAYER)
                {
                    player = obj.GetComponent<PlayerManager>();
                    moveObjPositionOnTile.Add(obj, position);
                }

                if (tileType == TILE_TYPE.BLOCK)
                {
                    moveObjPositionOnTile.Add(obj, position);
                }
            }
        }
    }


    public Vector2 GetScreenPositionFromTileTable(Vector2Int position)
    {
        return new Vector2(position.x * tileSize - centerPostion.x, -(position.y * tileSize - centerPostion.y));
    }
    // Update is called once per frame

}
