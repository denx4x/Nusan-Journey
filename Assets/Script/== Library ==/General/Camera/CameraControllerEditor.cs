#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraMovement))]
public class CameraMovementEditor : Editor {

    // Deklarasi properti
    SerializedProperty useCurrentObjectProp, objectToMoveProp, transitionSpeedProp, useTransformTargetProp;
    SerializedProperty useLocalCoordinatesProp, manualTargetPositionProp, manualTargetRotationProp, targetTransformProp;

    private void OnEnable() {
        // Mengambil referensi properti dari skrip
        useCurrentObjectProp = serializedObject.FindProperty("useCurrentObject");
        objectToMoveProp = serializedObject.FindProperty("objectToMove");
        transitionSpeedProp = serializedObject.FindProperty("transitionSpeed");
        useTransformTargetProp = serializedObject.FindProperty("useTransformTarget");
        useLocalCoordinatesProp = serializedObject.FindProperty("useLocalCoordinates"); // Ambil properti baru
        manualTargetPositionProp = serializedObject.FindProperty("manualTargetPosition");
        manualTargetRotationProp = serializedObject.FindProperty("manualTargetRotation");
        targetTransformProp = serializedObject.FindProperty("targetTransform");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        // Menggunakan kembali DrawScriptHeader dari contoh Anda jika ada
        CustomScriptNameInspector.DrawScriptHeader(target);

        EditorGUILayout.LabelField("Object to Move Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(useCurrentObjectProp);
        if (!useCurrentObjectProp.boolValue) {
            EditorGUILayout.PropertyField(objectToMoveProp);
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Target Destination Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(transitionSpeedProp);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(useTransformTargetProp);

        if (useTransformTargetProp.boolValue) {
            EditorGUILayout.PropertyField(targetTransformProp);
        } else {
            // --- TAMPILKAN TOGGLE BARU DI INSPECTOR ---
            EditorGUILayout.PropertyField(useLocalCoordinatesProp);
            // --- --------------------------------- ---
            EditorGUILayout.PropertyField(manualTargetPositionProp);
            EditorGUILayout.PropertyField(manualTargetRotationProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif