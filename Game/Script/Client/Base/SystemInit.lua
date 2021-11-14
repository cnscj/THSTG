

--Lang相关
require("System.Lang.Core.Class")
require("System.Lang.Core.Functions")
require("System.Lang.Core.Coroutine")
require("System.Lang.Core.Delegate")
require("System.Lang.Core.String")
require("System.Lang.Core.Table")

require("System.Lang.Math.Math")
require("System.Lang.Math.Quaternion")
require("System.Lang.Math.Rect")
require("System.Lang.Math.Vector2")
require("System.Lang.Math.Vector3")
require("System.Lang.Math.Vector4")

require("System.Lang.Collection.Array")
require("System.Lang.Collection.Heap")
require("System.Lang.Collection.HeapHelper")
require("System.Lang.Collection.List")
require("System.Lang.Collection.Queue")
require("System.Lang.Collection.Set")
require("System.Lang.Collection.SimpleArray")
require("System.Lang.Collection.Stack")

require("System.Lang.Utility.Color")
require("System.Lang.Utility.Date")
require("System.Lang.Utility.Time")

--工具函数相关
require("System.Tool.TableTool")
require("System.Tool.ConfigOptimizer")
require("System.Tool.TimerProfiler")
require("System.Tool.TimeTool")

--Unity相关
-- require("System.Engine.Base.UnityComponent")
-- require("System.Engine.Base.UnityGameObject")
require("System.Engine.LuaBehaviour")
require("System.Engine.LuaNode")


--XLua相关
require("System.XLua.Debug")
Profiler = require("System.XLua.Profiler")
Util = require("System.XLua.Util")
-- require("System.XLua.Tdr")
require("System.XLua.Strict")   --这里开始不能在设置全局变量了
