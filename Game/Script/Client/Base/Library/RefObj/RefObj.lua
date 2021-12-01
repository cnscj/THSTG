local M = class("RefObj")

function M:ctor()
    self._refCount = 0  --这里如果外部没主动引用都默认为不持有引用
end

function M:retain()
    self._refCount = self._refCount + 1
end

function M:release()
    self._refCount = self._refCount - 1
    if self._refCount == 0 then
        self:_onRelease()
    end
end

function M:refCount()
    return self._refCount
end

--下一帧自动移除
function M:releaseLate()
    RefObjManager:pushToReleaseList(self)
end
--[[由子类覆写]]
function M:_onRelease()

end

rawset(_G, "RefObj", M)