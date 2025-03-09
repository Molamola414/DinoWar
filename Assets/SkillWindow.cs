using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SkillWindow : EditorWindow
{
    [MenuItem("Window/SkillTree")]
    public static void ShowWindow(){
        EditorWindow.GetWindow<SkillWindow>("SkillWindow");

    }


    void OnGUI(){

    }
 
    
}
