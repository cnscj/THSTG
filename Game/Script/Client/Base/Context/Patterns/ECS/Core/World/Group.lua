local M = class("Group")
--持有所有entity和system和负责收集对应的entity

function M:collect(matcher)

end

function M:getEntities()

end

rawset(_G, "Group", M)
Group = M