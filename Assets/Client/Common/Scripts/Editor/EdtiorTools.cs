using UnityEditor;
using UnityEditor.SceneManagement;

public class EdtiorTools
{
    public static string editorScenePath = "Assets/Client/Scenes/MainScene.unity";

    [MenuItem("NGLG/Run Editor Scene", false, 1)]
    static void OpenEditorScene()
    {
        EditorSceneManager.OpenScene(editorScenePath);
        EditorApplication.isPlaying = true;
    }

    
}
