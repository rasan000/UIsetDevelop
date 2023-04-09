using System.IO;
using UIset.util;
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
        private VRCAvatarDescriptor avatarDescriptor;

        public void CreateAnimation(GameObject gameObject, GameObject avatarObject)
        {
            string saveAnimClipPath = avatarSettingInfoPath + "/" + avatarObject.name + "/AnimationClip";
            avatarDescriptor = avatarObject.GetComponent<VRCAvatarDescriptor>();

            //nullチェック
            if (gameObject == null || avatarObject == null || avatarDescriptor == null)
            {
                EditorUtility.DisplayDialog("Error", "オブジェクトをセットしてください", "戻る");
                return;
            }
            //フォルダがなければ作成
            if (!Directory.Exists(saveAnimClipPath))
            {
                Directory.CreateDirectory(saveAnimClipPath);
            }


            //アニメーション作成
            AnimationClip onClip = new AnimationClip();
            AnimationClip offClip = new AnimationClip();
            onClip.name = gameObject.name + "_ON";
            offClip.name = gameObject.name + "_OFF";
            if (!File.Exists(saveAnimClipPath + "/UIS" + onClip.name + ".anim"))
            {
                onClip.SetCurve(GetPath(avatarObject, gameObject), typeof(GameObject), "m_IsActive", AnimationCurve.Constant(0, 1, 1));

            }
            if (!File.Exists(saveAnimClipPath + "/UIS" + offClip.name + ".anim"))
            {
                offClip.SetCurve(GetPath(avatarObject, gameObject), typeof(GameObject), "m_IsActive", AnimationCurve.Constant(0, 1, 1));
            }
            AssetDatabase.CreateAsset(onClip, "Assets/" + onClip.name + ".anim");
            AssetDatabase.CreateAsset(offClip, "Assets/" + offClip.name + ".anim");
            AssetDatabase.Refresh();
            return;
        }


        /// <summary>
        ///アバターから、指定したオブジェクトまでのパスを取得
        /// </summary>
        /// <param name="rootAvatarObject"></param>
        /// <param name="targetObject"></param>
        /// <returns></returns>
        private string GetPath(GameObject rootObject, GameObject targetObject)
        {

            string path = targetObject.name;
            Transform parent = targetObject.transform.parent;
            Transform root = rootObject.transform;
            //親がアバターになるまでループ
            while (root != parent)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }
    }

}


