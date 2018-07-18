using PokemonBattele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

class ScriptObjectHelp 
{
    [MenuItem("Pokemon/技能特效一键配置")]
    public static void QuickSetSkillEffect()
    {
        var skilllist = SearchGameObject.SearchGameObjectList<Skill>
           ("Assets/Resources/SkillAsset");

        var effectlist = SearchGameObject.SearchGameObjectList<SkillEffect>
           ("Assets/Skill/SkillEffectAsset");
        var skillDic = skilllist.ToDictionary(e => e.SKillID.ToString());

        int size = effectlist.Count;
        for (int i = 0; i < size; ++i)
        {
            var e = effectlist[i];
            if (!skillDic.ContainsKey(e.name))
            {
                Debug.Log(e.name);
                continue;
            }
            skillDic[e.name].effect = e;
            EditorUtility.SetDirty(skillDic[e.name]);
            EditorUtility.DisplayCancelableProgressBar("配置技能特效", i + "/" + size, (float)i / size);
        }
        EditorUtility.ClearProgressBar();
        
        AssetDatabase.SaveAssets();
    }

    
}
