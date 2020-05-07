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
        BLOCK_POINT,
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

//スクリーンに描画するときに使う関数
    public Vector2 GetScreenPositionFromTileTable(Vector2Int position)
    {
        return new Vector2(position.x * tileSize - centerPostion.x, -(position.y * tileSize - centerPostion.y));
    }
    
    public bool IsWall(Vector2Int position)
    {
        if(tileTable[position.x,position.y] == TILE_TYPE.WALL)
        {
            return true;
        }
        return false;
    }

      public bool IsBlock(Vector2Int position)
    {
        if(tileTable[position.x,position.y] == TILE_TYPE.BLOCK)
        {
            return true;
        }
        return false;
    }

//ぶつかったblockを取得する関数を作成
GameObject GetBlockObjAt(Vector2Int position)
{
    //Dictionaryの性質を利用
    //pair key is obj value is positionが入っている
    foreach(var pair in moveObjPositionOnTile)
    {
        if(pair.Value == position)
        {
            return pair.Key;
        }
    }
    return null;
}

//Blockを移動させる

public void UpdateBlockPosition(Vector2Int currentBlockPosition, Vector2Int nextBlockPosition)
{
    //blockの取得
    GameObject block = GetBlockObjAt(currentBlockPosition);
    //blockを移動する
    block.transform.position = GetScreenPositionFromTileTable(nextBlockPosition);
   //位置データの修正
    moveObjPositionOnTile[block] = nextBlockPosition;

    //tileTableの更新
    //次にブロックがおかれる場所をBlockとする
    tileTable[nextBlockPosition.x,nextBlockPosition.y] = TILE_TYPE.BLOCK;

    
}

public void UpdateTileTableForPlayer(Vector2Int nextPosition,Vector2Int currentPosition)
{
    
    //tiletableの更新
    //次にブロックがおかれる場所をPlayerとする
     tileTable[nextPosition.x,nextPosition.y] = TILE_TYPE.PLAYER;
    //現在の場所をGORUNDとする
     tileTable[currentPosition.x,currentPosition.y] = TILE_TYPE.GROUND;

}
}
