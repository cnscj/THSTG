
local M = class("GRichText", GLabel)
-- c# GRichTextField

function M:ctor(obj)
end

function M:init(obj)
end


function M:getRichTextField()
	return self._obj.richTextField
end

rawset(_G, "GRichText", M)
