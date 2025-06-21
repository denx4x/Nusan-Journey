#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonoBehaviour), true)]
public class CustomScriptNameInspector : Editor {
    public override void OnInspectorGUI() {
        DrawScriptHeader(target);
        DrawDefaultInspector();
    }

    public static void DrawScriptHeader(Object target) {
        // Menampilkan sedikit jarak di atas
        GUILayout.Space(10);

        // Mendapatkan nama skrip
        string scriptName = target.GetType().Name;

        // Membuat gaya dengan latar belakang hitam dan teks putih
        GUIStyle backgroundStyle = new GUIStyle();
        backgroundStyle.normal.background = MakeTex(1, 1, Color.black);
        backgroundStyle.padding = new RectOffset(10, 10, 10, 10);

        GUIStyle textStyle = new GUIStyle(EditorStyles.whiteLabel);
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontSize = 14;
        textStyle.fontStyle = FontStyle.Bold;

        // Menampilkan nama skrip dengan latar belakang hitam
        EditorGUILayout.BeginVertical(backgroundStyle);
        EditorGUILayout.LabelField(scriptName, textStyle);
        EditorGUILayout.EndVertical();

        // Menampilkan sedikit jarak di bawah
        GUILayout.Space(10);

    }

    private static Texture2D MakeTex(int width, int height, Color color) {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = color;
        }
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}

#endif