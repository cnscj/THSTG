local M = class("ResourceLoader")
local P_Resource = require("Config.Profile.P_Resource")
local KEY_PLATFORM = "{platform}"

function M:initialize()
    --TODO:应该获取相对路径
    local abRootPath = "/Users/cnscj/UnityWorkspace/THSTG/Game/Resource/pc/"
    AssetLoaderManager:getOrCreateBundlerLoader():loadManifest(PathTool.combine(abRootPath,"pc"))
end


function M:loadModel(id,onSuccess,onFailed)
    local pathPattern = P_Resource.resDict.modelFolder

    local abPath = self:_normalizePath(pathPattern.abPattern,{id = id})
    local resPath = self:_normalizePath(pathPattern.resPattern,{id = id})
    local fullPath = string.format("%s|%s",abPath,resPath)
    AssetLoaderManager:loadBundleAssetAsync(fullPath,false,onSuccess,onFailed)
end

function M:loadEffect(id,onSuccess,onFailed)
    local pathPattern = P_Resource.resDict.effectFolder

    local abPath = self:_normalizePath(pathPattern.abPattern,{id = id})
    local resPath = self:_normalizePath(pathPattern.resPattern,{id = id})
    local fullPath = string.format("%s|%s",abPath,resPath)
    AssetLoaderManager:loadBundleAssetAsync(fullPath,false,onSuccess,onFailed)
end

--
function M:_normalizePath(path,params)
    path = self:_paresKeys(path,params)
    path = string.lower(path)

    return path
end

function M:_paresKeys( path ,params)
    for k,v in pairs(params) do
        local formatKey = string.format("{%s}",k)
        path = string.gsub(path,formatKey,v)
    end 
    path = string.gsub(path,KEY_PLATFORM,"pc")

    return path
end
rawset(_G, "ResourceLoader", false)
ResourceLoader = M.new()