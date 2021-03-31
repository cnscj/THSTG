local M = class("ResourceLoader")

function M:loadModel(id,onSuccess,onFailed)
    
end

function M:loadEffect(id,onSuccess,onFailed)
    
end

rawset(_G, "ResourceLoader", false)
ResourceLoader = M.new()