local M = class("AssetLoaderResult")

function M:ctor()  
    self._warp = false

    self.data = false
    self.isDone = false
end

function M:retain()
    if self._warp then
        self._warp:retain()
    end
end

function M:release()
    if self._warp then
        self._warp:release()
    end
end

rawset(_G, "AssetLoaderResult", M)