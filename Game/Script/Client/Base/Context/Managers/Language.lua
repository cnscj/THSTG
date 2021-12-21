local M = simpleClass("Language")

function M:ctor( ... )
    self._desc = false
end

function M:getString(id, ...)
    if not id then
        return ""
    end

    local format = self:_getDescTb()[id]  or ""
    local args = { ... }
    if #args > 0 then
        return string.format(str, ...)
    end

    return format
end

function M:_getDescTb()
    if not self._desc then 
        self._desc = require("Config.Handwork.H_Description")
    end
    return self._desc
end

rawset(_G, "Language", false)
Language = M.new()