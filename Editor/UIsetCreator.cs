using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using UIset.util;
using Newtonsoft.Json.Linq;
//ModularAvatar用
using nadena.dev.modular_avatar.core;
using UIset.Layar;

namespace UIset
{

    /// <summary>
    /// UIset用のアニメーターを、セットしたアバター用に新規作成します
    /// </summary>
    ///
    class UIsetCreator
    {
        //コントローラーとアニメーションを置くフォルダ
        private const string avatarSettingInfoPath = "Assets/UIset/AvatarSettingInfo";
        //コピー用マテリアル
        private const string currentMaterialFolder = "Assets/UIset/src/material/Material";
        private bool writeDefault = false;
        ObjectReader or = new ObjectReader();


        //UIFXのセットアップ
        public void CreateController(VRCAvatarDescriptor avatarDescriptor, GameObject avatarObject, GameObject prefabUIset)
        {
            string avatarName = avatarObject.name;
            string controllerPath = avatarSettingInfoPath + "/" + avatarName + "/" + avatarName + ".controller";
            try
            {
                //アバター用のフォルダがなければ作成する
                if (!Directory.Exists(avatarSettingInfoPath + "/" + avatarName))
                {
                    Directory.CreateDirectory(avatarSettingInfoPath + "/" + avatarName);
                }

                string destinationPath = avatarSettingInfoPath + "/" + avatarName + "/Material";
                //アバター用にマテリアルをコピペする
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                    AssetManipulator.CopyDirectoryRecursive(currentMaterialFolder, destinationPath);
                    // アセットデータベースを更新して、変更を反映
                    AssetDatabase.Refresh();
                }


                //コントローラーがない場合アバター用のコントローラー作成
                if (!File.Exists(controllerPath))
                {
                    AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);





                    //メニュー用のプロパティセット(Jsonで保存)
                    string json = JsonReader.ReadJson("Assets/UIset/Editor/UIsetInfo.json");
                    JObject jsonObj = JObject.Parse(json);
                    AddLayer ad = new AddLayer();
                    ad.setPass(controllerPath);




                    //初期表示レイヤー
                    AnimationClip animeNormarized = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Normarized.anim", typeof(AnimationClip)) as AnimationClip;
                    AnimatorControllerLayer NormarizedLayer = new AnimatorControllerLayer
                    {
                        name = "NormarizedLayer",
                        defaultWeight = 1,
                        stateMachine = new AnimatorStateMachine()
                    };
                    var stateNormarized = NormarizedLayer.stateMachine.AddState("Normarized");
                    stateNormarized.writeDefaultValues = writeDefault;
                    stateNormarized.motion = animeNormarized;
                    animatorController.AddLayer(NormarizedLayer);
                    NormarizedLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
                    AssetDatabase.AddObjectToAsset(NormarizedLayer.stateMachine, controllerPath);
                    EditorUtility.SetDirty(stateNormarized);



                    //効果音レイヤー
                    ad.CreateSoundLayer(animatorController, writeDefault);

                    //コントローラー作成
                    foreach (string property in jsonObj["UIsetInfo"]["Property"])
                    {
                        animatorController.AddParameter(property + "Contact", AnimatorControllerParameterType.Bool);
                        animatorController.AddParameter(property + "Toggle", AnimatorControllerParameterType.Bool);
                        ad.CreateContactLayer(animatorController, property, writeDefault);
                        ad.CreateToggleLayer(animatorController, property, writeDefault);
                    }

                    foreach (string menuList in jsonObj["UIsetInfo"]["MenuList"])
                    {
                        animatorController.AddParameter(menuList + "Contact", AnimatorControllerParameterType.Bool);
                        animatorController.AddParameter(menuList + "Toggle", AnimatorControllerParameterType.Bool);
                        ad.CreateContactLayer(animatorController, menuList, writeDefault);
                        ad.CreateToggleLayer(animatorController, menuList, writeDefault);
                    }

                    foreach (JObject layarInfo in jsonObj["UIsetInfo"]["LayerList"])
                    {
                        string layarName = (string)layarInfo["LayerName"];

                        //トグル用レイヤーのとき
                        if ((string)layarInfo["Category"] == "Toggle")
                        {
                            for (int count = 1; count <= int.Parse((string)layarInfo["Count"]); count++)
                            {
                                animatorController.AddParameter(layarName + "Object" + count + "Contact", AnimatorControllerParameterType.Bool);
                                animatorController.AddParameter(layarName + "Object" + count + "Toggle", AnimatorControllerParameterType.Bool);
                                ad.CreateContactLayer(animatorController, layarName + "Object" + count, writeDefault);
                                ad.CreateToggleLayer(animatorController, layarName + "Object" + count, writeDefault);
                                ad.CreateObjectLayer(animatorController, layarName + "Object" + count, writeDefault);
                            }
                        }
                        //排他的レイヤーのとき
                        else
                        {
                            animatorController.AddParameter(layarName + "ObjectInt", AnimatorControllerParameterType.Int);
                            for (int count = 1; count <= int.Parse((string)layarInfo["Count"]); count++)
                            {
                                animatorController.AddParameter(layarName + "Object" + count + "Contact", AnimatorControllerParameterType.Bool);
                                ad.CreateContactLayerInt(animatorController, layarName, count, writeDefault);
                                ad.CreateToggleLayerInt(animatorController, layarName, count, writeDefault);
                                ad.CreateObjectLayerInt(animatorController, layarName, count, writeDefault);
                            }
                        }

                    }


                    //UIsetの各ボタンをアバター専用のマテリアルに変更
                    foreach (string property in jsonObj["UIsetButtonList"])
                    {
                        GameObject searchObject = avatarObject.transform.Find("UIset").gameObject;
                        GameObject mesh = or.FindGameObjectByName(searchObject, property + "Mesh");
                        Material setMaterial = AssetDatabase.LoadAssetAtPath(destinationPath + "/" + property + ".mat", typeof(Material)) as Material;
                        if (mesh != null)
                        {
                            mesh.GetComponent<SkinnedMeshRenderer>().material = setMaterial;
                        }
                        else
                        {
                            Debug.Log(property + "Meshは見つかりませんでした");
                        }


                    }


                    //MAMergeAnimatorの設定
                    ModularAvatarMergeAnimator MAMergeAnimator = prefabUIset.GetComponent<ModularAvatarMergeAnimator>();
                    MAMergeAnimator.animator = animatorController;
                    MAMergeAnimator.deleteAttachedAnimator = true;
                    MAMergeAnimator.pathMode = MergeAnimatorPathMode.Absolute;
                    MAMergeAnimator.matchAvatarWriteDefaults = false;
                    //MAParamatersの設定
                    ModularAvatarParameters MAMergeParameters = prefabUIset.GetComponent<ModularAvatarParameters>();
                    //構造体なのでforeachは不可
                    ParameterConfig tempParameters = MAMergeParameters.parameters[0];
                    for (int i = 0; i < MAMergeParameters.parameters.Count; i++)
                    {
                        if (MAMergeParameters.parameters[i].nameOrPrefix.Contains("Toggle"))
                        {
                            ParameterConfig tempParameter = MAMergeParameters.parameters[i];
                            tempParameter.syncType = ParameterSyncType.Bool;
                            MAMergeParameters.parameters[i] = tempParameter;
                        }
                        else if (MAMergeParameters.parameters[i].nameOrPrefix.Contains("Int"))
                        {
                            ParameterConfig tempParameter = MAMergeParameters.parameters[i];
                            tempParameter.syncType = ParameterSyncType.Int;
                            tempParameter.defaultValue = 1;
                            MAMergeParameters.parameters[i] = tempParameter;
                        }
                    }

                    //saveする
                    GameObject saveObject = avatarObject.transform.Find("UIset").gameObject;
                    PrefabUtility.SaveAsPrefabAsset(saveObject, avatarSettingInfoPath + "/" + avatarName + "/UIset.prefab");
                    AssetDatabase.Refresh();
                }
                EditorUtility.DisplayDialog("Success", avatarName + "にUIsetをセットしました", "閉じる");
            }
            //TODO エラー処理を細分化
            catch (Exception e)
            {
                Debug.Log(e);
                EditorUtility.DisplayDialog("Error", "エラーが発生しました。ご連絡頂ければ可能な限り対応致します", "戻る");
            }
        }
    }

}










