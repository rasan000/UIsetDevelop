using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace UIset.Animation
{
    /// <summary>
    /// セットされたオブジェクトをONOFFするアニメーションを作成します
    /// </summary>


    class AnimationCreator
    {
        private const string avatarSettingInfoPath = "Assets/UIset/AvatarSettingInfo";

        public void CreateAnimation(GameObject gameObject, GameObject avatarObject, AnimatorController animatorController)
        {

            string saveAnimClipPath = avatarSettingInfoPath + "/" + avatarObject.name + "/AnimationClip";

            Debug.Log(saveAnimClipPath);

            //nullチェック
            if (gameObject == null || avatarObject == null || animatorController == null)
            {
                EditorUtility.DisplayDialog("Error", "オブジェクトをセットしてください", "戻る");
                return;
            }
            //フォルダがなければ作成
            if (!Directory.Exists(saveAnimClipPath))
            {
                Directory.CreateDirectory(saveAnimClipPath);
            }


            //ONアニメーション作成
            // Create ON animation
            AnimationClip onClip = new AnimationClip();
            onClip.name = gameObject.name + "_ON";
            onClip.SetCurve(gameObject.name, typeof(GameObject), "m_IsActive", AnimationCurve.Constant(0, 1, 1));

            // Create OFF animation
            AnimationClip offClip = new AnimationClip();
            offClip.name = gameObject.name + "_OFF";
            offClip.SetCurve(gameObject.name, typeof(GameObject), "m_IsActive", AnimationCurve.Constant(0, 1, 0));

            // Save animation clips to the assets folder
            AssetDatabase.CreateAsset(onClip, saveAnimClipPath + "/UIS" + onClip.name + ".anim");
            AssetDatabase.CreateAsset(offClip, saveAnimClipPath + "/UIS" + offClip.name + ".anim");


            return;
        }
    }
}


