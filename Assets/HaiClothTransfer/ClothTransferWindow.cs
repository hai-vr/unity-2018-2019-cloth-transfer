// This is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// For more information, please refer to <https://unlicense.org>

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using HaiClothTransfer.HctPartialKdTreeLib;
using UnityEditor;
using UnityEngine;

namespace HaiClothTransfer
{
    public class ClothTransferWindow : EditorWindow
    {
        private Cloth cloth;
        private ClothTransferData clothTransferData;
        private bool foldout;
        private bool optInApproximate;
        private InjectionExecutionPath algorithmExecutionPath = InjectionExecutionPath.NotExecuted;

        private const string CtLoadClothData = "Load cloth data / Clothの修正";
        private const string CtSaveClothData = "Save cloth data / Clothの保存";
        private const string CtClothToModify = "Cloth to modify / Clothの修正";
        private const string CtData = "Data / 修正";
        private const string CtOther = "Other / その他";
        private const string CtCloth = "Cloth";
        private const string CtOptInApproximateEn = "Allow inexact";
        private const string CtOptInApproximateJp = "寛容さ";
        private const string CtOptInApproximate = CtOptInApproximateEn + " / " + CtOptInApproximateJp;
        private const string CtNearestNeighbor = "Some vertices were approximated because their positions did not match. Please check that you are importing the correct data for this mesh.\n\n頂点の位置が一致しないため、一部の頂点を近似しています。このメッシュに対して正しいデータをインポートしているか確認してください。";
        private const string CtMissed = "Some vertices were not resolved. Please check that you are importing the correct data for this mesh.\nIf necessary, enable \"" + CtOptInApproximateEn + "\".\n\nいくつかの頂点が見つかりませんでした。このメッシュに対して正しいデータをインポートしているか確認してください。\n必要に応じて「" + CtOptInApproximateJp + "」を有効にする。";
        private const string CtNoVertices = @"The cloth component must be enabled and visible in the scene. Please make sure:
- The GameObject is enabled in the hierarchy
- The parents of the GameObject is enabled in the hierarchy
- The cloth component is enabled
When done, put the cursor on this window again.

Cloth componentが有効で、かつ表示されている必要があります。以下の点を確認してください。
- GameObjectが階層内で有効になっている。
- GameObjectの親が、階層内で有効になっている。
- Cloth component有効になっている。
終わったら、再びこのウィンドウにカーソルを合わせます。";
        private const string CtNoVerticesInAsset = @"This cloth data asset is invalid and must be exported again from the original.

Clothの修正は無効なので、オリジナルから再度エクスポートする必要があります。";

        private void OnEnable()
        {
            titleContent = new GUIContent("Cloth Transfer");
        }

        private void OnGUI()
        {
            var isUnity2018 = Application.unityVersion.StartsWith("2018");

            if (isUnity2018) { LayoutForSave(); }
            else { LayoutForLoad(); }

            foldout = EditorGUILayout.Foldout(foldout, CtOther);
            if (foldout)
            {
                if (isUnity2018) { LayoutForLoad(); }
                else { LayoutForSave(); }
            }
        }

        private void LayoutForSave()
        {
            EditorGUILayout.LabelField(CtSaveClothData, EditorStyles.boldLabel);
            cloth = (Cloth) EditorGUILayout.ObjectField(CtCloth, cloth, typeof(Cloth), true);

            if (cloth != null && (cloth.vertices == null || cloth.vertices.Length == 0))
            {
                EditorGUILayout.HelpBox(CtNoVertices, MessageType.Warning);
            }

            EditorGUI.BeginDisabledGroup(cloth == null || cloth.vertices == null || cloth.vertices.Length == 0);
            if (GUILayout.Button(CtSaveClothData))
            {
                SaveClothData();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void LayoutForLoad()
        {
            EditorGUILayout.LabelField(CtLoadClothData, EditorStyles.boldLabel);
            cloth = (Cloth) EditorGUILayout.ObjectField(CtClothToModify, cloth, typeof(Cloth), true);
            clothTransferData = (ClothTransferData) EditorGUILayout.ObjectField(CtData, clothTransferData, typeof(ClothTransferData), true);
            optInApproximate = EditorGUILayout.Toggle(CtOptInApproximate, optInApproximate);

            if (cloth != null && (cloth.vertices == null || cloth.vertices.Length == 0))
            {
                EditorGUILayout.HelpBox(CtNoVertices, MessageType.Warning);
            }
            if (clothTransferData != null && (clothTransferData.coefficients == null || clothTransferData.coefficients.Length == 0))
            {
                EditorGUILayout.HelpBox(CtNoVerticesInAsset, MessageType.Error);
            }

            EditorGUI.BeginDisabledGroup(
                cloth == null || cloth.vertices == null || cloth.vertices.Length == 0
                || clothTransferData == null || clothTransferData.coefficients == null || clothTransferData.coefficients.Length == 0
            );
            if (GUILayout.Button(CtLoadClothData))
            {
                algorithmExecutionPath = LoadClothData();
            }
            EditorGUI.EndDisabledGroup();
            switch (algorithmExecutionPath)
            {
                case InjectionExecutionPath.NearestNeighbor:
                    EditorGUILayout.HelpBox(CtNearestNeighbor, MessageType.Warning);
                    break;
                case InjectionExecutionPath.Missed:
                    EditorGUILayout.HelpBox(CtMissed, MessageType.Warning);
                    break;
                case InjectionExecutionPath.NotExecuted:
                case InjectionExecutionPath.Perfect:
                default:
                    break;
            }
        }

        private void SaveClothData()
        {
            var transferCoefficients = ExtractCoefficients();

            var dataAsset = CreateInstance<ClothTransferData>();
            dataAsset.coefficients = transferCoefficients;

            var savePath = EditorUtility.SaveFilePanel(CtSaveClothData, Application.dataPath, "", "asset");
            if (savePath == null || savePath.Trim() == "") return;
            if (!savePath.StartsWith(Application.dataPath)) return;

            var assetPath = "Assets" + savePath.Substring(Application.dataPath.Length);
            AssetDatabase.CreateAsset(dataAsset, assetPath);
            EditorGUIUtility.PingObject(dataAsset);
        }

        private ClothTransferCoefficient[] ExtractCoefficients()
        {
            var validCoefficientCount = ValidCoefficientCount();
            var transferCoefficients = new ClothTransferCoefficient[validCoefficientCount];
            for (var index = 0; index < validCoefficientCount; index++)
            {
                var coefficient = cloth.coefficients[index];
                transferCoefficients[index] = new ClothTransferCoefficient
                {
                    vertex = cloth.vertices[index],
                    maxDistance = coefficient.maxDistance,
                    collisionSphereDistance = coefficient.collisionSphereDistance
                };
            }

            return transferCoefficients;
        }

        private InjectionExecutionPath LoadClothData()
        {
            Undo.RecordObject(cloth, CtLoadClothData);
            var vertexToCoefficient = new Dictionary<Vector3, ClothTransferCoefficient>();
            foreach (var clothTransferCoefficient in clothTransferData.coefficients)
            {
                vertexToCoefficient[clothTransferCoefficient.vertex] = clothTransferCoefficient;
            }

            var coefficients = cloth.coefficients; // Preserve the array size of coefficients, regardless of the array size of vertices. See ValidCoefficientCount().
            var injectionExecutionPath = InjectCoefficients(coefficients, vertexToCoefficient);
            cloth.coefficients = coefficients;

            return injectionExecutionPath;
        }

        private InjectionExecutionPath InjectCoefficients(ClothSkinningCoefficient[] mutatedCoefficients, Dictionary<Vector3, ClothTransferCoefficient> vertexToCoefficient)
        {
            var injectionExecutionPath = InjectionExecutionPath.Perfect;
            var validCoefficientCount = ValidCoefficientCount();
            KdVector3Tree<ClothTransferCoefficient> kdTreeNullable = null;

            for (var index = 0; index < validCoefficientCount; index++)
            {
                var vertex = cloth.vertices[index];
                var found = vertexToCoefficient.TryGetValue(vertex, out var clothTransferCoefficient);
                if (!found)
                {
                    if (optInApproximate)
                    {
                        if (kdTreeNullable == null)
                        {
                            kdTreeNullable = InitializeKdTree(vertexToCoefficient);
                        }
                        injectionExecutionPath = InjectionExecutionPath.NearestNeighbor;
                        clothTransferCoefficient = FindNearestNeighbor(vertex, kdTreeNullable);
                    }
                    else
                    {
                        injectionExecutionPath = InjectionExecutionPath.Missed;
                        clothTransferCoefficient = new ClothTransferCoefficient();
                    }
                }

                mutatedCoefficients[index] = new ClothSkinningCoefficient
                {
                    maxDistance = clothTransferCoefficient.maxDistance,
                    collisionSphereDistance = clothTransferCoefficient.collisionSphereDistance
                };
            }

            return injectionExecutionPath;
        }

        private enum InjectionExecutionPath
        {
            NotExecuted, Perfect, NearestNeighbor, Missed
        }

        private KdVector3Tree<ClothTransferCoefficient> InitializeKdTree(Dictionary<Vector3, ClothTransferCoefficient> vertexToCoefficient)
        {
            var kdTree = new KdVector3Tree<ClothTransferCoefficient>();
            foreach (var coefficient in vertexToCoefficient)
            {
                kdTree.Add(coefficient.Key, coefficient.Value);
            }
            kdTree.Balance();

            return kdTree;
        }

        private ClothTransferCoefficient FindNearestNeighbor(Vector3 vertex, KdVector3Tree<ClothTransferCoefficient> kdTree)
        {
            return kdTree.GetNearestNeighbours(vertex, 1).Value;
        }

        private int ValidCoefficientCount()
        {
            // https://github.com/hai-vr/unity-2018-2019-cloth-transfer/issues/3
            // Sometimes the number of cloth vertices is lower than the number of cloth coefficients
            return Math.Min(cloth.coefficients.Length, cloth.vertices.Length);
        }

        [MenuItem("Window/Haï/Cloth Transfer")]
        public static void OpenEditor()
        {
            Obtain().Show();
        }

        [MenuItem ("CONTEXT/Cloth/Haï Cloth Transfer")]
        public static void OpenEditor(MenuCommand command)
        {
            var window = Obtain();
            window.UsingCloth((Cloth)command.context);
            window.Show();
        }

        private void UsingCloth(Cloth clothInContext)
        {
            cloth = clothInContext;
        }

        private static ClothTransferWindow Obtain()
        {
            var editor = GetWindow<ClothTransferWindow>(false, null, false);
            return editor;
        }
    }
}
#endif
