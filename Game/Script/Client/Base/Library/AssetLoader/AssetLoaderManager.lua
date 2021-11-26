local M = class("AssetLoaderManager")
local AssetLoaderManagerIns = CSharp.AssetLoaderManagerIns
--[[]
    加载器加载与释放必须成对出现
    默认使用异步加载
]]

function M:ctor()
    self._bundlerLoader = false
end


function M:loadAssetSync(path,type)
    local loader = self:getOrCreateBundlerLoader()
    local obj = loader:loadAssetSync(path)
    return obj
end

function M:loadAssetAsync(path,type,onSuccess,onFailed)
    local loader = self:getOrCreateBundlerLoader()
    return loader:loadAssetAsync(path,onSuccess,onFailed)
end

--
function M:getOrCreateBundlerLoader( ... )
    if not self._bundlerLoader then
        self._bundlerLoader = AssetBundleLoader.new()
    end
    return self._bundlerLoader
end

rawset(_G, "AssetLoaderManager", false)
AssetLoaderManager = M.new()
