

require("CSharp")

require("Test")
require("Agent")

function setBranch()

end

function update(dt)

end

function main()
    --设置分支版本,或者其他设置
    setBranch()


    --设置逻辑更新
    CSharp.LuaEngine:RegisterLuaUpdateListeners(update,nil)

    print("启动成功了!!!!!")
end

main()