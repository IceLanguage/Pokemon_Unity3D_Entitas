using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class My_AtlasPackage  {

    //快速设置精灵
    [MenuItem("Assets/MyEditor/SpriteSet &c")]
    static void SpriteSet()
    {
        //如果选择了对象
        if (Selection.objects.Length > 0)
        {
            //获得所有选中的Texture对象
            foreach (Texture texture in Selection.objects)
            {
                string selectionPath = AssetDatabase.GetAssetPath(texture);
                TextureImporter textureIm = AssetImporter.GetAtPath(selectionPath) as TextureImporter;
               
                //<-----------------设置参数---------------------->
                textureIm.textureType = TextureImporterType.Sprite;
                textureIm.spriteImportMode = SpriteImportMode.Multiple;
                textureIm.isReadable = true;
                TextureImporterPlatformSettings tis = textureIm.GetDefaultPlatformTextureSettings();
                tis.allowsAlphaSplitting = true;
                tis.overridden = true;
                tis.format = TextureImporterFormat.RGBA32;
                textureIm.SetPlatformTextureSettings(tis);
                //<-----------------设置参数---------------------->

                
            }
        }
    }

    //导出单个精灵
    [MenuItem("Assets/MyEditor/SingleSpritesExport &x")]
    static void SingleSpritesExport()
    {
        string resourcesPath = "Assets/Resources/";
        foreach (Object obj in Selection.objects)
        {
            string selectionPath = AssetDatabase.GetAssetPath(obj);

            // 必须最上级是"Assets/Resources/"
            if (selectionPath.StartsWith(resourcesPath))
            {
                string selectionExt = System.IO.Path.GetExtension(selectionPath);
                if (selectionExt.Length == 0)
                {
                    continue;
                }

                // 得到导出路径
                string loadPath = selectionPath.Remove(selectionPath.Length - selectionExt.Length);
                loadPath = loadPath.Substring(resourcesPath.Length);

                // 加载此文件下的所有资源
                Sprite[] sprites = Resources.LoadAll<Sprite>(loadPath);
                if (sprites.Length > 0)
                {
                    // 创建导出文件夹
                    string outPath = Application.dataPath + "/outSprite/" + loadPath;
                    System.IO.Directory.CreateDirectory(outPath);

                    foreach (Sprite sprite in sprites)
                    {
                        // 创建单独的纹理
                        Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
                        tex.SetPixels(sprite.texture.GetPixels((int)sprite.rect.xMin, (int)sprite.rect.yMin,
                            (int)sprite.rect.width, (int)sprite.rect.height));
                        tex.Apply();

                        // 写入成PNG文件
                        System.IO.File.WriteAllBytes(outPath + "/" + sprite.name + ".png", tex.EncodeToPNG());
                    }
                    Debug.Log(string.Format("Export {0} to {1}", loadPath, outPath));
                }
            }
        }
        Debug.Log("Export All Sprites Finished");
        AssetDatabase.Refresh();
    }


}
