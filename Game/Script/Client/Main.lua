require("CSharp")
require("Agent")
require("Test")
local updateFunc = function (dt)
    GameManager:update(dt)
end

local errorFunc = function (...)
    print(debug.traceback())
end

function init()
    require("Version")

    --设置主干版本和分支版本
    CSharp.LuaManagerIns:SetLuaTrunkAndBranch(__TRUNK__,__BRANCH__)

    --设置游戏逻辑更新
    -- FIXME:注册操作应该放到CSharp,至少lua奔溃了引擎还能跑
    CSharp.LuaManagerIns:RegisterLuaUpdateListeners(update)
end

function setup()
    --游戏依赖加载
    require("3rdInit")
    require("ConfigInit")
    require("SystemInit")
    require("LibraryInit")

    --游戏环境
    require("ContextInit")
end

function start()
    --启动
    print(string.format("Engine Name:%s Engine Version:%s\nProject Name:%s Project Version:%s",__ENGINE_NAME__,__ENGINE_VERSION__,__PROJECT_NAME__,__SCRIPT_VERSION__))
    print(string.format("Launch Finish!"))

    GameManager:launch()
end

function main()
    init()

    setup()
    start()
end

function update(dt)
    xpcall(updateFunc,errorFunc,dt)
end

main()