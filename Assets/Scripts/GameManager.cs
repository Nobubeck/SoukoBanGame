using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{ 
 [SerializeField] StageManager stage;
   [SerializeField] PlayerManager player;

    enum DIRECTION
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,

    }
    // Start is called before the first frame update
    void Start()
    {
        stage.LoadTileData();
        stage.CreateStage();
        //stageMangerで作られたプレイヤーの情報をGameManagerに格納
        player = stage.player;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            //player.Move(Vector2.up);
            MoveTo(DIRECTION.UP);

        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // player.Move(Vector2.down);
            MoveTo(DIRECTION.DOWN);


        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //player.Move(Vector2.left);
            MoveTo(DIRECTION.LEFT);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //player.Move(Vector2.right);
            MoveTo(DIRECTION.RIGHT);

        }

    }

    void MoveTo(DIRECTION direction)
    {
        Vector2Int currentPlayerPositionOnTile = stage.moveObjPositionOnTile[player.gameObject];
        //Debug.Log(currentPlayerPositionOnTile);
        Vector2Int nextPlayerPositionOnTile = GetNextPositionOnTile(currentPlayerPositionOnTile, direction);
       //Playerの移動先がWALL?
        if(stage.IsWall(nextPlayerPositionOnTile));
        {
            return;//処理をここで終了させる
        }

        //Playerの移動先がBLOCK?
         if(stage.IsBlock(nextPlayerPositionOnTile))
        {
            Vector2Int nextBlockPositionOnTile = GetNextPositionOnTile(nextPlayerPositionOnTile,direction);
            if(stage.IsWall(nextBlockPositionOnTile) || stage.IsBlock(nextBlockPositionOnTile))
            {
                return;
            }
            stage.UpdateBlockPosition(nextPlayerPositionOnTile,nextBlockPositionOnTile);
        }

        stage.UpdateTileTableForPlayer(currentPlayerPositionOnTile,nextPlayerPositionOnTile);
      

        //現実世界にプレイヤーの移動を反映
        player.Move(stage.GetScreenPositionFromTileTable(nextPlayerPositionOnTile));
        stage.moveObjPositionOnTile[player.gameObject] = nextPlayerPositionOnTile;
    }

    Vector2Int GetNextPositionOnTile(Vector2Int curretnPosition, DIRECTION direction)
    {
        switch (direction)
        {
            case DIRECTION.UP:
                return curretnPosition + Vector2Int.down;

            case DIRECTION.DOWN:
                return curretnPosition + Vector2Int.up;

            case DIRECTION.LEFT:
                return curretnPosition + Vector2Int.left;

            case DIRECTION.RIGHT:
                return curretnPosition + Vector2Int.right;
        }
        return curretnPosition;
    }
}

