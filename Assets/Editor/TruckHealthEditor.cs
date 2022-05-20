using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(TruckHealth))]
public class TruckHealthEditor : Editor
{

    #region tmp
    /*
    TruckHealth t;


    
    enum displayFieldType { DisplayAsAutomaticFields, DisplayAsCustomizableGUIFields }
    displayFieldType DisplayFieldType;

    SerializedObject GetTarget;
    SerializedProperty ThisList;
    int ListSize;

    void OnEnable()
    {
        t = (TruckHealth)target;
        GetTarget = new SerializedObject(t);
        ThisList = GetTarget.FindProperty("constantDamageType"); // Find the List in our script and create a refrence of it
    }

    public override void OnInspectorGUI()
    {
        //Update our list

        GetTarget.Update();

        //Choose how to display the list<> Example purposes only
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        DisplayFieldType = (displayFieldType)EditorGUILayout.EnumPopup("", DisplayFieldType);

        //Resize our list
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Define the list size with a number");
        ListSize = ThisList.arraySize;
        ListSize = EditorGUILayout.IntField("List Size", ListSize);

        if (ListSize != ThisList.arraySize)
        {
            while (ListSize > ThisList.arraySize)
            {
                ThisList.InsertArrayElementAtIndex(ThisList.arraySize);
            }
            while (ListSize < ThisList.arraySize)
            {
                ThisList.DeleteArrayElementAtIndex(ThisList.arraySize - 1);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Or");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Or add a new item to the List<> with a button
        EditorGUILayout.LabelField("Add a new item with a button");

        if (GUILayout.Button("Add New"))
        {
            t.constantDamageType.Add(new TruckHealth.ConstantDamageType());
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Display our list to the inspector window

        for (int i = 0; i < ThisList.arraySize; i++)
        {
            SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
            SerializedProperty dmgName = MyListRef.FindPropertyRelative("damageName");
            SerializedProperty dmgRatio = MyListRef.FindPropertyRelative("damagePercentagePerSecond");
            SerializedProperty dmgPersist = MyListRef.FindPropertyRelative("isPersistant");
            SerializedProperty dmgDuration = MyListRef.FindPropertyRelative("duration");


            // Display the property fields in two ways.

            if (DisplayFieldType == 0)
            {// Choose to display automatic or custom field types. This is only for example to help display automatic and custom fields.
                //1. Automatic, No customization <-- Choose me I'm automatic and easy to setup
                EditorGUILayout.LabelField("Automatic Field By Property Type");
                EditorGUILayout.PropertyField(dmgName);
                EditorGUILayout.PropertyField(dmgRatio);
                EditorGUILayout.PropertyField(dmgPersist);
                EditorGUILayout.PropertyField(dmgDuration);

                // Array fields with remove at index
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Array Fields");

            }
            else
            {
                //Or

                //2 : Full custom GUI Layout <-- Choose me I can be fully customized with GUI options.
                EditorGUILayout.LabelField("Customizable Field With GUI");
                dmgName.stringValue = EditorGUILayout.TextField("Name", dmgName.stringValue);
                dmgRatio.floatValue = EditorGUILayout.FloatField("Damage %", dmgRatio.floatValue);
                dmgPersist.boolValue = EditorGUILayout.ToggleLeft("My Custom Float", dmgPersist.boolValue);
                if (!dmgPersist.boolValue)
                    dmgDuration.floatValue = EditorGUILayout.FloatField("Duration", dmgDuration.floatValue);


                // Array fields with remove at index
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Array Fields");

                if (GUILayout.Button("Add New Index", GUILayout.MaxWidth(130), GUILayout.MaxHeight(20)))
                {
                    ThisList.InsertArrayElementAtIndex(ThisList.arraySize);
                    ThisList.GetArrayElementAtIndex(ThisList.arraySize - 1).intValue = 0;
                }

                for (int a = 0; a < ThisList.arraySize; a++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("My Custom Int (" + a.ToString() + ")", GUILayout.MaxWidth(120));
                    ThisList.GetArrayElementAtIndex(a).intValue = EditorGUILayout.IntField("", ThisList.GetArrayElementAtIndex(a).intValue, GUILayout.MaxWidth(100));
                    if (GUILayout.Button("-", GUILayout.MaxWidth(15), GUILayout.MaxHeight(15)))
                    {
                        ThisList.DeleteArrayElementAtIndex(a);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space();

            //Remove this index from the List
            EditorGUILayout.LabelField("Remove an index from the List<> with a button");
            if (GUILayout.Button("Remove This Index (" + i.ToString() + ")"))
            {
                ThisList.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();
    
    */

    #endregion

    TruckHealth t;

    bool showList = false;
    bool showConstList = false;
    SerializedObject GetTarget;
    SerializedProperty currList;
    SerializedProperty constList;
    int CurrListSize;
    int constListSize;

    void OnEnable()
    {
        t = (TruckHealth)target;
        GetTarget = new SerializedObject(t);
        currList = GetTarget.FindProperty("constantDamageType");
        constList = GetTarget.FindProperty("currentDamagesApplied");
    }
    bool show = true;
    public override void OnInspectorGUI()
    {
        //Update our list
        //base.DrawDefaultInspector();

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

        //if (constListSize == 0) showList = false;
        if (GUILayout.Button("Add"))
        {
            //constList.InsertArrayElementAtIndex(0);
            t.AddDamageInList();
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        showConstList = EditorGUILayout.BeginFoldoutHeaderGroup(showConstList, "Damage types list");

        for (int i = 0; i < constList.arraySize; i++)
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
            //EditorGUILayout.BeginHorizontal();
            dmgPersist.boolValue = EditorGUILayout.Toggle("Is Persistant", dmgPersist.boolValue);
            if (!dmgPersist.boolValue)
                dmgTime.floatValue = EditorGUILayout.FloatField("Damage Time", dmgTime.floatValue);
            //EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Launch"))
            {
                t.AddConstDamage(dmgName.stringValue/*MyListRef.objectReferenceValue as System.Object as TruckHealth.ConstantDamageType*/);
            }

            if (GUILayout.Button("Erase"))
            {
                constList.DeleteArrayElementAtIndex(i);
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();



        

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (CurrListSize == 0) showList = false;

        //Display our list to the inspector window
        showList = EditorGUILayout.BeginFoldoutHeaderGroup(showList, "Current Damages Applied(" + CurrListSize + ")");

        for (int i = 0; i < currList.arraySize; i++)
        {
            if (!showList) break;
            SerializedProperty MyListRef = currList.GetArrayElementAtIndex(i);
            SerializedProperty dmgName = MyListRef.FindPropertyRelative("damageName");
            SerializedProperty dmgRatio = MyListRef.FindPropertyRelative("damagePercentagePerSecond");
            SerializedProperty dmgCurrTime = MyListRef.FindPropertyRelative("currTime");
            //SerializedProperty dmgShow = MyListRef.FindPropertyRelative("show");
            //bool show = dmgShow.boolValue;
            //show = EditorGUILayout.BeginFoldoutHeaderGroup(show, dmgName.stringValue);
            //if (show)
            //{
            //    dmgName.stringValue = EditorGUILayout.TextField("Name", dmgName.stringValue);
            //    dmgRatio.floatValue = EditorGUILayout.FloatField("Damage %", dmgRatio.floatValue);
            //}
            EditorGUILayout.BeginHorizontal();
            dmgName.stringValue = EditorGUILayout.TextField("Name", dmgName.stringValue);
            dmgRatio.floatValue = EditorGUILayout.FloatField("Damage %", dmgRatio.floatValue);
            dmgCurrTime.floatValue = EditorGUILayout.FloatField("Time left", dmgCurrTime.floatValue);
            
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Stop Damage"))
            {
                currList.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();
    }
}