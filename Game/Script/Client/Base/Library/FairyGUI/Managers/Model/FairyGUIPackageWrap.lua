local M = class("FairyGUIPackageWrap",RefObj)

function M:ctor()
    self.package = false 
    self.stayTime = 5
    self.isResident = false     --是否为常驻包
    self.onUnwrap = false       --卸载函数

    self._unlockTick = false
end

function M:isStayTimeOut()
    --TODO:当引用计数为0时,如果超过这个时间,则
end

function M:_onRelease()
    if self.isResident then return end 

    if self.onUnwrap then self.onUnwrap() end
end

function M:destroy()
    while self:refCount() > 0 do 
        self:release()
    end
end

rawset(_G, "FairyGUIPackageWrap", M)