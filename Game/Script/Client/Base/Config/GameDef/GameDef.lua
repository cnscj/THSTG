GameDef = setmetatable({}, { __index = function(t, k)
    local tb = require(string.format("Config.Define.GameDef.%s", k))
    rawset(t, k, tb)
    return tb
end })

package.loaded["GameDef"] = GameDef