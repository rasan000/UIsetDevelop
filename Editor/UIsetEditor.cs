using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using UIset.util;
//ModularAvatar用
using nadena.dev.modular_avatar.core;
using System.Collections.Generic;
using UIset.Layar;
using UIset;
namespace UIset
{

    /// <summary>
    /// UIset用のアニメーターを、セットしたアバター用に新規作成します
    /// </summary>
    ///

    /// <summary>
    /// UIset用のアニメーターを、セットしたアバター用に新規作成します
    /// </summary>
    ///

    class UIsetEditor : EditorWindow
    {
        //コントローラーとアニメーションを置くフォルダ
        private const string avatarSettingInfoPath = "Assets/UIset/AvatarSettingInfo";
        //コピー用マテリアル
        private const string currentMaterialFolder = "Assets/UIset/src/material/Material";

        //アバター
        private VRCAvatarDescriptor avatarDescriptor;
        private GameObject avatarObject;

        //スクロール用
        private Vector2 _scrollPosition = Vector2.zero;
        //トグルウィンドウ用
        private bool _toggleMainMenu = false;
        private bool _toggleSub1Menu = false;
        private bool _toggleSub2Menu = false;
        private bool _toggleSub3Menu = false;

        //アニメーション入れ替え用
        private AnimatorController _animatorController;

        //defaultON用
        private List<bool> _checkboxDefaultON = new List<bool>();



        // メニュー
        private static void ShowWindow()
        {
            UIsetEditor window = GetWindowWithRect<UIsetEditor>(new Rect(0, 0, 480, 1000));
        }


        //setter
        public void SetData(VRCAvatarDescriptor avatarDescriptor, GameObject avatarObject)
        {
            this.avatarObject = avatarObject;
            this.avatarDescriptor = avatarDescriptor;
        }



        //UIFXのセットアップ
        private void OnGUI()
        {
            string avatarName = avatarObject.name;
            ObjectReader or = new ObjectReader();
            LayarViewer lv = new LayarViewer();

            //コントローラー名(~~~UIsetアバター名で)
            string controllerPath = avatarSettingInfoPath + "/" + avatarName + "/" + avatarName + ".controller";

            EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);

            //アバター用のコントローラー取得
            AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (animatorController == null) { return; }


            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("1.ボタンに登録したいアニメを設定してください", EditorStyles.boldLabel);

            //スクロールウィンドウ
            //メインメニュー
            Color originalContentColor = GUI.contentColor;
            GUI.contentColor = Color.white;
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            EditorGUILayout.TextField("※アバター正面のメニューです。", EditorStyles.miniLabel);
            _toggleMainMenu = EditorGUILayout.Foldout(_toggleMainMenu, "MainMenu");
            GUI.contentColor = originalContentColor;
            if (_toggleMainMenu)
            {
                lv.ShowLayerAnimations(animatorController, "Main", avatarObject);
            }
            GUILayout.Space(20);
            //サブメニュ－１
            EditorGUILayout.TextField("※アバターから向かって上のメニューです。", EditorStyles.miniLabel);
            GUI.contentColor = Color.red;
            _toggleSub1Menu = EditorGUILayout.Foldout(_toggleSub1Menu, "Sub1Menu");
            GUI.contentColor = originalContentColor;
            if (_toggleSub1Menu)
            {
                lv.ShowLayerAnimations(animatorController, "Sub1", avatarObject);
            }
            GUILayout.Space(20);


            //サブメニュ－２
            EditorGUILayout.TextField("※アバターから向かって右のメニューです。このレイヤーはいずれか一つだけ選択されます", EditorStyles.miniLabel);
            GUI.contentColor = Color.cyan;
            _toggleSub2Menu = EditorGUILayout.Foldout(_toggleSub2Menu, "Sub2Menu");
            GUI.contentColor = originalContentColor;
            //defaultIntの設定
            ModularAvatarParameters MAMergeIntParameters = avatarObject.transform.Find("UIset").GetComponent<ModularAvatarParameters>();
            ParameterConfig tempParameters = MAMergeIntParameters.parameters[0];
            for (int i = 0; i < MAMergeIntParameters.parameters.Count; i++)
            {
                if (MAMergeIntParameters.parameters[i].nameOrPrefix.Equals("Sub2ObjectInt"))
                {
                    {
                        //初期選択値
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("初期選択ボタン", GUILayout.Width(150));
                        int tempIntValue = EditorGUILayout.IntField((int)MAMergeIntParameters.parameters[i].defaultValue, GUILayout.Width(100));
                        GUILayout.EndHorizontal();
                        if (tempIntValue <= 6 && tempIntValue >= 0)
                        {
                            if (tempIntValue != (int)MAMergeIntParameters.parameters[i].defaultValue)
                            {
                                ParameterConfig tempParameterConfig = MAMergeIntParameters.parameters[i];
                                tempParameterConfig.syncType = ParameterSyncType.Int;
                                tempParameterConfig.defaultValue = tempIntValue;
                                MAMergeIntParameters.parameters[i] = tempParameterConfig;
                            }
                        }
                        else
                        {
                            //アラート表示
                            EditorUtility.DisplayDialog("Error", "0～6の整数を入力してください", "戻る");
                            tempIntValue = 1;
                        }

                        //保存するか
                        if (MAMergeIntParameters.parameters[i].saved)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("アバター変更時にリセットさせない", GUILayout.Width(200));
                            bool tempIntSavedValue = GUILayout.Toggle(true, "");
                            GUILayout.EndHorizontal();
                            if (!tempIntSavedValue)
                            {
                                ParameterConfig tempParameterConfig = MAMergeIntParameters.parameters[i];
                                tempParameterConfig.saved = false;
                                MAMergeIntParameters.parameters[i] = tempParameterConfig;
                            }
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("アバター変更時にリセットさせない", GUILayout.Width(200));
                            bool tempIntSavedValue = GUILayout.Toggle(false, "");
                            GUILayout.EndHorizontal();
                            if (tempIntSavedValue)
                            {
                                ParameterConfig tempParameterConfig = MAMergeIntParameters.parameters[i];
                                tempParameterConfig.saved = true;
                                MAMergeIntParameters.parameters[i] = tempParameterConfig;
                            }
                        }
                    }
                }

            }
            //lv.ShowLayerAnimations
            if (_toggleSub2Menu)
            {
                lv.ShowLayerAnimations(animatorController, "Sub2", avatarObject);
            }
            GUILayout.Space(20);

            //サブメニュ－３
            EditorGUILayout.TextField("※アバターから向かって左のメニューです。", EditorStyles.miniLabel);
            GUI.contentColor = Color.green;
            _toggleSub3Menu = EditorGUILayout.Foldout(_toggleSub3Menu, "Sub3Menu");
            GUI.contentColor = originalContentColor;
            if (_toggleSub3Menu)
            {
                lv.ShowLayerAnimations(animatorController, "Sub3", avatarObject);
            }
            EditorGUILayout.EndScrollView();

            //ウィンドウを閉じるボタン
            if (GUILayout.Button("閉じる"))
            {
                this.Close();
            }

        }
    }
}

