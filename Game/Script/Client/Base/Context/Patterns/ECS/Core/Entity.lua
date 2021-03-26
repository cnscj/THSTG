local M = class("Entity")

function M:ctor()
   
end

function M:addComponent(type,comp)

end

function M:removeComponent(type)

end

function M:getComponent(type)

end

function M:replaceComponent(type,comp)
    return true
end

rawset(_G, "Entity", M)
Entity = M