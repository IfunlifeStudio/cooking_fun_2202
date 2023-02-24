using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(EventManager))]
public class EventManagerInspector : Editor
{
    public const string PNAME = "registerEvents";
    private bool[] showItemSlots;
    EventManager gameEventController;
    SerializedProperty registerEventsProperties;
    private void OnEnable()
    {
        gameEventController = (EventManager)target;
        registerEventsProperties = serializedObject.FindProperty(PNAME);
        showItemSlots = new bool[gameEventController.registerEvents.Count];
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add", GUILayout.Width(100)))
        {
            gameEventController.registerEvents.Add(null);
            showItemSlots = new bool[gameEventController.registerEvents.Count];
        }
        if (GUILayout.Button("Sort", GUILayout.Width(100)))
        {
            gameEventController.registerEvents.Sort();
        }
        if (GUILayout.Button("Reset", GUILayout.Width(100)))
        {
            foreach (GameEvent ge in gameEventController.registerEvents)
            {
                ge.Reset();
                UnityEditor.EditorUtility.SetDirty(ge);
            }
            gameEventController.SaveToJson();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        for (int i = gameEventController.registerEvents.Count - 1; i > -1; i--)
            ShowGameEvent(i);
        serializedObject.ApplyModifiedProperties();
    }
    private void ShowGameEvent(int index)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;
        GUIStyle style = new GUIStyle(EditorStyles.foldout);
        if (gameEventController.registerEvents[index])
        {
            if (!gameEventController.registerEvents[index].isCompleted)
            {
                style.fontStyle = FontStyle.Normal;
                style.normal.textColor = Color.blue;
                style.onNormal.textColor = Color.blue;
                style.focused.textColor = Color.blue;
                style.onFocused.textColor = Color.blue;
            }
            else
            {
                style.fontStyle = FontStyle.Normal;
                style.normal.textColor = Color.gray;
                style.onNormal.textColor = Color.gray;
                style.focused.textColor = Color.gray;
                style.onFocused.textColor = Color.gray;
            }
            showItemSlots[index] = EditorGUILayout.Foldout(showItemSlots[index], gameEventController.registerEvents[index].name, style);// Display a foldout to determine whether the GUI should be shown or not.
        }
        else
        {
            style.fontStyle = FontStyle.Normal;
            style.normal.textColor = Color.red;
            style.onNormal.textColor = Color.red;
            style.focused.textColor = Color.red;
            style.onFocused.textColor = Color.red;
            showItemSlots[index] = EditorGUILayout.Foldout(showItemSlots[index], "New GameEvent", style);
        }
        if (showItemSlots[index])
        {
            EditorGUILayout.PropertyField(registerEventsProperties.GetArrayElementAtIndex(index), GUIContent.none);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Remove", GUILayout.Width(100)))
                gameEventController.registerEvents.RemoveAt(index);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
}
