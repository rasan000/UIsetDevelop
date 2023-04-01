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

    // メニュー
    [MenuItem("UIset/UIsetEditor")]
    private static void ShowWindow()
    {
        UIsetSetter window = GetWindowWithRect<UIsetSetter>(new Rect(0, 0, 400, 550));
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
                EditorGUILayout.LabelField("1.各ボタンに登録したいアニメを設定してください", EditorStyles.boldLabel);
                if (GUILayout.Button("-------編集ウィンドウを開く---------", GUILayout.Width(300), GUILayout.Height(50)))
                {
                    //アニメーション設定用のウィンドウを開く
                    UIsetEditor ue = new UIsetEditor();
                    ue.SetData(avatarDescriptor, avatarObject);
                    ue.Show();
                }

                EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("2.以下のボタンを押してからメニューの位置を調整してください", EditorStyles.boldLabel);
                GameObject menuPointObject = or.FindGameObjectByName(avatarObject.transform.Find("UIset").gameObject, "MenuPoint--(メニューの位置が調整できます)--");
                if (GUILayout.Button("メニューの位置を調整する", GUILayout.Width(200)))
                {
                    Selection.activeGameObject = menuPointObject;
                }

                //指輪のポジション調整
                EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("3.以下のボタンを押してから指輪の位置を調整してください", EditorStyles.boldLabel);
                GameObject ringPointObject = or.FindGameObjectByName(avatarObject.transform.Find("UIset").gameObject, "RingPoint--(指輪の場所が調整できます)--");
                if (GUILayout.Button("指輪の位置を調整する", GUILayout.Width(200)))
                {
                    Selection.activeGameObject = ringPointObject;
                }

                //指輪の大きさ調整
                EditorGUILayout.LabelField("--------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("4.指輪の大きさが合わない時は以下のボタンを押してからscaleの値を調整してください", EditorStyles.boldLabel);
                GameObject ringObject = or.FindGameObjectByName(avatarObject.transform.Find("UIset").gameObject, "指輪");
                if (GUILayout.Button("指輪の大きさを調整する", GUILayout.Width(200)))
                {
                    Selection.activeGameObject = ringObject;
                }

                //アップロード前に必要ではないものは非表示にする
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

                //再設定用にUIsetを表示する
                if (GUILayout.Button("再設定する", GUILayout.Width(200)))
                {

                    UIObject.SetActive(true);
                    UIsetObject.SetActive(true);
                    ringObject.SetActive(true);
                }
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
                    Debug.LogErrorFormat("アバターをセットしてください");
                    return;
                }

                //セットされていれば生成開始
                if (createControlerButton)
                {
                    //UIsetをアバター直下にセット
                    GameObject prefabUIset = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/UIset.prefab", typeof(GameObject)) as GameObject;
                    prefabUIset = Instantiate(prefabUIset);
                    prefabUIset.name = "UIset";
                    prefabUIset.transform.SetParent(avatarObject.transform);
                    UIsetCreator uc = new UIsetCreator();
                    uc.CreateController(avatarDescriptor, avatarObject, prefabUIset);
                }
            }
        }
    }
}



