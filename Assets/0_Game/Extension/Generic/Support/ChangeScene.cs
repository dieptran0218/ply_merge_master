#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;


public class ChangeScene : Editor {

    [MenuItem("Open Scene/Loading #1")]
    public static void OpenLoading()
    {
        OpenScene("Loading");
    }

    [MenuItem("Open Scene/Game #2")]
    public static void OpenGame()
    {
        OpenScene("Game");
    }

    [MenuItem("Open Scene/XepMap #3")]
    public static void OpenXepMap()
    {
        OpenScene("XepMap");
    }

    private static void OpenScene (string sceneName) {
		if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ()) {
			EditorSceneManager.OpenScene ("Assets/0_Game/Scenes/" + sceneName + ".unity");
		}
	}
}
#endif