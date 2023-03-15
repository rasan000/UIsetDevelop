using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace HakoTools
{
    class AddLayar
    {

        /// <summary>
        /// CoolTime用のレイヤーを作成します
        /// </summary>
        public void CreateCoolTimeLayar()
        {


        }

        /// <summary>
        ///     コンタクト用のレイヤーを作成します
        /// </summary>
        /// <param name="FxLayer"></param>
        /// <param name="process"></param>
        /// <param name="writeDefault"></param>
        public void CreateContactLayer(AnimatorController FxLayer, string process, bool writeDefault)
        {
            AnimatorControllerLayer contactLayer = new AnimatorControllerLayer
            {
                name = process + "Contact",
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };
            FxLayer.AddLayer(contactLayer);

            //ボタン用アニメーション
            AnimationClip animeEmpty = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Empty.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeON = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/" + process + "ON.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeOFF = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/" + process + "OFF.anim", typeof(AnimationClip)) as AnimationClip;


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


            //コンタクトONにドライバパラメータ追加
            var driverContactON = stateContactON.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            driverContactON.parameters.Add(new VRC_AvatarParameterDriver.Parameter()
            {
                name = process + "Toggle",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 0
            });
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
            driverContactOFF.parameters.Add(new VRC_AvatarParameterDriver.Parameter()
            {
                name = "CoolTimeOpen",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 1f
            });
            driverContactOFF.localOnly = true;


            //遷移を追加
            var transEmptyToON = stateEmpty.AddTransition(stateContactON);
            transEmptyToON.exitTime = 0;
            transEmptyToON.duration = 0;
            transEmptyToON.hasExitTime = false;
            transEmptyToON.AddCondition(AnimatorConditionMode.If, 1f, process + "Contact");
            transEmptyToON.AddCondition(AnimatorConditionMode.IfNot, 1f, process + "Toggle");

            var transEmptyToOFF = stateEmpty.AddTransition(stateContactOFF);
            transEmptyToOFF.exitTime = 0;
            transEmptyToOFF.duration = 0;
            transEmptyToOFF.hasExitTime = false;
            transEmptyToOFF.AddCondition(AnimatorConditionMode.If, 1f, process + "Contact");
            transEmptyToOFF.AddCondition(AnimatorConditionMode.If, 1f, process + "Toggle");


            var transContactONToExit = stateContactON.AddExitTransition();
            transContactONToExit.exitTime = 0;
            transContactONToExit.duration = 0;
            transContactONToExit.hasExitTime = false;
            transContactONToExit.AddCondition(AnimatorConditionMode.IfNot, 1f, process + "Contact");

            var transContactOFFToExit = stateContactOFF.AddExitTransition();
            transContactOFFToExit.exitTime = 0;
            transContactOFFToExit.duration = 0;
            transContactOFFToExit.hasExitTime = false;
            transContactOFFToExit.AddCondition(AnimatorConditionMode.IfNot, 1f, process + "Contact");

        }

        /// <summary>
        ///  トグル用のレイヤーを作成します
        /// </summary>
        /// <param name="FxLayer"></param>
        /// <param name="process"></param>
        /// <param name="writeDefault"></param>
        public void CreateToggleLayer(AnimatorController FxLayer, string process, bool writeDefault)
        {
            AnimatorControllerLayer toggleLayer = new AnimatorControllerLayer
            {
                name = process + "Toggle",
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };
            toggleLayer.stateMachine.exitPosition = new Vector3(1100, 120, 0);

            FxLayer.AddLayer(toggleLayer);

            //emptyアニメ
            AnimationClip animeEmpty = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Empty.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeButtonON = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/" + process + "ON.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeButtonOFF = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/" + process + "OFF.anim", typeof(AnimationClip)) as AnimationClip;


            //アニメーションステート[0]：Empty　追加
            var stateEmpty = toggleLayer.stateMachine.AddState("Empty", new Vector3(300, 120, 0));
            stateEmpty.motion = animeEmpty;
            stateEmpty.writeDefaultValues = writeDefault;
            //アニメーションステート[1]：defaultON　
            var stateDefaultON = toggleLayer.stateMachine.AddState("defaultON", new Vector3(550, 240, 0));
            stateDefaultON.motion = animeEmpty;
            stateDefaultON.writeDefaultValues = writeDefault;
            //アニメーションステート[2]：defaultOFF　
            var stateDefaultOFF = toggleLayer.stateMachine.AddState("defaultOFF", new Vector3(550, 0, 0));
            stateDefaultOFF.motion = animeEmpty;
            stateDefaultOFF.writeDefaultValues = writeDefault;
            //アニメーションステート[3]：buttonON　
            var stateButtonON = toggleLayer.stateMachine.AddState("ButtonON", new Vector3(800, 240, 0));
            stateButtonON.motion = animeButtonON;
            stateButtonON.writeDefaultValues = writeDefault;
            //アニメーションステート[4]：buttonOFF　
            var stateButtonOFF = toggleLayer.stateMachine.AddState("ButtonOFF", new Vector3(800, 0, 0));
            stateButtonOFF.motion = animeButtonOFF;
            stateButtonOFF.writeDefaultValues = writeDefault;

            //defaultON
            var transEmptyToDefaultON = stateEmpty.AddTransition(stateDefaultON);
            transEmptyToDefaultON.exitTime = 0;
            transEmptyToDefaultON.duration = 0;
            transEmptyToDefaultON.hasExitTime = false;
            transEmptyToDefaultON.AddCondition(AnimatorConditionMode.If, 1f, process + "Toggle");

            //defaultOFF
            var transEmptyToDefaultOFF = stateEmpty.AddTransition(stateDefaultOFF);
            transEmptyToDefaultOFF.exitTime = 0;
            transEmptyToDefaultOFF.duration = 0;
            transEmptyToDefaultOFF.hasExitTime = false;
            transEmptyToDefaultOFF.AddCondition(AnimatorConditionMode.IfNot, 1f, process + "Toggle");

            //defaultOFFからON
            var transDefaultOFFToButtonON = stateDefaultOFF.AddTransition(stateButtonON);
            transDefaultOFFToButtonON.exitTime = 0;
            transDefaultOFFToButtonON.duration = 0;
            transDefaultOFFToButtonON.hasExitTime = false;
            transDefaultOFFToButtonON.AddCondition(AnimatorConditionMode.If, 1f, process + "Toggle");

            //defaultONからOFF
            var transDefaultONToButtonOFF = stateDefaultON.AddTransition(stateButtonOFF);
            transDefaultONToButtonOFF.exitTime = 0;
            transDefaultONToButtonOFF.duration = 0;
            transDefaultONToButtonOFF.hasExitTime = false;
            transDefaultONToButtonOFF.AddCondition(AnimatorConditionMode.IfNot, 1f, process + "Toggle");

            //ONからOFF
            var transButtonONToButtonOFF = stateButtonON.AddTransition(stateButtonOFF);
            transButtonONToButtonOFF.exitTime = 0;
            transButtonONToButtonOFF.duration = 0;
            transButtonONToButtonOFF.hasExitTime = false;
            transButtonONToButtonOFF.AddCondition(AnimatorConditionMode.IfNot, 1f, process + "Toggle");

            //OFFからOFF
            var transButtonOFFToButtonON = stateButtonOFF.AddTransition(stateButtonON);
            transButtonOFFToButtonON.exitTime = 0;
            transButtonOFFToButtonON.duration = 0;
            transButtonOFFToButtonON.hasExitTime = false;
            transButtonOFFToButtonON.AddCondition(AnimatorConditionMode.If, 1f, process + "Toggle");

        }

    }

}


