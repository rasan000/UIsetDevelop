using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace UIset.Animation
{

    class AnimationSetter
    {


        /// <summary>
        /// アニメーションを名前で探索し、存在する場合はanimClipを返します
        /// ON/OFFのいずれかを指定してください
        /// </summary>
        private const string avatarSettingInfoPath = "Assets/UIset/AvatarSettingInfo";
        public AnimationClip SetAnimation(GameObject gameObject, GameObject avatarObject, string toggle)
        {
            string saveAnimClipPath = avatarSettingInfoPath + "/" + avatarObject.name + "/AnimationClip";
            AnimationClip animClip = null;
            if (toggle == "ON")
            {
                animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(saveAnimClipPath + "/UIS" + gameObject.name + "_ON.anim");
            }
            else if (toggle == "OFF")
            {
                animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(saveAnimClipPath + "/UIS" + gameObject.name + "_OFF.anim");
            }
            Debug.Log(animClip);
            return animClip;
        }
    }
}


