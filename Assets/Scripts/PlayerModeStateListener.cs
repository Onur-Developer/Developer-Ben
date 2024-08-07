using UnityEditor;

/* [InitializeOnLoad]
public class PlayModeStateListener
{
    static PlayModeStateListener()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            MyFunction();
        }
    }

    private static void MyFunction()
    {
        GameManager gm = GameManager.instance;

        for (int i = 0; i < gm.cmBen.Count; i++)
        {
            gm.cmBen[i].isWriting = false;
            gm.cmBen[i].isSound = false;
        }
    } 
} */
