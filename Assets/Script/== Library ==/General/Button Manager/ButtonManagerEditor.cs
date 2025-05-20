#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonManager))]
public class ButtonManagerEditor : Editor {
    SerializedProperty toggleLoadSceneProp;
    SerializedProperty sceneToLoadProp;

    private void OnEnable() {
        toggleLoadSceneProp = serializedObject.FindProperty("toggleLoadScene");
        sceneToLoadProp = serializedObject.FindProperty("sceneToLoad");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        // Memanggil header dari CustomScriptNameInspector
        CustomScriptNameInspector.DrawScriptHeader(target);

        // Menampilkan semua properti kecuali toggleLoadScene & sceneToLoad
        SerializedProperty prop = serializedObject.GetIterator();
        bool enterChildren = true;

        while (prop.NextVisible(enterChildren)) {
            // Hanya menampilkan properti yang bukan toggleLoadScene atau sceneToLoad
            if (prop.name != "toggleLoadScene" && prop.name != "sceneToLoad") {
                EditorGUILayout.PropertyField(prop, true);
            }
            enterChildren = false;
        }

        // Menampilkan toggle secara eksplisit
        EditorGUILayout.PropertyField(toggleLoadSceneProp);

        // Menampilkan sceneToLoad hanya jika toggle aktif
        if (toggleLoadSceneProp.boolValue) {
            EditorGUILayout.PropertyField(sceneToLoadProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif