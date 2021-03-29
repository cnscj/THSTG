local M = class("Component")


function M:clear()

end

rawset(_G, "Component", false)
Component = M