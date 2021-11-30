local M = class("AssetLoaderWrap")

function M:ctor( ... )
    self.asset = false
    self.onUnwrap = false 

    self._refCount = 0
end

function M:release()
    self._refCount = self._refCount - 1
    if self._refCount <= 0 then 
        self:_onUnwrap()
    end
end

function M:retain()
    self._refCount = self._refCount + 1
end

function M:_onUnwrap( ... )
    if self.onUnwrap then self.onUnwrap() end
end

rawset(_G, "AssetLoaderWrap", M)
