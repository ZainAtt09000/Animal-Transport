﻿//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(RCC_Records))]
public class RCC_RecordsEditor : Editor {

    RCC_Records prop;

    Color originalGUIColor;

    public override void OnInspectorGUI() {

        originalGUIColor = GUI.color;
        prop = (RCC_Records)target;
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("RCC Records Editor Window", EditorStyles.boldLabel);
        GUI.color = new Color(.75f, 1f, .75f);
        EditorGUILayout.LabelField("This editor will keep update necessary .asset files in your project for RCC. Don't change directory of the ''Resources/RCC Assets''.", EditorStyles.helpBox);
        GUI.color = originalGUIColor;
        EditorGUILayout.Space();

        GUI.color = new Color(.75f, 1f, .75f);
        EditorGUILayout.LabelField("All recorded clips are stored here. Replaying any recorded clip is so easy. Just use ''RCC.StartStopReplay(recordIndex or recordClip)'' in your script!", EditorStyles.helpBox);
        GUI.color = originalGUIColor;
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Recorded Clips", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        EditorGUI.indentLevel++;

        for (int i = 0; i < prop.records.Count; i++) {

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.LabelField(prop.records[i].recordName);

            GUI.color = Color.red;

            if (GUILayout.Button("X", GUILayout.Width(25f)))
                DeleteRecord(prop.records[i]);

            GUI.color = originalGUIColor;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

        }

        EditorGUI.indentLevel--;

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        GUI.color = Color.red;

        if (GUILayout.Button("Delete All Records"))
            DeleteAllRecords();

        GUI.color = originalGUIColor;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Created by Buğra Özdoğanlar\nBoneCrackerGames", EditorStyles.centeredGreyMiniLabel, GUILayout.MaxHeight(50f));

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

    }

    private void DeleteRecord(RCC_Recorder.RecordedClip record) {

        prop.records.Remove(record);

    }

    private void DeleteAllRecords() {

        prop.records.Clear();

    }

}
