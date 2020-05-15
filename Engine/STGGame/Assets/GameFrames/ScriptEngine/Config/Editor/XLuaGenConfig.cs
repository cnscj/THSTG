/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System.Collections.Generic;
using System;
using XLua;
using System.Reflection;
using System.Linq;


//配置的详细介绍请看Doc下《XLua的配置.doc》
public static class XLuaGenConfig
{
    /***************如果你全lua编程，可以参考这份自动化配置***************/
    static string[] customAssemblys = new string[] {
        "Assembly-CSharp",
    };

    //--------------begin 纯lua编程配置参考----------------------------
    // 屏蔽Unity的类和命名空间
    static List<string> exclude = new List<string> {
        "HideInInspector", "ExecuteInEditMode",
        "AddComponentMenu", "ContextMenu",
        "RequireComponent", "DisallowMultipleComponent",
        "SerializeField", "AssemblyIsEditorAssembly",
        "Attribute", "Types",
        "UnitySurrogateSelector", "TrackedReference",
        "TypeInferenceRules", "FFTWindow",
        "RPC", "Network", "MasterServer",
        "BitStream", "HostData",
        "ConnectionTesterStatus", "GUI", "EventType",
        "EventModifiers", "FontStyle", "TextAlignment",
        "TextEditor", "TextEditorDblClickSnapping",
        "TextGenerator", "TextClipping", "Gizmos",
        "ADBannerView", "ADInterstitialAd",
        "Android", "Tizen", "jvalue",
        "iPhone", "iOS", "Windows", "CalendarIdentifier",
        "CalendarUnit", "CalendarUnit",
        "ClusterInput", "FullScreenMovieControlMode",
        "FullScreenMovieScalingMode", "Handheld",
        "LocalNotification", "NotificationServices",
        "RemoteNotificationType", "RemoteNotification",
        "SamsungTV", "TextureCompressionQuality",
        "TouchScreenKeyboardType", "TouchScreenKeyboard",
        "MovieTexture", "UnityEngineInternal",
        "Terrain", "Tree", "SplatPrototype",
        "DetailPrototype", "DetailRenderMode",
        "MeshSubsetCombineUtility", "AOT", "Social", "Enumerator",
        "SendMouseEvents", "Cursor", "Flash", "ActionScript",
        "OnRequestRebuild", "Ping",
        "ShaderVariantCollection", "SimpleJson.Reflection",
        "CoroutineTween", "GraphicRebuildTracker",
        "Advertisements", "UnityEditor", "WSA",
        "EventProvider", "Apple",
        "ClusterInput", "Motion",
        "UnityEngine.UI.ReflectionMethodsCache", "NativeLeakDetection",
        "NativeLeakDetectionMode", "WWWAudioExtensions", "UnityEngine.Experimental",
            
		"UnityEngine.Purchasing",
		"UnityEngine.TestTools",
		"UnityEngine.tvOS",
		"UnityEngine.XR",
		"UnityEngine.Analytics",
		"UnityEngine.CanvasRenderer",
		"UnityEngine.LightProbeGroup",
        "UnityEngine.Rendering.PostProcessing",
        "UnityEngine.Tilemaps",
        "UnityEngine.UnityAnalyticsModule",
        "UnityEngine.RemoteSettings",
        "UnityEngine.RemoteConfigSettings",
        "UnityEngine.AI",
        "UnityEngine.Profiling",
    };

	static bool isExcluded(Type type)
    {
        var fullName = type.FullName;
        for (int i = 0; i < exclude.Count; i++)
        {
            if (fullName.Contains(exclude[i]))
            {
                return true;
            }
        }
        return false;
    }


    // 屏蔽自己的命名空间下面所有类
    static List<string> excludeSpaceName = new List<string>()
    {
        "UnityEditor",
        "XLua",
        "SevenZip",
        "ICSharpCode",
        "JetBrains",
        "SRF",
        "SRDebugger",
        "MikuLuaProfiler",
        "Poco",
        "TcpServer",
        "Bugly"
    };
    
    // 屏蔽指定类
    static List<string> excludeClassName = new List<string>()
    {
        "Assets.UWA.TypeHolder",
        "UWA.UWA_Launcher",
        "DG.Tweening.DOTweenModuleUtils",

        "ConsoleWindow",
        "XConsoleHelper",

    };

    static bool isExcludedCustom(Type type)
    {
        // 按命名空间过滤
        string nameSpace = type.Namespace;
        if (nameSpace != null )
        {
            for (int i = 0; i < excludeSpaceName.Count; i++)
            {
                if (nameSpace.StartsWith(excludeSpaceName[i]))
                {
                    return true;
                }
            }
        }

        // 按类名过滤
        string name = type.FullName;
        if (string.IsNullOrEmpty(name))
            name = type.Name;

        for (int i = 0; i < excludeClassName.Count; i++)
        {
            if (name == excludeClassName[i])
            {
                return true;
            }
        }

        return false;
    }

    // lua可以调用C#的类
    [LuaCallCSharp]
    public static IEnumerable<Type> LuaCallCSharp
    {
        get
        {
            var unityTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                              where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                              from type in assembly.GetExportedTypes()
                              where type.Namespace != null && type.Namespace.StartsWith("UnityEngine") && !isExcluded(type)
                                      && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
                              select type);            

            var customTypes = (from assembly in customAssemblys.Select(s => Assembly.Load(s))
                               from type in assembly.GetExportedTypes()
                               where !isExcludedCustom(type)
                                       && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
                               select type);
            return unityTypes.Concat(customTypes);
        }
    }


    // C#中可以用lua函数作为回调的
    [CSharpCallLua]
    public static List<Type> CSharpCallLua
    {
        get
        {
            //自动把LuaCallCSharp涉及到的delegate加到CSharpCallLua列表，后续可以直接用lua函数做callback
            var lua_call_csharp = LuaCallCSharp;

            var delegate_types = new List<Type>();
            var flag = BindingFlags.Public | BindingFlags.Instance
                | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly;

            foreach (var field in (from type in lua_call_csharp select type).SelectMany(type => type.GetFields(flag)))
            {
                if (typeof(Delegate).IsAssignableFrom(field.FieldType))
                {
                    delegate_types.Add(field.FieldType);
                }
            }

            foreach (var method in (from type in lua_call_csharp select type).SelectMany(type => type.GetMethods(flag)))
            {
                if (typeof(Delegate).IsAssignableFrom(method.ReturnType))
                {
                    delegate_types.Add(method.ReturnType);
                }
                foreach (var param in method.GetParameters())
                {
                    var paramType = param.ParameterType.IsByRef ? param.ParameterType.GetElementType() : param.ParameterType;
                    if (typeof(Delegate).IsAssignableFrom(paramType))
                    {
                        delegate_types.Add(paramType);
                    }
                }
            }
            // 过滤掉某些委托
            return delegate_types.Where(t => !isExcludedCustom(t) && !isExcludedDelegate(t)).Distinct().ToList();
        }
    }
    //--------------end 纯lua编程配置参考----------------------------


	//--------------begin 热补丁自动化配置-------------------------
	static bool hasGenericParameter(Type type)
    {
        //去掉匿名函数
        if(type.Name.Contains("f__AnonymousType"))
        {
            //Debug.Log("ignore type:"+type.Name);
            return true;
        }

        if (type.IsGenericTypeDefinition) return true;
        if (type.IsGenericParameter) return true;
        if (type.IsByRef || type.IsArray)
        {
            return hasGenericParameter(type.GetElementType());
        }
        if (type.IsGenericType)
        {
            foreach (var typeArg in type.GetGenericArguments())
            {
                if (hasGenericParameter(typeArg))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 类.委托名 中间不是“.”，而是"+"，只有命名空间下面的东西才是"."
    static List<string> excludeDelegates = new List<string>()
    {
        "XLua.ObjectCast",
        "XLua.ObjectCheck",
        "XLua.LuaEnv+CustomLoader",
        "System.Comparison`1[[Cinemachine.CinemachineClearShot+Pair, Cinemachine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]",
    };

    // 过滤掉某些不需要导出的delegate，即使所在类不导出，类里的委托也会自动导出，所以需要显示的过滤
    static bool isExcludedDelegate(Type t)
    {
        string fullName = t.FullName;
        if (string.IsNullOrEmpty(fullName))
            fullName = t.Name;
        for (int i = 0; i < excludeDelegates.Count; i++)
        {
            if (fullName == excludeDelegates[i])
            {
                return true;
            }
        }
        return false;
    }

    // C#中可以用lua函数作为回调的
    // 配置某Assembly下所有涉及到的delegate到CSharpCallLua下，Hotfix下拿不准那些delegate需要适配到lua function可以这么配置
    [CSharpCallLua]
    static IEnumerable<Type> AllDelegate
    {
        get
        {
            BindingFlags flag = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
            List<Type> allTypes = new List<Type>();

            foreach (var t in (from assembly in customAssemblys.Select(s => Assembly.Load(s))
                                from type in assembly.GetTypes()
                                select type))
            {
                var p = t;
                // while (p != null && !t.FullName.Contains("f__AnonymousType"))
                while (p != null)
                {
                    allTypes.Add(p);
                    p = p.BaseType;
                }
            }
            allTypes = allTypes.Distinct().ToList();
            var allMethods = from type in allTypes
                             from method in type.GetMethods(flag)
                             select method;
            var returnTypes = from method in allMethods
                              select method.ReturnType;
            var paramTypes = allMethods.SelectMany(m => m.GetParameters()).Select(pinfo => pinfo.ParameterType.IsByRef ? pinfo.ParameterType.GetElementType() : pinfo.ParameterType)
                                       .Where(t => !isExcludedCustom(t));
            var fieldTypes = from type in allTypes
                             from field in type.GetFields(flag)
                             select field.FieldType;

            // 过滤掉某些委托
            return (returnTypes.Concat(paramTypes).Concat(fieldTypes))
                        .Where(t => t.BaseType == typeof(MulticastDelegate) && !isExcludedCustom(t) && !isExcludedDelegate(t) && !hasGenericParameter(t))
                        .Distinct();
        }
    }
    //--------------end 热补丁自动化配置-------------------------


    /***************热补丁可以参考这份自动化配置***************/
    // 过滤掉某些不需要导出的delegate，即使所在类不导出，类里的委托也会自动导出，所以需要显示的过滤
    static bool isExcludedHotfix(Type type)
    {
        // 第3方库不需要hotfix
        if (type.Namespace != null && type.Namespace.StartsWith("Cinemachine"))
            return true;

        if (type.Name == "SdkAndroid"
          || type.Name == "SdkIOS"
          || type.Name == "SdkPC"
          || type.Name == "AndroidPlugin"
          || type.Name.StartsWith("UniWebView")
           )
        {
            return true;
        }

        return false;
    }

    [Hotfix]
    static IEnumerable<Type> HotfixInject
    {
        get
        {
            return (from assembly in customAssemblys.Select(s => Assembly.Load(s))
                    from type in assembly.GetExportedTypes()
                    where ( !isExcludedCustom(type) && !isExcludedHotfix(type) )
                    select type);
        }
    }
    /***************end 热补丁***************/

    //黑名单
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
        new List<string>(){"System.Xml.XmlNodeList", "ItemOf"},
        new List<string>(){"UnityEngine.WWW", "movie"},
#if UNITY_WEBGL
        new List<string>(){"UnityEngine.WWW", "threadPriority"},
#endif
        new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
        new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
        new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
        new List<string>(){"UnityEngine.Light", "areaSize"},
        new List<string>(){"UnityEngine.Light", "lightmapBakeType"},
        new List<string>(){"UnityEngine.WWW", "MovieTexture"},
        new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
        new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
#if !UNITY_WEBPLAYER
        new List<string>(){"UnityEngine.Application", "ExternalEval"},
#endif
        new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
        new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
        new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
        new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
        new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
        new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
        new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
        new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
        new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
 
        new List<string>(){"UnityEngine.AnimatorControllerParameter", "name"},
        new List<string>(){"UnityEngine.AudioSettings", "GetSpatializerPluginNames"},
        new List<string>(){"UnityEngine.AudioSettings", "SetSpatializerPluginName", "System.String"},
        new List<string>(){"UnityEngine.Caching", "SetNoBackupFlag", "System.String", "UnityEngine.Hash128"},
        new List<string>(){"UnityEngine.Caching", "SetNoBackupFlag", "UnityEngine.CachedAssetBundle"},
        new List<string>(){"UnityEngine.Caching", "ResetNoBackupFlag", "System.String", "UnityEngine.Hash128"},
        new List<string>(){"UnityEngine.Caching", "ResetNoBackupFlag", "UnityEngine.CachedAssetBundle"},
        new List<string>(){"UnityEngine.DrivenRectTransformTracker", "StopRecordingUndo"},
        new List<string>(){"UnityEngine.DrivenRectTransformTracker", "StartRecordingUndo"},
        new List<string>(){"UnityEngine.Input", "IsJoystickPreconfigured", "System.String"},

        new List<string>(){"UnityEngine.Light", "SetLightDirty"},
        new List<string>(){"UnityEngine.Light", "shadowRadius"},
        new List<string>(){"UnityEngine.Light", "shadowAngle"},
        new List<string>(){"UnityEngine.ParticleSystemForceField", "FindAll"},
        new List<string>(){"UnityEngine.Playables.PlayableGraph", "GetEditorName"},
        new List<string>(){"UnityEngine.QualitySettings", "streamingMipmapsRenderersPerFrame"},
        new List<string>(){"UnityEngine.Timeline.AnimationTrack", "CreateRecordableClip", "System.String"},
        new List<string>(){"UnityEngine.Timeline.AnimationPlayableAsset", "LiveLink"},
        new List<string>(){"UnityEngine.Texture", "imageContentsHash"},
        
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "GetEditorPreviewTile", "UnityEngine.Vector3Int"},
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "SetEditorPreviewTile", "UnityEngine.Vector3Int", "UnityEngine.Tilemaps.TileBase"},
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "HasEditorPreviewTile", "UnityEngine.Vector3Int"},
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "GetEditorPreviewTileFlags", "UnityEngine.Vector3Int"},
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "GetEditorPreviewSprite", "UnityEngine.Vector3Int"},
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "GetEditorPreviewTransformMatrix", "UnityEngine.Vector3Int"},
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "SetEditorPreviewTransformMatrix", "UnityEngine.Vector3Int", "UnityEngine.Matrix4x4"},
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "GetEditorPreviewColor", "UnityEngine.Vector3Int"},
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "SetEditorPreviewColor", "UnityEngine.Vector3Int", "UnityEngine.Color"},
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "EditorPreviewFloodFill", "UnityEngine.Vector3Int", "UnityEngine.Tilemaps.TileBase"},
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "EditorPreviewBoxFill", "UnityEngine.Vector3Int", "UnityEngine.Object", "System.Int32", "System.Int32", "System.Int32", "System.Int32"},
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "ClearAllEditorPreviewTiles"},                
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "editorPreviewOrigin"},
        new List<string>(){"UnityEngine.Tilemaps.Tilemap", "editorPreviewSize"},

        new List<string>(){"UnityEngine.UI.Graphic", "OnRebuildRequested"},
        new List<string>(){"UnityEngine.UI.Text", "OnRebuildRequested"},

        new List<string>() {"Cinemachine.CinemachineClearShot", "Randomize"},


//----------------------------------------
    };

#if UNITY_2018_1_OR_NEWER
    static List<string> DictFuncFilters = new List<string>()
    {
        "GetObjectData",
        "OnDeserialization",
    };

    static List<string> ListFuncFilters = new List<string>() 
    {
        "AsReadOnly",
        "AsReadOnly",
        "AddRange",
        "BinarySearch",
        "ConvertAll",
        "Exists",
        "Find",
        "FindAll",
        "FindIndex",
        "FindLast",
        "FindLastIndex",
        "InsertRange",
        "RemoveAll",
        "TrimExcess",
        "TrueForAll",
    };

    [BlackList]
    public static Func<MemberInfo, bool> MethodFilter = (memberInfo) =>
    {
        if (memberInfo.DeclaringType.IsGenericType)
        {
            if (memberInfo.DeclaringType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                if (memberInfo.MemberType == MemberTypes.Constructor)
                {
                    ConstructorInfo constructorInfo = memberInfo as ConstructorInfo;
                    var parameterInfos = constructorInfo.GetParameters();
                    if (parameterInfos.Length > 0)
                    {
                        // 最多只要一个参数的构造函数
                        if (parameterInfos.Length > 1)
                        {
                            return true;
                        }
                        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(parameterInfos[0].ParameterType))
                        {
                            return true;
                        }
                    }
                }
                else if (memberInfo.MemberType == MemberTypes.Method)
                {
                    var methodInfo = memberInfo as MethodInfo;
                    if (methodInfo.Name == "TryAdd" || methodInfo.Name == "Remove" && methodInfo.GetParameters().Length == 2)
                    {
                        return true;
                    }
                    else if (DictFuncFilters.Contains(methodInfo.Name))
                    {
                        return true;
                    }
                }
                else if (memberInfo.MemberType == MemberTypes.Property)
                {
                    if (memberInfo.Name == "Comparer")
                        return true;
                }
            }
            else if(memberInfo.DeclaringType.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (memberInfo.MemberType == MemberTypes.Constructor)
                {
                    ConstructorInfo constructorInfo = memberInfo as ConstructorInfo;
                    var parameterInfos = constructorInfo.GetParameters();

                    if (parameterInfos.Length > 0)
                    {
                        // 只需要没参数的
                        return true;
                    }
                }
                else if (memberInfo.MemberType == MemberTypes.Method)
                {
                    var methodInfo = memberInfo as MethodInfo;

                    if (ListFuncFilters.Contains(methodInfo.Name))
                    {
                        return true;
                    }
                    else if (methodInfo.Name == "Sort" && methodInfo.GetParameters().Length > 0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    };
#endif
}
