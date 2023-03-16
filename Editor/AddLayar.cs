using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace HakoTools
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

            //アニメーション用変数
            AnimationClip animeALLON = AssetDatabase.LoadAssetAtPath("Assets/UIset/src/Animation/Empty.anim", typeof(AnimationClip)) as AnimationClip;
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
            SoundLayer.stateMachine.AddState("ALLON");
            SoundLayer.stateMachine.AddState("Select");
            SoundLayer.stateMachine.AddState("SelectLong");
            SoundLayer.stateMachine.AddState("SelectMiddle");
            SoundLayer.stateMachine.AddState("Cancle");
            SoundLayer.stateMachine.AddState("CancleLong");
            SoundLayer.stateMachine.AddState("CancleMiddle");

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


