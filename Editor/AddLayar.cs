using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace UIset.util
{
    class AddLayer
    {



        /// <summary>
        /// CoolTime用のレイヤーを作成します
        /// </summary>
        public void CreateSoundLayer(AnimatorController FXController, bool writeDefault)
        {
            //パラメータ追加
            FXController.AddParameter("CoolTimeClose", AnimatorControllerParameterType.Bool);
            FXController.AddParameter("CoolTimeOpen", AnimatorControllerParameterType.Bool);
            FXController.AddParameter("CoolTimeMiddleOpen", AnimatorControllerParameterType.Bool);
            FXController.AddParameter("CoolTimeMiddleClose", AnimatorControllerParameterType.Bool);
            FXController.AddParameter("CoolTimeLongOpen", AnimatorControllerParameterType.Bool);
            FXController.AddParameter("CoolTimeLongClose", AnimatorControllerParameterType.Bool);

            string[] nameList = new string[]{
                "Empty",
                "ALLON",
                "Cancel",
                "CancelMiddle",
                "CancelLong",
                "Select",
                "SelectMiddle",
                "SelectLong"
            };

            //アニメーション用変数
            AnimationClip animeEmpty = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Empty.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeALLON = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/ALLON.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeCancelLong = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/CancelLong.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeCancelMiddle = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/CancelMiddle.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeCancel = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Cancel.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeSelectLong = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/SelectLong.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeSelectMiddle = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/SelectMiddle.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeSelect = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Select.anim", typeof(AnimationClip)) as AnimationClip;


            AnimatorControllerLayer SoundLayer = new AnimatorControllerLayer
            {
                name = "SoundLayer",
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };
            FXController.AddLayer(SoundLayer);

            //ステート追加
            var stateEmpty = SoundLayer.stateMachine.AddState("Empty", new Vector3(300, 120, 0));
            stateEmpty.writeDefaultValues = writeDefault;
            stateEmpty.motion = animeEmpty;

            var stateALLON = SoundLayer.stateMachine.AddState("ALLON", new Vector3(300, 300, 0));
            stateALLON.writeDefaultValues = writeDefault;
            stateALLON.motion = animeALLON;

            var stateSelect = SoundLayer.stateMachine.AddState("Select", new Vector3(0, 250, 0));
            stateSelect.writeDefaultValues = writeDefault;
            stateSelect.motion = animeSelect;

            var stateSelectLong = SoundLayer.stateMachine.AddState("SelectLong", new Vector3(50, 400, 0));
            stateSelectLong.writeDefaultValues = writeDefault;
            stateSelectLong.motion = animeSelectLong;

            var stateSelectMiddle = SoundLayer.stateMachine.AddState("SelectMiddle", new Vector3(100, 550, 0));
            stateSelectMiddle.writeDefaultValues = writeDefault;
            stateSelectMiddle.motion = animeSelectMiddle;

            var stateCancel = SoundLayer.stateMachine.AddState("Cancel", new Vector3(600, 250, 0));
            stateCancel.writeDefaultValues = writeDefault;
            stateCancel.motion = animeCancel;

            var stateCancelLong = SoundLayer.stateMachine.AddState("CancelLong", new Vector3(500, 400, 0));
            stateCancelLong.writeDefaultValues = writeDefault;
            stateCancelLong.motion = animeCancelLong;

            var stateCancelMiddle = SoundLayer.stateMachine.AddState("CancelMiddle", new Vector3(400, 550, 0));
            stateCancelMiddle.writeDefaultValues = writeDefault;
            stateCancelMiddle.motion = animeCancelMiddle;

            //stateALLONにドライバパラメータ追加
            var driverStateALLON = stateALLON.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            driverStateALLON.parameters.Add(new VRC_AvatarParameterDriver.Parameter()
            {
                name = "CoolTimeOpen",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 0
            });
            driverStateALLON.parameters.Add(new VRC_AvatarParameterDriver.Parameter()
            {
                name = "CoolTimeClose",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 0
            });
            driverStateALLON.parameters.Add(new VRC_AvatarParameterDriver.Parameter()
            {
                name = "CoolTimeLongOpen",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 0
            });
            driverStateALLON.parameters.Add(new VRC_AvatarParameterDriver.Parameter()
            {
                name = "CoolTimeLongClose",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 0
            });
            driverStateALLON.parameters.Add(new VRC_AvatarParameterDriver.Parameter()
            {
                name = "CoolTimeMiddleOpen",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 0
            });
            driverStateALLON.parameters.Add(new VRC_AvatarParameterDriver.Parameter()
            {
                name = "CoolTimeMiddleClose",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 0
            });
            driverStateALLON.localOnly = true;


            //EmptyToCancel
            var transEmptyToCancel = stateEmpty.AddTransition(stateCancel);
            transEmptyToCancel.exitTime = 0;
            transEmptyToCancel.duration = 0;
            transEmptyToCancel.hasExitTime = false;
            transEmptyToCancel.AddCondition(AnimatorConditionMode.If, 1f, "CoolTimeClose");

            //EmptyToCancelMiddle
            var transEmptyToCancelMiddle = stateEmpty.AddTransition(stateCancelMiddle);
            transEmptyToCancelMiddle.exitTime = 0;
            transEmptyToCancelMiddle.duration = 0;
            transEmptyToCancelMiddle.hasExitTime = false;
            transEmptyToCancelMiddle.AddCondition(AnimatorConditionMode.If, 1f, "CoolTimeMiddleClose");

            //EmptyToCancelLong
            var transEmptyToCancelLong = stateEmpty.AddTransition(stateCancelLong);
            transEmptyToCancelLong.exitTime = 0;
            transEmptyToCancelLong.duration = 0;
            transEmptyToCancelLong.hasExitTime = false;
            transEmptyToCancelLong.AddCondition(AnimatorConditionMode.If, 1f, "CoolTimeLongClose");

            //EmptyToSelect
            var transEmptyToSelect = stateEmpty.AddTransition(stateSelect);
            transEmptyToSelect.exitTime = 0;
            transEmptyToSelect.duration = 0;
            transEmptyToSelect.hasExitTime = false;
            transEmptyToSelect.AddCondition(AnimatorConditionMode.If, 1f, "CoolTimeOpen");

            //EmptyToSelectMiddle
            var transEmptyToSelectMiddle = stateEmpty.AddTransition(stateSelectMiddle);
            transEmptyToSelectMiddle.exitTime = 0;
            transEmptyToSelectMiddle.duration = 0;
            transEmptyToSelectMiddle.hasExitTime = false;
            transEmptyToSelectMiddle.AddCondition(AnimatorConditionMode.If, 1f, "CoolTimeMiddleOpen");

            //EmptyToSelectLong
            var transEmptyToSelectLong = stateEmpty.AddTransition(stateSelectLong);
            transEmptyToSelectLong.exitTime = 0;
            transEmptyToSelectLong.duration = 0;
            transEmptyToSelectLong.hasExitTime = false;
            transEmptyToSelectLong.AddCondition(AnimatorConditionMode.If, 1f, "CoolTimeLongOpen");

            //CancelToALLON
            var transCancelToALLON = stateCancel.AddTransition(stateALLON);
            transCancelToALLON.exitTime = 1;
            transCancelToALLON.hasFixedDuration = true;
            transCancelToALLON.duration = 0;
            transCancelToALLON.offset = 0;

            //CancelMiddleToALLON
            var transCancelMiddleToALLON = stateCancelMiddle.AddTransition(stateALLON);
            transCancelMiddleToALLON.exitTime = 1;
            transCancelMiddleToALLON.hasFixedDuration = true;
            transCancelMiddleToALLON.duration = 0;
            transCancelMiddleToALLON.offset = 0;

            //CancelLongToALLON
            var transCancelLongToALLON = stateCancelLong.AddTransition(stateALLON);
            transCancelLongToALLON.exitTime = 1;
            transCancelLongToALLON.hasFixedDuration = true;
            transCancelLongToALLON.duration = 0;
            transCancelLongToALLON.offset = 0;

            //SelectToALLON
            var transSelectToALLON = stateSelect.AddTransition(stateALLON);
            transSelectToALLON.exitTime = 1;
            transSelectToALLON.hasFixedDuration = true;
            transSelectToALLON.duration = 0;
            transSelectToALLON.offset = 0;

            //SelectMiddleToALLON
            var transSelectMiddleToALLON = stateSelectMiddle.AddTransition(stateALLON);
            transSelectMiddleToALLON.exitTime = 1;
            transSelectMiddleToALLON.hasFixedDuration = true;
            transSelectMiddleToALLON.duration = 0;
            transSelectMiddleToALLON.offset = 0;

            //SelectLongToALLON
            var transSelectLongToALLON = stateSelectLong.AddTransition(stateALLON);
            transSelectLongToALLON.exitTime = 1;
            transSelectLongToALLON.hasFixedDuration = true;
            transSelectLongToALLON.duration = 0;
            transSelectLongToALLON.offset = 0;

            //ALLONtoEmpty
            var transALLONToEmpty = stateALLON.AddTransition(stateEmpty);
            transALLONToEmpty.exitTime = 1;
            transALLONToEmpty.hasFixedDuration = true;
            transALLONToEmpty.duration = 0;
            transALLONToEmpty.offset = 0;



        }

        /// <summary>
        ///     コンタクト用のレイヤーを作成します
        /// </summary>
        /// <param name="FXController"></param>
        /// <param name="process"></param>
        /// <param name="writeDefault"></param>
        public void CreateContactLayer(AnimatorController FXController, string process, bool writeDefault)
        {
            AnimatorControllerLayer contactLayer = new AnimatorControllerLayer
            {
                name = process + "Contact",
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };
            FXController.AddLayer(contactLayer);

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
        ///     排他的なコンタクト用のレイヤーを作成します
        /// </summary>
        /// <param name="FXController"></param>
        /// <param name="process"></param>
        /// <param name="writeDefault"></param>
        public void CreateContactLayerInt(AnimatorController FXController, string process, int count, bool writeDefault)
        {


            AnimatorControllerLayer contactLayer = new AnimatorControllerLayer
            {
                name = process + "Object" + count + "Contact",
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };
            FXController.AddLayer(contactLayer);

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
                name = process + "ObjectInt",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = count
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
                name = process + "ObjectInt",
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                value = 1
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
            transEmptyToON.AddCondition(AnimatorConditionMode.If, 1f, process + "Object" + count + "Contact");
            transEmptyToON.AddCondition(AnimatorConditionMode.NotEqual, count, process + "ObjectInt");

            var transEmptyToOFF = stateEmpty.AddTransition(stateContactOFF);
            transEmptyToOFF.exitTime = 0;
            transEmptyToOFF.duration = 0;
            transEmptyToOFF.hasExitTime = false;
            transEmptyToOFF.AddCondition(AnimatorConditionMode.If, 1f, process + "Object" + count + "Contact");
            transEmptyToOFF.AddCondition(AnimatorConditionMode.Equals, count, process + "ObjectInt");


            var transContactONToExit = stateContactON.AddExitTransition();
            transContactONToExit.exitTime = 0;
            transContactONToExit.duration = 0;
            transContactONToExit.hasExitTime = false;
            transContactONToExit.AddCondition(AnimatorConditionMode.IfNot, 1f, process + "Object" + count + "Contact");

            var transContactOFFToExit = stateContactOFF.AddExitTransition();
            transContactOFFToExit.exitTime = 0;
            transContactOFFToExit.duration = 0;
            transContactOFFToExit.hasExitTime = false;
            transContactOFFToExit.AddCondition(AnimatorConditionMode.IfNot, 1f, process + "Object" + count + "Contact");

        }


        /// <summary>
        ///  Toggle用のレイヤーを作成します
        /// </summary>
        /// <param name="FXController"></param>
        /// <param name="process"></param>
        /// <param name="writeDefault"></param>
        public void CreateToggleLayer(AnimatorController FXController, string process, bool writeDefault)
        {
            AnimatorControllerLayer toggleLayer = new AnimatorControllerLayer
            {
                name = process + "Toggle",
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };
            toggleLayer.stateMachine.exitPosition = new Vector3(1100, 120, 0);

            FXController.AddLayer(toggleLayer);

            //emptyアニメ
            AnimationClip animeEmpty = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Empty.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeButtonON = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/" + process + "ON.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeButtonOFF = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/" + process + "OFF.anim", typeof(AnimationClip)) as AnimationClip;


            //アニメーションステート[0]：Empty　追加
            var stateEmpty = toggleLayer.stateMachine.AddState("Empty", new Vector3(300, 120, 0));
            stateEmpty.motion = animeEmpty;
            stateEmpty.writeDefaultValues = writeDefault;
            //アニメーションステート[1]：buttonON　
            var stateButtonON = toggleLayer.stateMachine.AddState("ButtonON", new Vector3(800, 240, 0));
            stateButtonON.motion = animeButtonON;
            stateButtonON.writeDefaultValues = writeDefault;
            //アニメーションステート[2]：buttonOFF　
            var stateButtonOFF = toggleLayer.stateMachine.AddState("ButtonOFF", new Vector3(800, 0, 0));
            stateButtonOFF.motion = animeButtonOFF;
            stateButtonOFF.writeDefaultValues = writeDefault;

            //emptyからON
            var transEmptyToButtonON = stateEmpty.AddTransition(stateButtonON);
            transEmptyToButtonON.exitTime = 0;
            transEmptyToButtonON.duration = 0;
            transEmptyToButtonON.hasExitTime = false;
            transEmptyToButtonON.AddCondition(AnimatorConditionMode.If, 1f, process + "Toggle");

            //emptyからOFF
            var transEmptyToButtonOFF = stateEmpty.AddTransition(stateButtonOFF);
            transEmptyToButtonOFF.exitTime = 0;
            transEmptyToButtonOFF.duration = 0;
            transEmptyToButtonOFF.hasExitTime = false;
            transEmptyToButtonOFF.AddCondition(AnimatorConditionMode.IfNot, 1f, process + "Toggle");

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

        /// <summary>
        ///  排他的なToggle用のレイヤーを作成します
        /// </summary>
        /// <param name="FXController"></param>
        /// <param name="process"></param>
        /// <param name="writeDefault"></param>
        public void CreateToggleLayerInt(AnimatorController FXController, string process, int count, bool writeDefault)
        {
            AnimatorControllerLayer toggleLayer = new AnimatorControllerLayer
            {
                name = process + "Object" + count + "Toggle",
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };
            toggleLayer.stateMachine.exitPosition = new Vector3(1100, 120, 0);

            FXController.AddLayer(toggleLayer);

            //emptyアニメ
            AnimationClip animeEmpty = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Empty.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeButtonON = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/" + process + "ON.anim", typeof(AnimationClip)) as AnimationClip;
            AnimationClip animeButtonOFF = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/" + process + "OFF.anim", typeof(AnimationClip)) as AnimationClip;


            //アニメーションステート[0]：Empty　追加
            var stateEmpty = toggleLayer.stateMachine.AddState("Empty", new Vector3(300, 120, 0));
            stateEmpty.motion = animeEmpty;
            stateEmpty.writeDefaultValues = writeDefault;
            //アニメーションステート[1]：buttonON　
            var stateButtonON = toggleLayer.stateMachine.AddState("ButtonON", new Vector3(800, 240, 0));
            stateButtonON.motion = animeButtonON;
            stateButtonON.writeDefaultValues = writeDefault;
            //アニメーションステート[2]：buttonOFF　
            var stateButtonOFF = toggleLayer.stateMachine.AddState("ButtonOFF", new Vector3(800, 0, 0));
            stateButtonOFF.motion = animeButtonOFF;
            stateButtonOFF.writeDefaultValues = writeDefault;

            //emptyからON
            var transEmptyToButtonON = stateEmpty.AddTransition(stateButtonON);
            transEmptyToButtonON.exitTime = 0;
            transEmptyToButtonON.duration = 0;
            transEmptyToButtonON.hasExitTime = false;
            transEmptyToButtonON.AddCondition(AnimatorConditionMode.Equals, count, process + "ObjectInt");

            //emptyからOFF
            var transEmptyToButtonOFF = stateEmpty.AddTransition(stateButtonOFF);
            transEmptyToButtonOFF.exitTime = 0;
            transEmptyToButtonOFF.duration = 0;
            transEmptyToButtonOFF.hasExitTime = false;
            transEmptyToButtonOFF.AddCondition(AnimatorConditionMode.NotEqual, count, process + "ObjectInt");

            //ONからOFF
            var transButtonONToButtonOFF = stateButtonON.AddTransition(stateButtonOFF);
            transButtonONToButtonOFF.exitTime = 0;
            transButtonONToButtonOFF.duration = 0;
            transButtonONToButtonOFF.hasExitTime = false;
            transButtonONToButtonOFF.AddCondition(AnimatorConditionMode.NotEqual, count, process + "ObjectInt");

            //OFFからOFF
            var transButtonOFFToButtonON = stateButtonOFF.AddTransition(stateButtonON);
            transButtonOFFToButtonON.exitTime = 0;
            transButtonOFFToButtonON.duration = 0;
            transButtonOFFToButtonON.hasExitTime = false;
            transButtonOFFToButtonON.AddCondition(AnimatorConditionMode.Equals, count, process + "ObjectInt");

        }


        /// <summary>
        ///  オブジェクト用のレイヤーを作成します
        /// </summary>
        /// <param name="FXController"></param>
        /// <param name="process"></param>
        /// <param name="writeDefault"></param>
        public void CreateObjectLayer(AnimatorController FXController, string process, bool writeDefault)
        {
            AnimatorControllerLayer toggleLayer = new AnimatorControllerLayer
            {
                name = process,
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };
            toggleLayer.stateMachine.exitPosition = new Vector3(1100, 120, 0);

            FXController.AddLayer(toggleLayer);

            //emptyアニメ
            AnimationClip animeEmpty = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Empty.anim", typeof(AnimationClip)) as AnimationClip;

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
            stateButtonON.motion = animeEmpty;
            stateButtonON.writeDefaultValues = writeDefault;
            //アニメーションステート[4]：buttonOFF　
            var stateButtonOFF = toggleLayer.stateMachine.AddState("ButtonOFF", new Vector3(800, 0, 0));
            stateButtonOFF.motion = animeEmpty;
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

        /// <summary>
        ///  オブジェクト用のレイヤーを作成します
        /// </summary>
        /// <param name="FXController"></param>
        /// <param name="process"></param>
        /// <param name="writeDefault"></param>
        public void CreateObjectLayerInt(AnimatorController FXController, string process, int count, bool writeDefault)
        {


            AnimatorControllerLayer toggleLayer = new AnimatorControllerLayer
            {
                name = process + "Object" + count,
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };
            toggleLayer.stateMachine.exitPosition = new Vector3(1100, 120, 0);

            FXController.AddLayer(toggleLayer);

            //emptyアニメ
            AnimationClip animeEmpty = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Empty.anim", typeof(AnimationClip)) as AnimationClip;

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
            stateButtonON.motion = animeEmpty;
            stateButtonON.writeDefaultValues = writeDefault;
            //アニメーションステート[4]：buttonOFF　
            var stateButtonOFF = toggleLayer.stateMachine.AddState("ButtonOFF", new Vector3(800, 0, 0));
            stateButtonOFF.motion = animeEmpty;
            stateButtonOFF.writeDefaultValues = writeDefault;

            //defaultON
            var transEmptyToDefaultON = stateEmpty.AddTransition(stateDefaultON);
            transEmptyToDefaultON.exitTime = 0;
            transEmptyToDefaultON.duration = 0;
            transEmptyToDefaultON.hasExitTime = false;
            transEmptyToDefaultON.AddCondition(AnimatorConditionMode.Equals, count, process + "ObjectInt");
            Debug.Log(process + "ObjectInt");

            //defaultOFF
            var transEmptyToDefaultOFF = stateEmpty.AddTransition(stateDefaultOFF);
            transEmptyToDefaultOFF.exitTime = 0;
            transEmptyToDefaultOFF.duration = 0;
            transEmptyToDefaultOFF.hasExitTime = false;
            transEmptyToDefaultOFF.AddCondition(AnimatorConditionMode.NotEqual, count, process + "ObjectInt");

            //defaultOFFからON
            var transDefaultOFFToButtonON = stateDefaultOFF.AddTransition(stateButtonON);
            transDefaultOFFToButtonON.exitTime = 0;
            transDefaultOFFToButtonON.duration = 0;
            transDefaultOFFToButtonON.hasExitTime = false;
            transDefaultOFFToButtonON.AddCondition(AnimatorConditionMode.Equals, count, process + "ObjectInt");

            //defaultONからOFF
            var transDefaultONToButtonOFF = stateDefaultON.AddTransition(stateButtonOFF);
            transDefaultONToButtonOFF.exitTime = 0;
            transDefaultONToButtonOFF.duration = 0;
            transDefaultONToButtonOFF.hasExitTime = false;
            transDefaultONToButtonOFF.AddCondition(AnimatorConditionMode.NotEqual, count, process + "ObjectInt");

            //ONからOFF
            var transButtonONToButtonOFF = stateButtonON.AddTransition(stateButtonOFF);
            transButtonONToButtonOFF.exitTime = 0;
            transButtonONToButtonOFF.duration = 0;
            transButtonONToButtonOFF.hasExitTime = false;
            transButtonONToButtonOFF.AddCondition(AnimatorConditionMode.NotEqual, count, process + "ObjectInt");

            //OFFからOFF
            var transButtonOFFToButtonON = stateButtonOFF.AddTransition(stateButtonON);
            transButtonOFFToButtonON.exitTime = 0;
            transButtonOFFToButtonON.duration = 0;
            transButtonOFFToButtonON.hasExitTime = false;
            transButtonOFFToButtonON.AddCondition(AnimatorConditionMode.Equals, count, process + "ObjectInt");

        }



    }

}


