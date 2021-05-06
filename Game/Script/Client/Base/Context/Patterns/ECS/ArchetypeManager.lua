local M = class("ArchetypeManager")

function M:ctor()
    --保存所有Archetype对应的Entity的集合
    self._archetypeDict = {}
end

function M:bindComponent(entity,component)
    --获取旧的archetype,添加之

end

function M:unbindComponent(entity,component)
    --获取旧的archetype,移除之

end

function M:getEntities(...)

end

--
rawset(_G, "ArchetypeManager", false)
