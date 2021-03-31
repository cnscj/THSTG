local M = class("MVCManager")

function M:ctor()

end


rawset(_G, "MVCManager", M)
MVCManager = M.new()