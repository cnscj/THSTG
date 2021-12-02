local M = class("RefObj")

function M:ctor()
    self._refCount = 1
end

function M:retain()
    self._refCount = self._refCount + 1
end

function M:weekRelease()
    self._refCount = self._refCount - 1
end

function M:release(isRelease)
    isRelease = (isRelease ~= false)
    self._refCount = self._refCount - 1
    if isRelease then
        if self._refCount <= 0 then
            self:_onRelease()
        end
    end
end

--下一帧自动移除
function M:releaseLate()
    RefObjManager:pushToReleaseList(self)
end

function M:refCount()
    return self._refCount
end


--[[由子类覆写]]
function M:_onRelease()

end

rawset(_G, "RefObj", M)