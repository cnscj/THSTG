local M = class("FairyGUIPackageWrap",RefObj)

function M:ctor()
    self.package = false 
    self.onUnwrap = false
end

function M:_onRelease()
    if self.onUnwrap then self.onUnwrap() end
end

rawset(_G, "FairyGUIPackageWrap", M)