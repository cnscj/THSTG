---@class BaseCache
local M = class("Cache")

--
function M:ctor()

end

--清掉缓存,用于手动状态下
function M:clear()
    
end

rawset(_G, "Cache", M)
Cache = M