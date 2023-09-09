//C#
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//
// TODO: REMOVE, PROBABLY
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//

// #if UNITY_EDITOR

// public class DetectDuplicates : AssetModificationProcessor
// {
//     public static List<string> newAssets = new List<string>();
//     static void OnWillCreateAsset(string aMetaAssetPath)
//     {
//         int index = aMetaAssetPath.IndexOf(".meta");
//         string assetPath = aMetaAssetPath.Substring(0, index);
//         newAssets.Add(assetPath);
//     }
// }

// class DetectDuplicatesPostprocessor : AssetPostprocessor
// {
//     static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
//     {
//         if (DetectDuplicates.newAssets.Count == 0)
//             return;

//         foreach (var assetPath in importedAssets)
//         {
//             if (DetectDuplicates.newAssets.Contains(assetPath))
//             {
//                 GameObject obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
//                 if (obj == null)
//                     continue;

//                 if (!obj.TryGetComponent<MonoSaveable>(out var saveObj))
//                     continue;

//                 saveObj.RegenerateUUID();
//             }
//         }
//         DetectDuplicates.newAssets.Clear();
//     }
// }

// #endif
