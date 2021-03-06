

require("CSharp")
require("Agent")
require("Test")

function init()
    require("Version")

    --设置主干版本和分支版本
    CSharp.LuaEngine:SetLuaTrunkAndBrench(__TRUNK__,__BRANCH__)

    --设置游戏逻辑更新
    CSharp.LuaEngine:RegisterLuaUpdateListeners(update)
end
function setup()
    --游戏依赖加载
    require("3rdInit")
    require("SystemInit")
    require("LibraryInit")

    require("ContextInit")
end

function start()
    --启动
    print(string.format("Engine Name:%s Engine Version:%s\nProject Name:%s Project Version:%s",__ENGINE_NAME__,__ENGINE_VERSION__,__PROJECT_NAME__,__SCRIPT_VERSION__))
    print(string.format("Launch Finish!"))
end

function main()
    init()

    setup()
    start()
end

function update(dt)

end

main()