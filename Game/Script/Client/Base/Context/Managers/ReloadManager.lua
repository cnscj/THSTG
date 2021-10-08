local M = class("ReloadManager")

local P_Reload = require("Config.Profile.P_Reload")
local function isInExcludePattern(value)
    for _, pattern in ipairs(P_Reload.excludePattern) do
        if string.find(value, pattern) then
            return true
        end
    end
    return false
end

function M:ctor()

end

function M:reload()
    if not __DEBUG__ then return end

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

    --刷新View
    UIManager:reload()
    
    --刷新Manager

end

--
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