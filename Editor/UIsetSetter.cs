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

/// <summary>
/// UIset用のアニメーターを、セットしたアバター用に新規作成します
/// </summary>
///

public class UIsetSetter : UnityEditor.EditorWindow
{
    //コントローラーとアニメーションを置くフォルダ
    private const string avatarSettingInfoPath = "Assets/UIset/AvatarSettingInfo";
    //コピー用マテリアル
    private const string currentMaterialFolder = "Assets/UIset/src/material/Material";

    //アバター
    private VRCAvatarDescriptor avatarDescriptor;
    private GameObject avatarObject;



    //アニメーション入れ替え用
    private AnimatorController _animatorController;

    //defaultON用
    private List<bool> _checkboxDefaultON = new List<bool>();

    ObjectReader or = new ObjectReader();

    LayarViewer lv = new LayarViewer();
    private object prefabUtility;


    // メニュー
    [MenuItem("UIset/UIsetEditor")]
    private static void ShowWindow()
    {
        UIsetSetter window = GetWindowWithRect<UIsetSetter>(new Rect(0, 0, 450, 700));
        window.Show();
    }


    //UIFXのセットアップ
    private void OnGUI()
    {
        avatarObject = EditorGUILayout.ObjectField("AvatarName", avatarObject, typeof(GameObject), true) as GameObject;

        //window表示
        EditorGUILayout.BeginVertical();
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
            string controllerPath = avatarSettingInfoPath + "/" + avatarName + "/" + avatarName + ".controller";


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
                EditorGUILayout.LabelField("1.以下のボタンを押して、各ボタンに登録したいアニメを設定してください", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                bool openEditorWindowButton = GUILayout.Button("-------編集ウィンドウを開く---------", GUILayout.Width(300), GUILayout.Height(50));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (openEditorWindowButton)
                {
                    //アニメーション設定用のウィンドウを開く
                    UIsetEditor ue = new UIsetEditor();
                    ue.SetData(avatarDescriptor, avatarObject);
                    ue.Show();
                }

                EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("2.以下のボタンを押してからメニューの位置を調整してください", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GameObject menuPointObject = or.FindGameObjectByName(avatarObject.transform.Find("UIset").gameObject, "MenuPoint--(メニューの位置が調整できます)--");
                if (GUILayout.Button("メニューの位置を調整する", GUILayout.Width(200), GUILayout.Height(30)))
                {
                    Selection.activeGameObject = menuPointObject;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                //指輪のポジション調整
                EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("3.以下のボタンを押してから指輪を左手人差し指の位置にセットしてください", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GameObject ringPointObject = or.FindGameObjectByName(avatarObject.transform.Find("UIset").gameObject, "RingPoint--(指輪の場所が調整できます)--");
                if (GUILayout.Button("指輪の位置を調整する", GUILayout.Width(200), GUILayout.Height(30)))
                {
                    Selection.activeGameObject = ringPointObject;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                //指輪の大きさ調整
                EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("4.指輪の大きさが合わない時は以下のボタンを押してからscaleの値を調整してください", EditorStyles.boldLabel);
                GameObject ringObject = or.FindGameObjectByName(avatarObject.transform.Find("UIset").gameObject, "指輪");
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("指輪の大きさを調整する", GUILayout.Width(200), GUILayout.Height(30)))
                {
                    Selection.activeGameObject = ringObject;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                //アップロード前に必要ではないものは非表示にする
                EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("5.最後に以下のボタンを押して設定を完了してください", EditorStyles.boldLabel);
                GameObject UIsetObject = avatarObject.transform.Find("UIset").gameObject;
                GameObject UIObject = or.FindGameObjectByName(UIsetObject, "UI");
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                //controllerがセットされてない場合は自動セット
                if (UIsetObject.GetComponent<ModularAvatarMergeAnimator>().animator == null)
                {
                    UIsetObject.GetComponent<ModularAvatarMergeAnimator>().animator = animatorController;
                }
                if (GUILayout.Button("UIsetの設定を完了する", GUILayout.Width(200), GUILayout.Height(30)))
                {
                    UIObject.SetActive(false);
                    UIsetObject.SetActive(true);
                    ringObject.SetActive(true);

                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                //再設定と保存ボタン
                EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("※以下のボタンから再設定と設定の保存ができます", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                //設定は上書き保存でprefabとして保持
                if (GUILayout.Button("設定を保存する", GUILayout.Width(200), GUILayout.Height(30)))
                {
                    GameObject saveObject = avatarObject.transform.Find("UIset").gameObject;
                    PrefabUtility.SaveAsPrefabAsset(saveObject, avatarSettingInfoPath + "/" + avatarName + "/UIset.prefab");
                    AssetDatabase.Refresh();
                    EditorUtility.DisplayDialog("Success", avatarName + "用の設定を保存しました", "戻る");
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                //再設定用にUIsetを表示する
                if (GUILayout.Button("再設定する", GUILayout.Width(200), GUILayout.Height(30)))
                {
                    UIObject.SetActive(true);
                    UIsetObject.SetActive(true);
                    ringObject.SetActive(true);
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                //削除ボタン
                EditorGUILayout.LabelField("---------------------------------------------------------------", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("設定を全て削除する", GUILayout.Width(200)))
                {
                    // ファイルは強制削除でSceaneの方も削除
                    if (EditorUtility.DisplayDialog("設定を削除しますか？", "この操作は取り消せません。本当に削除しますか？", "はい", "いいえ"))
                    {
                        Directory.Delete(avatarSettingInfoPath + "/" + avatarName, true);
                        DestroyImmediate(avatarObject.transform.Find("UIset").gameObject);
                        AssetDatabase.Refresh();
                        EditorUtility.DisplayDialog("Success", avatarName + "用の設定ファイルを削除しました", "戻る");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("キャンセル？", "削除を取消しました", "戻る");
                    }
                }
                GUI.backgroundColor = originalColor;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();




                EditorGUILayout.EndVertical();

            }

            else
            {
                //ここからコントローラー生成
                //ファイルがない場合はコントローラー生成ボタンを表示
                GUILayout.Label(avatarName + "用のファイルを作成します。CreateControllerボタンを押してください", EditorStyles.boldLabel);
                bool createControlerButton = GUILayout.Button("CreateController");
                GUILayout.BeginHorizontal();
                GUILayout.EndHorizontal();

                //アバターがセットされていない場合はエラーで終了
                if (avatarDescriptor == null && createControlerButton)
                {
                    EditorUtility.DisplayDialog("Error", "アバターがセットされていません", "戻る");
                    return;
                }

                //セットされていれば生成開始
                if (createControlerButton)
                {
                    //アバター用のUIsetがなければ作成
                    if (!File.Exists(avatarSettingInfoPath + "/" + avatarName + "/UIset.prefab"))
                    {
                        //アバター用のフォルダがなければ作成する
                        if (!Directory.Exists(avatarSettingInfoPath + "/" + avatarName))
                        {
                            Directory.CreateDirectory(avatarSettingInfoPath + "/" + avatarName);
                        }

                        AssetDatabase.CopyAsset("Assets/UIset/src/UIset.prefab", avatarSettingInfoPath + "/" + avatarName + "/UIset.prefab");
                    }
                    //UIsetをアバター直下にセット
                    GameObject prefabUIset = AssetDatabase.LoadAssetAtPath(avatarSettingInfoPath + "/" + avatarName + "/UIset.prefab", typeof(GameObject)) as GameObject;
                    prefabUIset = PrefabUtility.InstantiatePrefab(prefabUIset) as GameObject;
                    prefabUIset.name = "UIset";
                    prefabUIset.transform.SetParent(avatarObject.transform);
                    UIsetCreator uc = new UIsetCreator();
                    uc.CreateController(avatarDescriptor, avatarObject, prefabUIset);
                }
            }
        }
    }
}



