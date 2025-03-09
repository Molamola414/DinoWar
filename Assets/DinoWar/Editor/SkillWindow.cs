
using UnityEngine;
using UnityEditor;

public class SkillTreeWindow : EditorWindow
{
    [MenuItem("Window/SkillTree")]
    public static void ShowWindow(){
        EditorWindow.GetWindow<SkillTreeWindow>("SkillTree");

    }


    void OnGUI(){

    }
 
    
}
