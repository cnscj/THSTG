local M = class("AssetManager")
local AssetLoaderManager = CSharp.AssetLoaderManager

function M:ctor()

end

function M:loadAssetBundle(abPath,onSuccess,onFailed)
    
end

function M:loadAssetBundleResource(abPath,resPath,onSuccess,onFailed)
    
end

rawset(_G, "AssetManager", false)
AssetManager = M.new()
