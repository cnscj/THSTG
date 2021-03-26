local M = class("Component")


function M:_onClear()

end

rawset(_G, "Component", M)
Component = M