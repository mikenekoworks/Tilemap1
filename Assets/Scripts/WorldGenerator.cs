using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour {

    [SerializeField]
    private Tilemap targetTilemap;

    [SerializeField]
    private Vector2Int size;

    [SerializeField]
    private TileBase field;

    [SerializeField]
    private TileBase sea;

    [SerializeField]
    private TileBase mountain;

    [SerializeField]
    private TileBase forest;

    [SerializeField]
    private int mountainSeedCount;

    [SerializeField]
    private int forestSeedCount;

    [SerializeField]
    private int seaSeedCount;

    [SerializeField]
    private Player player;

    [SerializeField]
    private TileBase[] town;

    public class MountainSeed {
        public Vector2Int position;
        public int influence;
    }
    public class SeaSeed {
        public Vector2Int position;
        public int influence;
    }
    public class ForestSeed {
        public Vector2Int position;
        public int influence;
    }

    public void Elevation( int x, int y, int power ) {

        if ( ( x < 0 ) || ( x > size.x - 1 ) || ( y < 0 ) || ( y > size.y - 1 ) ) {
            return;
        }


        targetTilemap.SetTile( new Vector3Int( x, y, 0 ), mountain );

        --power;
        if ( power == 0 ) {
            return;
        }

        if ( Random.value < ( power * 0.2f ) ) {
            Elevation( x + 1, y, power );
        } else {
            Sprout( x + 1, y, power );
        }
        if ( Random.value < ( power * 0.2f ) ) {
            Elevation( x - 1, y, power );
        } else {
            Sprout( x - 1, y, power );
        }
        if ( Random.value < ( power * 0.2f ) ) {
            Elevation( x, y + 1, power );
        } else {
            Sprout( x, y + 1, power );
        }
        if ( Random.value < ( power * 0.2f ) ) {
            Elevation( x, y - 1, power );
        } else {
            Sprout( x, y - 1, power );
        }
    }

    public void Erosion( int x, int y, int power ) {

        if ( ( x < 0 ) || ( x > size.x - 1 ) || ( y < 0 ) || ( y > size.y - 1 ) ) {
            return;
        }

        var tile = targetTilemap.GetTile( new Vector3Int( x, y, 0 ) );
        if ( tile != mountain ) {
            targetTilemap.SetTile( new Vector3Int( x, y, 0 ), sea );
        }

        --power;
        if ( power == 0 ) {
            return;
        }

        if ( Random.value < ( power * 0.2f ) ) {
            Erosion( x + 1, y, power );
        }
        if ( Random.value < ( power * 0.2f ) ) {
            Erosion( x - 1, y, power );
        }
        if ( Random.value < ( power * 0.2f ) ) {
            Erosion( x, y + 1, power );
        }
        if ( Random.value < ( power * 0.2f ) ) {
            Erosion( x, y - 1, power );
        }
    }

    public void Sprout( int x, int y, int power ) {

        if ( ( x < 0 ) || ( x > size.x - 1 ) || ( y < 0 ) || ( y > size.y - 1 ) ) {
            return;
        }

        var tile = targetTilemap.GetTile( new Vector3Int( x, y, 0 ) );
        if ( tile != mountain ) {
            targetTilemap.SetTile( new Vector3Int( x, y, 0 ), forest );
        }

        --power;
        if ( power == 0 ) {
            return;
        }

        if ( Random.value < power ) {
            Sprout( x + 1, y, power );
        }
        if ( Random.value < power ) {
            Sprout( x - 1, y, power );
        }
        if ( Random.value < power ) {
            Sprout( x, y + 1, power );
        }
        if ( Random.value < power ) {
            Sprout( x, y - 1, power );
        }
    }


    public void Generate() {
        Debug.Log( "Generate!" );

        targetTilemap.ClearAllTiles();
        targetTilemap.SetTile( new Vector3Int( size.x - 1, size.y - 1, 0 ), field );

        targetTilemap.BoxFill( Vector3Int.zero, field, 0, 0, size.x - 1, size.y - 1 );

        List< MountainSeed > mountain_seeds = new List<MountainSeed>();
        List< SeaSeed > sea_seeds = new List<SeaSeed>();
        List< ForestSeed > forest_seeds = new List<ForestSeed>();

        // 山の種を作る。
        for ( int i = 0; i < mountainSeedCount; ++i ) {

            int x = Random.Range( 2, size.x );
            int y = Random.Range( 2, size.y );

            var new_seed = new MountainSeed();
            new_seed.position.x = x;
            new_seed.position.y = y;
            new_seed.influence = Random.Range( 2, 8 );

            mountain_seeds.Add( new_seed );
        }
        // 森の種を作る
        for ( int i = 0; i < seaSeedCount; ++i ) {
            int x = Random.Range( 2, size.x );
            int y = Random.Range( 2, size.y );

            var new_seed = new ForestSeed();
            new_seed.position.x = x;
            new_seed.position.y = y;
            new_seed.influence = Random.Range( 2, 4 );

            forest_seeds.Add( new_seed );
        }
        // 湖の種を作る
        for ( int i = 0; i < seaSeedCount; ++i ) {
            int x = Random.Range( 2, size.x );
            int y = Random.Range( 2, size.y );

            var new_seed = new SeaSeed();
            new_seed.position.x = x;
            new_seed.position.y = y;
            new_seed.influence = Random.Range( 2, 8 );

            sea_seeds.Add( new_seed );
        }

        // 山を成長させる。
        for ( int i = 0; i < mountain_seeds.Count; ++i ) {
            var seed = mountain_seeds[ i ];
            Elevation( seed.position.x, seed.position.y, seed.influence );
        }

        // 森を成長させる。
        for ( int i = 0; i < forest_seeds.Count; ++i ) {
            var seed = forest_seeds[ i ];
            Sprout( seed.position.x, seed.position.y, seed.influence );
        }

        // 川を成長させます
        for ( int i = 0; i < sea_seeds.Count; ++i ) {
            var seed = sea_seeds[ i ];
            Erosion( seed.position.x, seed.position.y, seed.influence );
        }

        // プレイヤーの初期位置を設定します（マップの真ん中）
        player.SetTilemap( targetTilemap );
        player.SetPlayerPosition( new Vector2Int( size.x / 2, size.y / 2 ) );

        for ( int x = 0; x < town.Length; ++x ) {
            targetTilemap.SetTile( new Vector3Int( size.x / 2 + x, size.y / 2, 0 ), town[ x ] );
            targetTilemap.SetTransformMatrix( new Vector3Int( size.x / 2 + x, size.y / 2, 0 ), Matrix4x4.identity );
        }
            
    }
}
