using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( WorldGenerator ) )]
public class WorldGeneratorEditor : Editor {

    public void OnEnable() {

    }

    public override void OnInspectorGUI() {
        WorldGenerator generator = ( WorldGenerator )target;

        DrawDefaultInspector();

        if ( GUILayout.Button( "生成" ) ) {
            generator.Generate();
        }
    }


}
