local M = class("AssetLoaderManager")
local AssetLoaderManagerIns = CSharp.AssetLoaderManagerIns
--[[]
    加载器加载与释放必须成对出现
    默认使用异步加载
    LUA方式的加载存在很大的性能问题,研究下可以了,不可大范围使用
]]

function M:ctor()
    self._binaryLoader = false
    self._bundlerLoader = false
end

function M:loadBytesAssetSync(path,onSuccess,onFailed)
    local loader = self:getOrCreateBinaryLoader()
    return loader:loadAssetSync(path,onSuccess,onFailed)
end

function M:loadBytesAssetAsync(path,onSuccess,onFailed)
    type = type or CSharp.Object
    local loader = self:getOrCreateBinaryLoader()
    return loader:loadAssetAsync(path,onSuccess,onFailed)
end

function M:loadBundleAssetSync(path,type,onSuccess,onFailed)
    type = type or CSharp.Object
    local loader = self:getOrCreateBundlerLoader()
    return loader:loadAssetSync(path,onSuccess,onFailed)
end

function M:loadBundleAssetAsync(path,type,onSuccess,onFailed)
    type = type or CSharp.Object
    local loader = self:getOrCreateBundlerLoader()
    return loader:loadAssetAsync(path,onSuccess,onFailed)
end
--

--
function M:getOrCreateBinaryLoader( ... )
    if not self._binaryLoader then
        self._binaryLoader = AssetBinaryLoader.new()
    end
    return self._binaryLoader
end

function M:getOrCreateBundlerLoader( ... )
    if not self._bundlerLoader then
        self._bundlerLoader = AssetBundleLoader.new()
    end
    return self._bundlerLoader
end

rawset(_G, "AssetLoaderManager", false)
AssetLoaderManager = M.new()
