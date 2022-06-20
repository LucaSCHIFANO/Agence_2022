using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(TruckFuel))]
public class TruckFuelEditor : Editor
{

    TruckFuel t;

    bool showList = false;
    bool showConstList = false;
    SerializedObject GetTarget;
    SerializedProperty currList;
    SerializedProperty constList;
    int CurrListSize;
    int constListSize;
    SerializedProperty maxFuel;
    SerializedProperty currFuel;

    void OnEnable()
    {
        t = (TruckFuel)target;
        GetTarget = new SerializedObject(t);
        currList = GetTarget.FindProperty("currentDamagesApplied");
        constList = GetTarget.FindProperty("constantDamageType");
        maxFuel = GetTarget.FindProperty("maxFuel");
        currFuel = GetTarget.FindProperty("currFuel");
    }
    bool show = true;
    public override void OnInspectorGUI()
    {
        //Update our list
        base.DrawDefaultInspector();

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("Maximum Fuel");
        t.maxFuel = EditorGUILayout.FloatField(maxFuel.floatValue);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Current Fuel");
        t.currFuel = EditorGUILayout.Slider(t.currFuel, 0, t.maxFuel);

        GetTarget.Update();


        //Resize our list
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        CurrListSize = currList.arraySize;
        constListSize = constList.arraySize;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        if (GUILayout.Button("Add"))
        {

            t.AddDamageInList();
            constListSize++;
            Debug.Log(constListSize);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        showConstList = EditorGUILayout.BeginFoldoutHeaderGroup(showConstList, "Damage types list");

        for (int i = 0; i < constListSize;)
        {
            if (!showConstList) break;
            SerializedProperty MyListRef = constList.GetArrayElementAtIndex(i);
            SerializedProperty dmgName = MyListRef.FindPropertyRelative("damageName");
            SerializedProperty dmgRatio = MyListRef.FindPropertyRelative("damagePercentagePerSecond");
            SerializedProperty dmgPersist = MyListRef.FindPropertyRelative("isPersistant");
            SerializedProperty dmgTime = MyListRef.FindPropertyRelative("duration");

            EditorGUILayout.BeginHorizontal();
            dmgName.stringValue = EditorGUILayout.TextField("Name", dmgName.stringValue);
            dmgRatio.floatValue = EditorGUILayout.FloatField("Damage %", dmgRatio.floatValue);
            EditorGUILayout.EndHorizontal();

            dmgPersist.boolValue = EditorGUILayout.Toggle("Is Persistant", dmgPersist.boolValue);
            if (!dmgPersist.boolValue)
                dmgTime.floatValue = EditorGUILayout.FloatField("Damage Time", dmgTime.floatValue);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Launch"))
            {
                t.AddConstDamage(dmgName.stringValue/*MyListRef.objectReferenceValue as System.Object as TruckHealth.ConstantDamageType*/);
            }

            if (GUILayout.Button("Erase"))
            {
                constList.DeleteArrayElementAtIndex(i);
                constListSize--;
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (i == constListSize - 1)
                break;
            else i++;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();



        

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (CurrListSize == 0) showList = false;

        //Display our list to the inspector window
        showList = EditorGUILayout.BeginFoldoutHeaderGroup(showList, "Current Damages Applied(" + CurrListSize + ")");

        for (int i = 0; i < CurrListSize;)
        {
            if (!showList) break;
            SerializedProperty MyListRef = currList.GetArrayElementAtIndex(i);
            SerializedProperty dmgName = MyListRef.FindPropertyRelative("damageName");
            SerializedProperty dmgRatio = MyListRef.FindPropertyRelative("damagePercentagePerSecond");
            SerializedProperty dmgCurrTime = MyListRef.FindPropertyRelative("currTime");
            SerializedProperty dmgPersist = MyListRef.FindPropertyRelative("isPersistant");

            EditorGUILayout.BeginHorizontal();
            dmgName.stringValue = EditorGUILayout.TextField("Name", dmgName.stringValue);
            dmgRatio.floatValue = EditorGUILayout.FloatField("Damage %", dmgRatio.floatValue);
            dmgCurrTime.floatValue = EditorGUILayout.FloatField("Time left", dmgCurrTime.floatValue);
            
            EditorGUILayout.EndHorizontal();
            if (!dmgPersist.boolValue)
                dmgCurrTime.floatValue = EditorGUILayout.FloatField("Time left", dmgCurrTime.floatValue);
            if (GUILayout.Button("Stop Damage"))
            {
                currList.DeleteArrayElementAtIndex(i);
                CurrListSize--;
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (i == CurrListSize - 1)
                break;
            else i++;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();
    }
}