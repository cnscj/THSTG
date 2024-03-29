--[[
    这个文件是用来配置所有C#实例的获取和缓存，真正用到的时候才去获取，主要目的是减少require脚本所消耗的时间，把require操作变得平滑
    使用方法：直接取.操作，如 CSharp.LuaManagerInstance.Start()
    NOTE：
        1. 如果有重名的，自己用前缀或后缀区分，不要再包一层table，本来缓存下来就是为了省时间，包一层又多费了时间
        2. 部分类只是为了设置一些全局参数，可以在C#那边封装一些工具方法来设置，不需要直接调用对应的类去设置，除非是后期加的功能，前期功能比较明确的时候，尽量通过工具类去做
--]]


--命名空间
local UnityEngine = CS.UnityEngine
local FairyGUI = CS.FairyGUI
local XLua = CS.XLua

local XLibGame = CS.XLibGame
local SEGame = CS.SEGame
local ASGame = CS.ASGame
local THGame = CS.THGame
local STGGame = CS.STGGame

local Tweening = CS.DG.Tweening

local _getter = {
    --Unity
    Application = function() return UnityEngine.Application end,
    Object = function() return UnityEngine.Object end,
    GameObject = function() return UnityEngine.GameObject end,
    Debug = function() return UnityEngine.Debug end,
    Time = function() return UnityEngine.Time end,
    AssetBundle = function() return UnityEngine.AssetBundle end,

    --XLibrary
    MonoManagerIns = function() return XLibGame.MonoManager.GetInstance() end,
    TimerIns = function() return XLibGame.Timer.GetInstance() end,

    --AssetStudio
    AssetLoaderManagerIns = function() return ASGame.AssetLoaderManager.GetInstance() end,

    --THGame
    GRoot = function() return FairyGUI.GRoot end,
    Stage = function() return FairyGUI.Stage end,
    StageCamera = function() return FairyGUI.StageCamera end,
    BlurFilter = function() return FairyGUI.BlurFilter end,
    TypingEffect = function() return FairyGUI.TypingEffect end,
    FGUIUtil = function() return THGame.UI.FGUIUtil end,
    UIPackageManagerIns = function() return THGame.UI.UIPackageManager.GetInstance() end,

    --SEGame
    LuaBehaviour = function() return SEGame.LuaBehaviour end,
    LuaManagerIns = function() return SEGame.LuaManager.GetInstance() end,  

    --自定义
    GameEngineIns = function() return STGGame.GameEngine.GetInstance() end, 

    --
}



---@class CSharp
CSharp = setmetatable({}, {
    __index = function(t, k)
        local obj = rawget(t, k)
        if not obj then
            if _getter[k] then
                -- startProfile("accessing " .. k)
                obj = _getter[k]()
                rawset(t, k, obj)
                -- stopProfile()
            end
        end
        return obj
    end,
    __newindex = function(_, k, v)
        error("CSharp is a readonly table!")
    end,
})
