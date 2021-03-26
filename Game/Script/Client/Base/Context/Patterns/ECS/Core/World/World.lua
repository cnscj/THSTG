local M = class("World")
--持有所有entity和system和负责收集对应的entity

function M:getGroup(matcher)
    local group = Group.new()
    group:collect(matcher)
    return group
end

function M:update(dt)

end

rawset(_G, "World", M)
World = M