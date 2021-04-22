local M = class("ResourceLoader")
local P_Resource = require("Config.Profile.P_Resource")
local KEY_PLATFORM = "{platform}"

function M:loadModel(id,onSuccess,onFailed)
    
end

function M:loadEffect(id,onSuccess,onFailed)
    
end

function M:_onSuccess(response)

end

function M:_onFailed(response)

end
--
function M:_normalizePath(path)
    path = string.gsub(path,KEY_PLATFORM,"pc")

end
rawset(_G, "ResourceLoader", false)
ResourceLoader = M.new()