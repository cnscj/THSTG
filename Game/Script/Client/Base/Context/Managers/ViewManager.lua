local M = class("ViewManager")
local _DICT = require("Config.Profile.P_View")

function M:ctor( ... )

end

function M:open(viewName)
    
end

function M:close(viewName)

end

function M:isOpened(viewName)

end
rawset(_G, "ViewManager", false)
ViewManager = M.new()