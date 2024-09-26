/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Description: Handels Scene Loading
 */

using UnityEngine.SceneManagement;

public static class Loader {
    public enum Scene {
        GameScene,
        LoadingScene,
        MenuScene,
    }
    static Scene targetScene;

    public static void Load(Scene scene) {
        targetScene = scene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }
    public static void LoaderCallback() {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
