using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using BasicPCG;
using UnityEngine;

namespace BasicPCG
{
    [CustomEditor(typeof(PCGRoom))]
    public class PCGRoomEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            PCGRoom pcg = (PCGRoom)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                pcg.PrepareToGenerate();
            }
        }
    }
}