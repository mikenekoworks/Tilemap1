using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour {

    [SerializeField]
    private Animator playerAnimator;

    [SerializeField]
    private Camera primaryCamera;

    [SerializeField]
    private Camera secondaryCameraX;

    [SerializeField]
    private Camera secondaryCameraY;

    [SerializeField]
    private Camera secondaryCameraXY;

    [SerializeField]
    private Vector2Int mapPosition;

    [SerializeField]
    private Tilemap targetTilemap;

    float moveTime = 0.0f;

    int x_dir = 0;
    int y_dir = 0;

    float offsetX;
    float offsetY;

    public void SetPlayerPosition( Vector2Int pos ) {
        mapPosition.x = pos.x;
        mapPosition.y = pos.y;

        transform.localPosition = new Vector3( mapPosition.x + 0.5f, mapPosition.y + 0.5f, -10 );

    }

    public void SetTilemap( Tilemap target ) {
        targetTilemap = target;
    }

    public bool IsWalkable( int x, int y ) {

        var tile = targetTilemap.GetTile( new Vector3Int( x, y, 0 ) );

        if ( tile.name == "SeaTile" ) {
            return false;
        }

        return true;

    }

    public int LoopValue( int value, int adjust, int min, int max ) {

        value += adjust;

        if ( value >= max ) {
            value = min;
        }
        if ( value < min ) {
            value = max - 1;
        }

        return value;
    }

    // Use this for initialization
    void Start() {

        var tip_size = Screen.height / primaryCamera.orthographicSize;
        offsetX = targetTilemap.size.x; //Screen.width / tip_size * 4.0f;
        offsetY = targetTilemap.size.y; //Screen.height / tip_size * 4.0f;

        secondaryCameraX.transform.localPosition = new Vector3( offsetX, 0.0f, 0.0f );
        secondaryCameraY.transform.localPosition = new Vector3( 0.0f, offsetY, 0.0f );
        secondaryCameraXY.transform.localPosition = new Vector3( offsetX, offsetY, 0.0f );

    }

    // Update is called once per frame
    void Update() {

        if ( ( x_dir == 0 ) && ( y_dir == 0 ) ) {
            if ( Input.GetAxis( "Horizontal" ) > 0.0f ) {
                if ( IsWalkable( LoopValue( mapPosition.x, 1, 0, targetTilemap.size.x ), mapPosition.y ) == true ) {
                    x_dir = 1;
                    moveTime = 0.0f;
                }
                playerAnimator.SetFloat( "Direction", 2 );
            }
            if ( Input.GetAxis( "Horizontal" ) < 0.0f ) {
                if ( IsWalkable( LoopValue( mapPosition.x, -1, 0, targetTilemap.size.x ), mapPosition.y ) == true ) {
                    x_dir = -1;
                    moveTime = 0.0f;

                }
                playerAnimator.SetFloat( "Direction", 3 );
            }
        }
        if ( ( x_dir == 0 ) && ( y_dir == 0 ) ) {

            if ( Input.GetAxis( "Vertical" ) > 0.0f ) {
                if ( IsWalkable( mapPosition.x, LoopValue( mapPosition.y, 1, 0, targetTilemap.size.y ) ) == true ) {
                    y_dir = 1;
                    moveTime = 0.0f;

                }
                playerAnimator.SetFloat( "Direction", 1 );
            }
            if ( Input.GetAxis( "Vertical" ) < 0.0f ) {
                if ( IsWalkable( mapPosition.x, LoopValue( mapPosition.y, -1, 0, targetTilemap.size.y ) ) == true ) {
                    y_dir = -1;
                    moveTime = 0.0f;

                }
                playerAnimator.SetFloat( "Direction", 0 );
            }
        }

        if ( ( x_dir != 0 ) || ( y_dir != 0 ) ) {

            moveTime += Time.deltaTime;

            if ( moveTime > 0.4f ) {

                mapPosition.x = LoopValue( mapPosition.x, x_dir, 0, targetTilemap.size.x );
                mapPosition.y = LoopValue( mapPosition.y, y_dir, 0, targetTilemap.size.y );

                Vector2 v = new Vector3( offsetX, offsetY, 0.0f );
                if ( mapPosition.x < ( targetTilemap.size.x / 2.0f ) ) {
                    secondaryCameraX.transform.localPosition = new Vector3( offsetX, 0.0f, 0.0f );
                    v.x = offsetX;
                }
                if ( mapPosition.x > ( targetTilemap.size.x / 2.0f ) ) {
                    secondaryCameraX.transform.localPosition = new Vector3( -offsetX, 0.0f, 0.0f );
                    v.x = -offsetX;
                }

                if ( mapPosition.y < ( targetTilemap.size.y / 2.0f ) ) {
                    secondaryCameraY.transform.localPosition = new Vector3( 0.0f, offsetY, 0.0f );
                    v.y = offsetY;
                }
                if ( mapPosition.y > ( targetTilemap.size.y / 2.0f ) ) {
                    secondaryCameraY.transform.localPosition = new Vector3( 0.0f, -offsetY, 0.0f );
                    v.y = -offsetY;
                }
                secondaryCameraXY.transform.localPosition = v;

                x_dir = 0;
                y_dir = 0;
                moveTime = 0.0f;

            }
            transform.localPosition = new Vector3( mapPosition.x + Mathf.Clamp01( moveTime / 0.4f ) * x_dir + 0.5f, mapPosition.y + Mathf.Clamp01( moveTime / 0.4f ) * y_dir + 0.5f, -10 );

        }

    }
}
