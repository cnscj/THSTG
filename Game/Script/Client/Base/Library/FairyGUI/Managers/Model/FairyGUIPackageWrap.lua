local M = class("FairyGUIPackageWrap",RefObj)

function M:ctor()
    self.package = false 
end

rawset(_G, "FairyGUIPackageWrap", M)