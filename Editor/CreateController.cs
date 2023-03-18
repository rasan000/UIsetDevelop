using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using HakoTools;
using Newtonsoft.Json.Linq;
//ModularAvatar用
using nadena.dev.modular_avatar.core;


/// <summary>
/// UIset用のアニメーターを、セットしたアバター用に新規作成します
/// </summary>
public class CreateController : EditorWindow
{


    //コントローラーとアニメーションを置くフォルダ
    private const string controllerFolderPath = "Assets/UIset/Animations";

    //アバター
    private VRCAvatarDescriptor avatarDescriptor;
    private GameObject avatarObject;
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
        avatarObject = EditorGUILayout.ObjectField("AvatarName", avatarObject, typeof(GameObject), true) as GameObject;

        avatarDescriptor = new VRCAvatarDescriptor();

        //window
        GUILayout.Space(20);
        GUILayout.Label("設定したいアバターをセットして、createボタンを押してください", EditorStyles.boldLabel);
        GUILayout.Space(20);
        bool CreateUIFXButton = GUILayout.Button("CreateController");
        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();

        //avatarがセットされていたらavatarDisprictorを取得
        if (avatarObject != null)
        {
            avatarDescriptor = avatarObject.GetComponent<VRCAvatarDescriptor>();
        }

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



                //効果音用
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
                EditorUtility.DisplayDialog("Success", avatarName + "用のコントローラーを作成しました", "次へ");

                //UIsetをアバター直下にセット
                GameObject prefabUIset = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/UIset.prefab", typeof(GameObject)) as GameObject;
                prefabUIset = Instantiate(prefabUIset);
                prefabUIset.name = "UIset";
                prefabUIset.transform.SetParent(avatarObject.transform);

                //MAMergeAnimatorの設定
                ModularAvatarMergeAnimator MAMergeAnimator = prefabUIset.GetComponent<ModularAvatarMergeAnimator>();
                MAMergeAnimator.animator = animatorController;
                MAMergeAnimator.deleteAttachedAnimator = true;
                MAMergeAnimator.pathMode = MergeAnimatorPathMode.Absolute;
                MAMergeAnimator.matchAvatarWriteDefaults = false;

                //MAParamatersの設定
                ModularAvatarParameters MAMergeParameters = prefabUIset.GetComponent<ModularAvatarParameters>();
                Debug.Log(MAMergeParameters.parameters[0].nameOrPrefix);

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
                EditorUtility.DisplayDialog("Error", "エラーが発生しました。お問合わせいただき、解決可能であれば早めに修正します。", "戻る");
            }




        }

    }
}





