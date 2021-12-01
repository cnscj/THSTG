local M = class("AssetLoaderWrap",RefObj)

function M:ctor( ... )
    self.asset = false
    self.onUnwrap = false 
end

function M:_onRelease()
    if self.onUnwrap then self.onUnwrap() end
end

rawset(_G, "AssetLoaderWrap", M)
