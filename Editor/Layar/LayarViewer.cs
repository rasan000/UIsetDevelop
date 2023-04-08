
using System;
using System.IO;
using nadena.dev.modular_avatar.core;
using UIset.util;
using UIset.Animation;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Dynamics.Contact.Components;
using System.Collections.Generic;

namespace UIset.Layar
{

    class LayarViewer
    {
        /// <summary>
        ///     アニメーションコントローラーにセットされているアニメクリップを表示します
        /// </summary>
        /// <param name="animatorController"></param>
        /// <param name="layerCategory"></param>
        ///


        // gameObjectとlayanamenameを紐づけるためのリスト
        private Dictionary<string, GameObject> _gameObjectDict;

        public void SetDict(Dictionary<string, GameObject> gameObjectDict)
        {
            _gameObjectDict = gameObjectDict;
        }

        public void ShowLayerAnimations(AnimatorController animatorController, string layerCategory, GameObject avatarObject)
        {
            AnimationClip animeEmpty = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Empty.anim", typeof(AnimationClip)) as AnimationClip;
            ObjectReader or = new ObjectReader();


            AnimationCreator ac = new AnimationCreator();
            AnimationSetter aseter = new AnimationSetter();

            EditorGUI.indentLevel++;
            foreach (AnimatorControllerLayer layer in animatorController.layers)
            {
                //オブジェクト操作レイヤーのみを表示
                if (layer.name.Contains("Object") && layer.name.Contains(layerCategory))
                {
                    if ((!layer.name.Contains("Toggle")) && (!layer.name.Contains("Contact")) && (!layer.name.Contains("Int")))
                    {
                        //カテゴリ名
                        GameObject searchObject = avatarObject.transform.Find("UIset").gameObject;
                        GameObject mesh = or.FindGameObjectByName(searchObject, layer.name + "Mesh");
                        if (mesh != null)
                        {
                            //editorにボタンとして表示して、クリックしたらsceneを参照する
                            if (GUILayout.Button(layer.name, GUILayout.Width(200)))
                            {
                                Selection.activeGameObject = mesh;
                            }
                        }

                        //デフォルトONチェックボックス

                        ModularAvatarParameters MAMergeParameters = searchObject.GetComponent<ModularAvatarParameters>();
                        ParameterConfig tempParameters = MAMergeParameters.parameters[0];
                        for (int i = 0; i < MAMergeParameters.parameters.Count; i++)
                        {
                            if (MAMergeParameters.parameters[i].nameOrPrefix.Equals(layer.name + "Toggle"))
                            {
                                if (MAMergeParameters.parameters[i].defaultValue == 1.0f)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("デフォルトON", GUILayout.Width(150));
                                    bool tempParameter = GUILayout.Toggle(true, "");
                                    GUILayout.EndHorizontal();
                                    if (!tempParameter)
                                    {
                                        ParameterConfig tempParameterConfig = MAMergeParameters.parameters[i];
                                        tempParameterConfig.syncType = ParameterSyncType.Bool;
                                        tempParameterConfig.defaultValue = 0;
                                        MAMergeParameters.parameters[i] = tempParameterConfig;
                                    }
                                }
                                else
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("デフォルトON", GUILayout.Width(150));
                                    bool tempParameter = GUILayout.Toggle(false, "");
                                    GUILayout.EndHorizontal();
                                    if (tempParameter)
                                    {
                                        ParameterConfig tempParameterConfig = MAMergeParameters.parameters[i];
                                        tempParameterConfig.syncType = ParameterSyncType.Bool;
                                        tempParameterConfig.defaultValue = 1.0f;
                                        MAMergeParameters.parameters[i] = tempParameterConfig;
                                    }
                                }
                            }

                        }


                        //他のユーザーからの操作を許可するかチェックボックス
                        GameObject tempReceiverObject = or.FindGameObjectByName(searchObject, layer.name + "Receiver");
                        if (tempReceiverObject != null)
                        {
                            VRCContactReceiver contactReceiver = tempReceiverObject.GetComponent<VRCContactReceiver>();
                            if (contactReceiver.allowOthers == true)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("他のユーザーからの操作を許可する", GUILayout.Width(150));
                                bool tempParameter = GUILayout.Toggle(true, "");
                                GUILayout.EndHorizontal();
                                if (!tempParameter)
                                {
                                    contactReceiver.allowOthers = false;
                                }
                            }
                            else
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("他のユーザーからの操作を許可する", GUILayout.Width(150));
                                bool tempParameter = GUILayout.Toggle(false, "");
                                GUILayout.EndHorizontal();
                                if (tempParameter)
                                {
                                    contactReceiver.allowOthers = true;
                                }
                            }
                        }

                        //アバターチェンジ時に値を保持するか
                        for (int i = 0; i < MAMergeParameters.parameters.Count; i++)
                        {
                            if (MAMergeParameters.parameters[i].nameOrPrefix.Equals(layer.name + "Toggle"))
                            {
                                if (MAMergeParameters.parameters[i].saved)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("アバター変更時にリセットさせない", GUILayout.Width(200));
                                    bool tempParameter = GUILayout.Toggle(true, "");
                                    GUILayout.EndHorizontal();
                                    if (!tempParameter)
                                    {
                                        ParameterConfig tempParameterConfig = MAMergeParameters.parameters[i];
                                        tempParameterConfig.saved = false;
                                        MAMergeParameters.parameters[i] = tempParameterConfig;
                                    }
                                }
                                else
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("アバター変更時にリセットさせない", GUILayout.Width(200));
                                    bool tempParameter = GUILayout.Toggle(false, "");
                                    GUILayout.EndHorizontal();
                                    if (tempParameter)
                                    {
                                        ParameterConfig tempParameterConfig = MAMergeParameters.parameters[i];
                                        tempParameterConfig.saved = true;
                                        MAMergeParameters.parameters[i] = tempParameterConfig;
                                    }
                                }
                            }
                        }



                        // ステートマシン一覧を表示する
                        foreach (ChildAnimatorState state in layer.stateMachine.states)
                        {
                            if (state.state.name.Contains("ButtonOFF") || state.state.name.Contains("ButtonON"))
                            {

                                EditorGUILayout.BeginHorizontal();
                                //buttonONはONで赤、buttonOFFはOFFで青表示
                                if (state.state.name.Contains("ON"))
                                {
                                    GUIStyle coloredLabel = new GUIStyle(EditorStyles.label);
                                    coloredLabel.normal.textColor = Color.red;
                                    EditorGUILayout.LabelField("ON", coloredLabel, GUILayout.Width(200));
                                }
                                else
                                {
                                    GUIStyle coloredLabel = new GUIStyle(EditorStyles.label);
                                    coloredLabel.normal.textColor = Color.cyan;
                                    EditorGUILayout.LabelField("OFF", coloredLabel, GUILayout.Width(200));
                                }

                                AnimationClip oldClip = state.state.motion as AnimationClip;
                                AnimationClip newClip = (AnimationClip)EditorGUILayout.ObjectField("", oldClip, typeof(AnimationClip), false, GUILayout.Width(200));
                                if (newClip != oldClip)
                                {
                                    state.state.motion = newClip;
                                }
                                if (newClip == null)
                                {
                                    state.state.motion = animeEmpty;
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }

                        //マテリアルに設定されているテクスチャを表示するEditorGUILayout.EndVertical();
                        Material tempMaterial = AssetDatabase.LoadAssetAtPath("Assets/UIset/AvatarSettingInfo/" + avatarObject.name + "/Material/" + layer.name + ".mat", typeof(Material)) as Material;
                        if (tempMaterial != null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("テクスチャ", GUILayout.Width(200));
                            Texture texture = tempMaterial.mainTexture;
                            texture = (Texture)EditorGUILayout.ObjectField(texture, typeof(Texture), false, GUILayout.Width(128), GUILayout.Height(128));
                            EditorGUILayout.EndHorizontal();
                            if (tempMaterial != null && texture != tempMaterial.mainTexture)
                            {
                                tempMaterial.mainTexture = texture;
                            }
                        }
                        else
                        {
                            Debug.Log("マテリアルが見つかりませんでした");
                        }


                        //アニメーション作成フィールド
                        EditorGUILayout.BeginHorizontal();
                        if (_gameObjectDict.ContainsKey(layer.name))
                        {
                            EditorGUI.BeginChangeCheck();
                            GameObject tempGameObject = EditorGUILayout.ObjectField(_gameObjectDict[layer.name], typeof(GameObject), true, GUILayout.Width(200)) as GameObject;

                            if (EditorGUI.EndChangeCheck() && tempGameObject != null)
                            {
                                _gameObjectDict[layer.name] = tempGameObject;

                            }
                            Debug.Log(_gameObjectDict[layer.name]);
                        }
                        else

                        {
                            EditorGUI.BeginChangeCheck();
                            GameObject tempGameObject = EditorGUILayout.ObjectField(null, typeof(GameObject), true, GUILayout.Width(200)) as GameObject;

                            if (EditorGUI.EndChangeCheck() && tempGameObject != null)
                            {
                                _gameObjectDict.Add(layer.name, tempGameObject);
                                Debug.Log(_gameObjectDict[layer.name]);
                            }

                        }
                        //アニメーション作成ボタン
                        if (GUILayout.Button("アニメーション作成"))
                        {
                            //アニメーション作成
                            ac.CreateAnimation();
                        }
                        EditorGUILayout.EndHorizontal();






                        //write horizontal line
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                        //スペースを開ける
                        EditorGUILayout.Space(10);

                    }
                }
            }
            EditorGUI.indentLevel--;
        }

    }
}