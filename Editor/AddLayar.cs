using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace HakoTools
{
    class AddLayar
    {
        public void CreateContactLayer(AnimatorController FxLayer, string process, bool writeDefault)
        {
            AnimatorControllerLayer contactLayer = new AnimatorControllerLayer
            {
                name = process + "Contact",
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };
            FxLayer.AddLayer(contactLayer);

            //emptyアニメ
            AnimationClip animeEmpty = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Empty.anim", typeof(AnimationClip)) as AnimationClip;

            //アニメーションステート[0]：Empty　追加
            var stateEmpty = contactLayer.stateMachine.AddState("Empty", new Vector3(300, 120, 0));
            stateEmpty.motion = animeEmpty;
            stateEmpty.writeDefaultValues = writeDefault;
            //アニメーションステート[1]：contactON　
            var stateContactON = contactLayer.stateMachine.AddState("contactON", new Vector3(550, 240, 0));
            stateContactON.motion = animeEmpty;
            stateContactON.writeDefaultValues = writeDefault;
            //アニメーションステート[2]：contactOFF　
            var stateContactOFF = contactLayer.stateMachine.AddState("contactOFF", new Vector3(550, 0, 0));
            stateContactOFF.motion = animeEmpty;
            stateContactOFF.writeDefaultValues = writeDefault;

            var driverContactON = stateContactON.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            driverContactON.parameters.Add(new VRC_AvatarParameterDriver.Parameter()
            {
                name = process + "Object" + "Toggle",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 0
            });

            //コンタクトONにドライバパラメータ追加
            driverContactON.parameters.Add(new VRC_AvatarParameterDriver.Parameter()
            {
                name = "CoolTimeClose",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 1f
            });
            driverContactON.localOnly = true;

            //コンタクトOFFにドライバパラメータ追加
            var driverContactOFF = stateContactOFF.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            driverContactOFF.parameters.Add(new VRC_AvatarParameterDriver.Parameter()
            {
                name = process + "Toggle",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 1f
            });
            driverContactON.parameters.Add(new VRC_AvatarParameterDriver.Parameter()
            {
                name = "CoolTimeOpen",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 1f
            });
            driverContactOFF.localOnly = true;


            //遷移を追加
            var transEmptyToON = stateEmpty.AddTransition(stateContactON);
            transEmptyToON.hasExitTime = false;
            transEmptyToON.AddCondition(AnimatorConditionMode.If, 1f, process + "Contact");
            transEmptyToON.AddCondition(AnimatorConditionMode.If, 0, process + "Toggle");

            var transEmptyToOFF = stateEmpty.AddTransition(stateContactOFF);
            transEmptyToOFF.hasExitTime = false;
            transEmptyToOFF.AddCondition(AnimatorConditionMode.If, 1f, process + "Contact");
            transEmptyToOFF.AddCondition(AnimatorConditionMode.If, 1f, process + "Toggle");

            // Debug.Log(contactLayer.stateMachine);

            // var transContactONToExit = stateContactON.AddTransition(contactLayer.stateMachine);
            // transContactONToExit.hasExitTime = false;
            // transContactONToExit.AddCondition(AnimatorConditionMode.If, 0, process + "Object" + number + "Contact");

        }

    }

}