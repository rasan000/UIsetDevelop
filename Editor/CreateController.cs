using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using HakoTools;
using Newtonsoft.Json.Linq;

/// <summary>
/// UIset用のアニメーターを、セットしたアバター用に新規作成します
/// </summary>
public class CreateController : EditorWindow
{


    //コントローラーとアニメーションを置くフォルダ
    private const string controllerFolderPath = "Assets/UIset/Animations";

    private VRCAvatarDescriptor avatarDescriptor;
    public Vector2 Scroll = new Vector2(200, 200);

    //バグのもとなのでwriteDefaultはfalseで
    bool writeDefault = false;

    // メニュー
    [MenuItem("UIset/Create Controller")]

    private static void ShowWindow()
    {
        CreateController window = GetWindowWithRect<CreateController>(new Rect(0, 0, 480, 600));
        window.Show();
    }

    //UIFXのセットアップ
    private void OnGUI()
    {
        avatarDescriptor =
            (VRCAvatarDescriptor)EditorGUILayout.ObjectField("Avatar", avatarDescriptor, typeof(VRCAvatarDescriptor), true);

        //window
        GUILayout.Space(20);
        GUILayout.Label("設定したいアバターをセットして、createボタンを押してください", EditorStyles.boldLabel);
        GUILayout.Space(20);
        bool CreateUIFXButton = GUILayout.Button("CreateController");
        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();


        //アバターがセットされていない場合はエラーで終了
        if (avatarDescriptor == null && CreateUIFXButton)
        {
            EditorUtility.DisplayDialog("Error", "アバターがセットされていません", "戻る");
            Debug.LogErrorFormat("アバターをセットしてください");
            return;
        }

        //セットされていれば生成開始
        if (CreateUIFXButton)
        {
            try
            {
                //アバター名
                string avatarName = avatarDescriptor.gameObject.name;
                //コントローラー名(~~~UIsetアバター名で)
                string controllerPath = controllerFolderPath + "/" + avatarName + "/UIset" + avatarName + ".controller";


                //アバター用のフォルダがなければ作成する
                if (!Directory.Exists(controllerFolderPath + "/" + avatarName))
                {
                    Directory.CreateDirectory(controllerFolderPath + "/" + avatarName);
                    EditorUtility.DisplayDialog("Success", avatarName + "用のフォルダを作成しました", "戻る");
                }

                //アバター用のコントローラー作成
                AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

                //メニュー用のプロパティセット(Jsonで保存)
                string json = ReadJson.Read("Assets/UIset/Editor/UIsetInfo.json");
                JObject jsonObj = JObject.Parse(json);
                AddLayar ad = new AddLayar();

                //パラメータセット
                foreach (string property in jsonObj["UIsetInfo"]["Property"])
                {
                    animatorController.AddParameter(property + "Contact", AnimatorControllerParameterType.Bool);
                    animatorController.AddParameter(property + "Toggle", AnimatorControllerParameterType.Bool);
                    ad.CreateContactLayer(animatorController, property, writeDefault);
                }
                foreach (string menuList in jsonObj["UIsetInfo"]["MenuList"])
                {
                    Debug.Log(menuList);
                }
                foreach (JObject layarInfo in jsonObj["UIsetInfo"]["LayarList"])
                {
                    Debug.Log(layarInfo["LayarName"]);
                    Debug.Log(layarInfo["Category"]);
                    Debug.Log(layarInfo["Count"]);
                }






                EditorUtility.DisplayDialog("Success", avatarName + "用のコントローラーを作成しました", "戻る");

            }
            //TODO エラー処理を細分化
            catch (Exception e)
            {
                Debug.Log(e);
                EditorUtility.DisplayDialog("Error", "エラーが発生しました。お問合わせいただき、解決可能であれば早めに修正します。", "戻る");
            }




        }
    }





    /// <summary>
    ///    FXレイヤーへ各種レイヤーを追加します
    /// </summary>
    /// <param name="process">Main or Sub1 or Sub2 or Sub3 </param>
    /// <param name="FxLayer">アニメーションレイヤー</param>
    /// <param name="number">レイヤーに付与する識別No</param>
    /// <return>null</return>










    //トグルレイヤー作成用
    private AnimatorControllerLayer CreateToggleLayer(int number)
    {
        AnimatorControllerLayer toggleLayer = new AnimatorControllerLayer
        {
            name = "オブジェクト" + number + "トグル",
            defaultWeight = 1,
            stateMachine = new AnimatorStateMachine()
        };
        return toggleLayer;
    }
    //オブジェクトレイヤー作成用
    private AnimatorControllerLayer CreateObjectLayer(int number)
    {
        AnimatorControllerLayer objectLayer = new AnimatorControllerLayer
        {
            name = "オブジェクト" + number,
            defaultWeight = 1,
            stateMachine = new AnimatorStateMachine()
        };
        return objectLayer;
    }
}





