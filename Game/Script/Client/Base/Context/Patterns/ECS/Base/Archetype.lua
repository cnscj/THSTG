local M = class("Archetype")
function M:ctor(bit)
   self._value = BitNum.new(bit)
end

function M:toString()
   self._value:toString()
end

function M:clone()
   local newArchetype = M.new()
   newArchetype._value = self._value:clone()

   return newArchetype
end

function M:toString()
   return self._value:toString()
end

function M:toHash()
   return self._value:toString()
end

function M:clear()
   self._value:clear()
end

return M