local M = simpleClass("ReloadManager")

local P_Reload = require("Config.Profile.P_Reload")
local function isInExcludePattern(value)
    if P_Reload.excludePattern and next(P_Reload.excludePattern) then
        for _, pattern in ipairs(P_Reload.excludePattern) do
            if string.find(value, pattern) then
                return true
            end
        end
    end
    return false
end

function M:ctor()

end

function M:reload()
    if not __DEBUG__ then return end

    --重载脚本文件
    self:reloadScript()

    --刷新View
    UIManager:reload()
    
    --刷新Manager
    EntityManager:reload()
end

--
function M:reloadScript()
    --刷新Main_Script
    for pack, data in pairs(package.loaded) do
        while true do
            for _, pattern in ipairs(P_Reload.includePattern) do
                if string.find(pack, pattern) and not isInExcludePattern(pack) then
                    self:_deal(pack, data)
                    break
                end
            end
            break
        end
    end
end

function M:_deal(pack, data)
    -- 刷新controller
    if type(data) == "table" and type(data.super) == "table" and data.super.cname == "Controller" then
        package.loaded[pack] = nil
        local newCls = require(pack)
        CacheControllerManager:reloadController(pack, newCls)
    
    -- 刷新Cache
    elseif type(data) == "table" and type(data.super) == "table" and data.super.cname == "Cache" then
        package.loaded[pack] = nil
        local newCls = require(pack)
        CacheControllerManager:reloadCache(pack, newCls)
    else
        package.loaded[pack] = nil 
        require(pack)
    end
end

rawset(_G, "ReloadManager", false)
ReloadManager = M.new()