local M = class("AssetManager")

function M:loadAssetBundle(abPath,onSuccess,onFailed)

end

function M:loadAssetBundleAsset(abPath,resPath,onSuccess,onFailed)

end

rawset(_G, "AssetManager", false)
AssetManager = M.new()