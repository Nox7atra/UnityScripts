using UnityEngine;
using System.Collections;
namespace VampLamp.EditorTools.ObstaclesTools
{
    public static class BuilderProperties
    {

        #region constants
        //Временный костыль, так как в спрайты запечены тени и при смене спрайта прийдётся вручную пересчитывать ширину тени
        public const float LINK_JOINT_ANCOR_X = 0;
        public const float LINK_JOINT_ANCOR_Y = 0.1f;
        public const float LINK_LENGHT = 0.35f;
        public const float MOUNT_JOINT_ANCOR_X = 0f;
        public const float MOUNT_JOINT_ANCOR_Y = 0.25f;
        public const float LOG_MOUNT_JOINT_ANCHOR_X = 0.04f;
        public const float LOG_MOUNT_JOINT_ANCHOR_Y = 0.35f;
        #endregion
        #region private properties
        private static GameObject _LogPrefab;
        private static GameObject _LogMountPrefab;
        private static GameObject _MountPrefab;
        private static GameObject _FirstLinkPrefab;
        private static GameObject _SecondLinkPrefab;
        private static GameObject _LampPrefab;
        private static GameObject _SwitcherPrefab;
        private static GameObject _WallSquarePrefab;
        private static GameObject _WallCirclePrefab;
        private static GameObject _WoodenBox;
        private static bool _IsInitialized = false;
        private static bool IsInitialized
        {
            get
            {
                _IsInitialized = _IsInitialized && _LogPrefab != null &&
                    _MountPrefab != null && _FirstLinkPrefab != null &&
                    _SecondLinkPrefab != null && _SwitcherPrefab != null
                    && _WallSquarePrefab != null && _WallCirclePrefab != null
                    && _WoodenBox != null;
                return _IsInitialized;
            }
        }
        #endregion
        #region public properties
        public static GameObject LogPrefab
        {
            get
            {
                Init();
                return _LogPrefab;
            }
        }
        public static GameObject LogMountPrefab
        {
            get
            {
                Init();
                return _LogMountPrefab;
            }
        }
        public static GameObject MountPrefab
        {
            get
            {
                Init();
                return _MountPrefab;
            }
        }
        public static GameObject FirstLinkPrefab
        {
            get
            {
                Init();
                return _FirstLinkPrefab;
            }
        }
        public static GameObject SecondLinkPrefab
        {
            get
            {
                Init();
                return _SecondLinkPrefab;
            }
        }
        public static GameObject LampPrefab
        {
            get
            {
                Init();
                return _LampPrefab;
            }
        }
        public static GameObject SwitcherPrefab
        {
            get
            {
                Init();
                return _SwitcherPrefab;
            }
        }
        public static GameObject WallCirclePrefab
        {
            get
            {
                Init();
                return _WallCirclePrefab;
            }
        }
        public static GameObject WallSquarePrefab
        {
            get
            {
                Init();
                return _WallSquarePrefab;
            }
        }
        public static GameObject WoodenBox
        {
            get
            {
                Init();
                return _WoodenBox;
            }
        }
        #endregion
        public static void Init()
        {
            if (IsInitialized)
            {
                return;
            }
            //Loading prefabs
            _LogPrefab        = Resources.Load(EditorPathConstants.LogPrefabPath)        as GameObject;
            _LogMountPrefab   = Resources.Load(EditorPathConstants.LogMountPrefabPath)   as GameObject;
            _MountPrefab      = Resources.Load(EditorPathConstants.MountPrefabPath)      as GameObject;
            _FirstLinkPrefab  = Resources.Load(EditorPathConstants.FirstLinkPrefabPath)  as GameObject;
            _SecondLinkPrefab = Resources.Load(EditorPathConstants.SecondLinkPrefabPath) as GameObject;
            _LampPrefab       = Resources.Load(EditorPathConstants.LampPrefabPath)       as GameObject;
            _SwitcherPrefab   = Resources.Load(EditorPathConstants.SwitcherPrefabPath)   as GameObject;
            _WallSquarePrefab = Resources.Load(EditorPathConstants.WallSquarePath)       as GameObject;
            _WallCirclePrefab = Resources.Load(EditorPathConstants.WallCirclePath)       as GameObject;
            _WoodenBox        = Resources.Load(EditorPathConstants.WoodenBoxPath)        as GameObject;
            _IsInitialized = true;
        }
    }
}