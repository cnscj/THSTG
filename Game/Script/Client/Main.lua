

require("CSharp")

require("Test")
require("Agent")
require("Version")

function update(dt)

end

function main()
    --设置分支版本,或者其他设置


    --设置逻辑更新
    CSharp.LuaEngine:RegisterLuaUpdateListeners(update)

    print(string.format("EngineName:%s EngineVersion:%s",__ENGINE_NAME__,__ENGINE_VERSION__))
    print(string.format("ProjectName:%s ProjectVersion:%s",__PROJECT_NAME__,__SCRIPT_VERSION__))

end

main()