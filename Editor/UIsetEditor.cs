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
using System.Collections.Generic;
using VRC.SDK3.Dynamics.Contact.Components;

/// <summary>
/// UIset用のアニメーターを、セットしたアバター用に新規作成します
/// </summary>
///

[UnityEditor.InitializeOnLoad]
public class UIsetEditor : EditorWindow
{

    //コントローラーとアニメーションを置くフォルダ
    private const string avatarSettingInfoPath = "Assets/UIset/AvatarSettingInfo";
    //コピー用マテリアル
    private const string currentMaterialFolder = "Assets/UIset/src/material/Material";

    //アバター
    private VRCAvatarDescriptor avatarDescriptor;
    private GameObject avatarObject;

    //バグのもとなのでwriteDefaultはfalseで
    bool writeDefault = false;

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

    ObjectReader or = new ObjectReader();

    // メニュー
    [MenuItem("UIset/UIsetEditor")]
    private static void ShowWindow()
    {
        UIsetEditor window = GetWindowWithRect<UIsetEditor>(new Rect(0, 0, 480, 800));
        window.Show();
    }


    //UIFXのセットアップ
    private void OnGUI()
    {
        avatarObject = EditorGUILayout.ObjectField("AvatarName", avatarObject, typeof(GameObject), true) as GameObject;
        avatarDescriptor = new VRCAvatarDescriptor();

        //window表示
        GUILayout.Space(20);
        GUILayout.Label("設定したいアバターをセットしてください", EditorStyles.boldLabel);
        GUILayout.Space(20);


        //avatarがセットされたらavatarDisprictorを取得
        if (avatarObject != null)
        {
            avatarDescriptor = avatarObject.GetComponent<VRCAvatarDescriptor>();
            //アバター名
            string avatarName = avatarDescriptor.gameObject.name;
            //コントローラー名(~~~UIsetアバター名で)
            string controllerPath = avatarSettingInfoPath + "/" + avatarName + "/UIset" + avatarName + ".controller";


            //コントローラー生成済みであればConfig画面へ
            if (File.Exists(controllerPath) && (avatarObject.transform.Find("UIset")))
            {
                GUILayout.Label(avatarName + "用のファイルを読み込みました", EditorStyles.boldLabel);
                GUILayout.Space(20);
                GUILayout.Label("編集モード", EditorStyles.boldLabel);
                GUILayout.Label("以下の手順に沿って操作を行ってください", EditorStyles.boldLabel);


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
                    ShowLayerAnimations(animatorController, "Main", avatarName);
                }
                GUILayout.Space(20);

                //サブメニュ－１
                EditorGUILayout.TextField("※アバターから向かって上のメニューです。", EditorStyles.miniLabel);
                GUI.contentColor = Color.red;
                _toggleSub1Menu = EditorGUILayout.Foldout(_toggleSub1Menu, "Sub1Menu");
                GUI.contentColor = originalContentColor;
                if (_toggleSub1Menu)
                {
                    ShowLayerAnimations(animatorController, "Sub1", avatarName);
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
                //showLayerAnimations
                if (_toggleSub2Menu)
                {
                    ShowLayerAnimations(animatorController, "Sub2", avatarName);
                }
                GUILayout.Space(20);


                //サブメニュ－３
                EditorGUILayout.TextField("※アバターから向かって左のメニューです。", EditorStyles.miniLabel);
                GUI.contentColor = Color.green;
                _toggleSub3Menu = EditorGUILayout.Foldout(_toggleSub3Menu, "Sub3Menu");
                GUI.contentColor = originalContentColor;
                if (_toggleSub3Menu)
                {
                    ShowLayerAnimations(animatorController, "Sub3", avatarName);
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("2.以下のボタンを押してからメニューの位置を調整してください", EditorStyles.boldLabel);
                GameObject menuPointObject = or.FindGameObjectByName(avatarObject.transform.Find("UIset").gameObject, "MenuPoint--(メニューの位置が調整できます)--");
                if (GUILayout.Button("メニューの位置を調整する", GUILayout.Width(200)))
                {
                    Selection.activeGameObject = menuPointObject;
                }

                EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("3.以下のボタンを押してから指輪の位置を調整してください", EditorStyles.boldLabel);
                GameObject ringPointObject = or.FindGameObjectByName(avatarObject.transform.Find("UIset").gameObject, "RingPoint--(指輪の場所が調整できます)--");
                if (GUILayout.Button("指輪の位置を調整する", GUILayout.Width(200)))
                {
                    Selection.activeGameObject = ringPointObject;
                }
                EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("4.指輪の大きさが合わない時は以下のボタンを押してからscaleの値を調整してください", EditorStyles.boldLabel);
                GameObject ringObject = or.FindGameObjectByName(avatarObject.transform.Find("UIset").gameObject, "指輪");
                if (GUILayout.Button("指輪の大きさを調整する", GUILayout.Width(200)))
                {
                    Selection.activeGameObject = ringObject;
                }
                EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("5.最後に以下のボタンを押して設定を完了してください", EditorStyles.boldLabel);
                GameObject UIsetObject = avatarObject.transform.Find("UIset").gameObject;
                GameObject UIObject = or.FindGameObjectByName(UIsetObject, "UI");
                if (GUILayout.Button("UIsetの設定を完了する", GUILayout.Width(200)))
                {
                    UIObject.SetActive(false);
                    UIsetObject.SetActive(true);
                    ringObject.SetActive(true);

                }
                if (GUILayout.Button("再設定する", GUILayout.Width(200)))
                {

                    UIObject.SetActive(true);
                    UIsetObject.SetActive(true);
                    ringObject.SetActive(true);
                }
                EditorGUILayout.EndVertical();
                return;
            }



            //ここからコントローラー生成
            //ファイルがない場合はコントローラー生成ボタンを表示
            GUILayout.Label(avatarName + "用のファイルを作成します。CreateControllerボタンを押してください", EditorStyles.boldLabel);
            bool createUIFXButton = GUILayout.Button("CreateController");
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();

            //アバターがセットされていない場合はエラーで終了
            if (avatarDescriptor == null && createUIFXButton)
            {
                EditorUtility.DisplayDialog("Error", "アバターがセットされていません", "戻る");
                Debug.LogErrorFormat("アバターをセットしてください");
                return;
            }

            //セットされていれば生成開始
            if (createUIFXButton)
            {
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


                    //アバター用のコントローラー作成
                    AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);



                    //メニュー用のプロパティセット(Jsonで保存)
                    string json = JsonReader.ReadJson("Assets/UIset/Editor/UIsetInfo.json");
                    JObject jsonObj = JObject.Parse(json);
                    AddLayer ad = new AddLayer();


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


                    //UIsetをアバター直下にセット
                    GameObject prefabUIset = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/UIset.prefab", typeof(GameObject)) as GameObject;
                    prefabUIset = Instantiate(prefabUIset);
                    prefabUIset.name = "UIset";
                    prefabUIset.transform.SetParent(avatarObject.transform);

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



    /// <summary>
    ///     アニメーションコントローラーにセットされているアニメクリップを表示します
    /// </summary>
    /// <param name="animatorController"></param>
    /// <param name="layerCategory"></param>
    private void ShowLayerAnimations(AnimatorController animatorController, string layerCategory, string avatarName)
    {
        AnimationClip animeEmpty = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Empty.anim", typeof(AnimationClip)) as AnimationClip;

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
                    Material tempMaterial = AssetDatabase.LoadAssetAtPath("Assets/UIset/AvatarSettingInfo/" + avatarName + "/Material/" + layer.name + ".mat", typeof(Material)) as Material;
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





