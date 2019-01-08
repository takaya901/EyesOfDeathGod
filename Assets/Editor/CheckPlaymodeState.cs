using UnityEditor;

static class GameViewUtil
{   
    [InitializeOnLoadMethod]
    static void CheckPlaymodeState()
    {
        // プレイモードが変わったときのコールバックに登録する
        EditorApplication.playModeStateChanged += x =>
        {
            // Playモードに変わったときに処理する
            if (x == PlayModeStateChange.EnteredPlayMode)
            {
                var asm = typeof(Editor).Assembly;
                var type = asm.GetType("UnityEditor.GameView");
                EditorWindow gameView = EditorWindow.GetWindow(type);

                // GameViewクラスのSnapZoomプライベートインスタンスメソッドを引数1で呼び出す
                var flag = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                type.GetMethod("SnapZoom", flag, null, new System.Type[] { typeof(float) }, null).Invoke(gameView, new object[] { 1 });
            }
        };
    }
}