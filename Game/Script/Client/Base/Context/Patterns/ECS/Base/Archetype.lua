local M = class("Archetype",false,{
   empty = false
})
local ONLY_DICT = {}

function M:ctor(bit)
   self._value = BitNum.new(bit)
end

function M:add(archetype)
   if archetype then 
      self._value:add(archetype._value)
   end
   return self
end

function M:del(archetype)
   if archetype then 
      self._value:del(archetype._value)
   end
   return self
end

function M:equal(archetype)
   if not archetype then return false end 

   return self._value:isEqual(archetype._value)
end

function M:containAll(archetype)
   if not archetype then return false end 

   return self._value:isContainAll(archetype._value)
end

function M:containAny(archetype)
   if not archetype then return false end 

   return self._value:isContainAny(archetype._value)
end


function M:toString()
   return self._value:toString()
end

function M:toOnly()
   local str = self:toString()
   if not ONLY_DICT[str] then
      local cloneSelf = self:clone()
      cloneSelf:_setOnly(true)
      ONLY_DICT[str] = cloneSelf
   end
   return ONLY_DICT[str]
end

function M:clone()
   local newArchetype = M.new()
   newArchetype._value = self._value:clone()

   return newArchetype
end

function M:clear()
   self._value:clear()
end

function M:_setOnly(val)
   self._value.isReadOnly = val
end

M.empty = M.new()
M.empty:_setOnly(true)

rawset(_G, "Archetype", M)