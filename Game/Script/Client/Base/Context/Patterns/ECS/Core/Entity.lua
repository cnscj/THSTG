local M = class("Entity")

function M:ctor()
    self._id = 0
    self._owner = false  --所属世界

    self._dirtyComps = {}
end

function M:addComponent(typeName)

end

function M:removeComponent(typeName)

end

function M:getComponent(typeName)

end

function M:replaceComponent(comp)
    return true
end


function M:clear()

end

function M:removeFromWorld()

end

rawset(_G, "Entity", false)
Entity = M