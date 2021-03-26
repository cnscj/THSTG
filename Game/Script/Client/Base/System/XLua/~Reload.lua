

-- 正在刷新，取消一些限制
__RELOADING_LUA__ = false

local dumpPath = "/Users/alonso/work/jz3d/Project/100/Client/Scripts/Base/System/Unity/Dump.lua"
local tracebackPath = "/Users/alonso/work/jz3d/Project/100/Client/Scripts/Base/System/Unity/traceback.lua"


function reloadView()
    -- local refreshPack = {
    --     -- "UI",
    --     "Test",
    --     "Mirror",
    --     "Team",
    --     "Marry",
    --     "ActivityRebate",
    -- }
    
    -- local refreshView = {
    --     "TestView",
    --     "PlayerView",
    --     "ArenaView",
    --     "TeamView",
    --     "AffectionView",
    -- }

    -- for k, v in ipairs(refreshPack) do
    --     UIPackageManager.removePackage(v)
    --     UIPackageManager.addPackage(v)
    -- end

    -- for k, v in ipairs(refreshView) do
    --     ViewManager.close(v)
    --     ViewManager.open(v)
    -- end

    ViewManager.closeAll(false, true)
    UIPackageManager.clearExcepPublicPackage()
    

end

function reloadLua()

    -- 调试模式才能刷
    if not __DEBUG__ then
        return
    end

    __RELOADING_LUA__ = true

    -- 自身文件 可以刷
    local debugPath = "System.Unity.Reload"

    -- 带字样
    local includePattern = {
        -- "Controller",
        -- "Util",
        -- "MsgManager",
        -- "Config", -- 太卡
        "Game.Modules",
        -- "Game.ConfigReaders", -- 太卡
        "Game.Utils",
        "Game.Manager",
        "Game.UI",
        "SkillConfig"
    }

    -- 多数manager不能刷，UIDUtil不能刷
    local excludePattern = {
        "Agent",
        "Dex",

        -- 部分manager
        "ViewManager",
        "UIPool",
        "CacheManager",
        "ControllerManager",
        "UIPackageManager",
        "PopUpManager",
        "RedDotManager",
        "SoundManager",
        "UpdateManager",
        "MapManager",
        "MapLoaderManager",

        -- 部分module
        "Game.Modules.Pack",
        "Game.Modules.Map.Entity",

        -- 部分util和cache
        "Game.Utils.Timer",
        "UIDUtil",
        "ResourceUtil",
        "ServerTimeCache",
        "ItemData",
        "Headwear",
        "MapConfig",
        "Alert",
    }
    

    local function isInExcludePattern(value)
        for _, pattern in ipairs(excludePattern) do
            if string.find(value, pattern) then
                return true
            end
        end
        return false
    end

    local a = 0

    local hasView = true
    local function deal( pack, data )
        local a = 0
        if pack ~= "Game.ConfigReaders.ConstVars" then
            -- return
        end
        -- 刷新controller
        if type(data) == "table" and type(data.super) == "table" and data.super.cname == "Controller" then
            package.loaded[pack] = nil
            local newCls = require(pack)
            ControllerManager.reloadController(pack, newCls)
            a = 1

        -- 刷新Cache
        elseif type(data) == "table" and type(data.super) == "table" and data.super.cname == "BaseCache" then
            package.loaded[pack] = nil
            local newCls = require(pack)
            Cache:reloadCache(pack, newCls)

            a = 2
        else
            -- local oldCls = package.loaded[pack]
            -- if type(oldCls) == "table" then
            --     package.loaded[pack] = nil
            --     local newCls = require(pack)
            --     for k, v in pairs(oldCls) do
            --         oldCls[k] = newCls[k]
            --     end
            --     for k, v in pairs(newCls) do
            --         oldCls[k] = newCls[k]
            --     end
            --     package.loaded[pack] = oldCls
            --     a = 3
            --     -- package.loaded[pack] = nil 
            --     -- require(pack)
            -- else
            --     package.loaded[pack] = nil 
            --     require(pack)
            --     a = 4
            -- end

            package.loaded[pack] = nil 
            require(pack)
            a = 5
        end

        --FileUtil.appendFileText(dumpPath, pack .. " " .. a .. "\n")
    end
    
    --if dumpPath and dumpPath ~= "" then
    --    FileUtil.writeFileText(dumpPath, "=====\n")
    --end
    for pack, data in pairs(package.loaded) do
        for i = 1, 1 do
            if pack == debugPath then
                deal(pack, data)
                __RELOADING_LUA__ = true
                break
            end

            for _, pattern in ipairs(includePattern) do
                if string.find(pack, pattern) and not isInExcludePattern(pack) then
                    deal(pack, data)
                    break
                end
            end
            break
        end
    end

    if hasView then
        ViewManager.reload()
    end
    
    World:reloadFile("Game.Modules.Map.Entity.Util.SkillEffectHandler")
    AI:clear()
    World:reloadFile("Game.Modules.MapAI.AI")
    AI:refreshHero()
    World:reload()
    UIWorld:reload()
    ClientWorld:reload()
    TurnBasedWorld:reload()
    SDK.init()
    __RELOADING_LUA__ = false
end

function reloadTraceback(str)
    if tracebackPath and tracebackPath ~= "" then
       FileUtil.writeFileText(tracebackPath, str)
    end
end
