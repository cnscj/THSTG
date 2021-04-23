local M = class("ResourceLoader")
local P_Resource = require("Config.Profile.P_Resource")
local KEY_PLATFORM = "{platform}"

function M:initialize()
    --TODO:
    --加载并设置manifest
end

function M:loadManifest(platform,onSuccess,onFailed)
    local pathPattern = P_Resource.manifestPattern
    local abPath = self:_normalizePath(pathPattern,{platform = platform})

    AssetLoader:loadAssetBundleAsync(abPath,onSuccess,onFailed)
end

function M:loadModel(id,onSuccess,onFailed)
    local pathPattern = P_Resource.resDict.modelFolder

    local abPath = self:_normalizePath(pathPattern.abPattern,{id = id})
    local resPath = self:_normalizePath(pathPattern.resPattern,{id = id})
    AssetLoader:loadAssetBundleResourceAsync(abPath,resPath,onSuccess,onFailed)
end

function M:loadEffect(id,onSuccess,onFailed)
    local pathPattern = P_Resource.resDict.effectFolder

    local abPath = self:_normalizePath(pathPattern.abPattern,{id = id})
    local resPath = self:_normalizePath(pathPattern.resPattern,{id = id})
    AssetLoader:loadAssetBundleResourceAsync(abPath,resPath,onSuccess,onFailed)
end

--
function M:_normalizePath(path,params)
    for k,v in pairs(params) do
        local formatKey = string.format("{%s}",k)
        path = string.gsub(path,formatKey,v)
    end 
    path = string.gsub(path,KEY_PLATFORM,"pc")
    path = string.lower(path)
end
rawset(_G, "ResourceLoader", false)
ResourceLoader = M.new()