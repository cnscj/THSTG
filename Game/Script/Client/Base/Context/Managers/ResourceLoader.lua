local M = class("ResourceLoader")
local KEY_PLATFORM = "{platform}"

function M:normalizePath(path)
    path = string.gsub(path,KEY_PLATFORM,"pc")
    

end

function M:loadModel(id,onSuccess,onFailed)
    
end

function M:loadEffect(id,onSuccess,onFailed)
    
end

function M:_onSuccess(response)

end

function M:_onFailed(response)

end

rawset(_G, "ResourceLoader", false)
ResourceLoader = M.new()