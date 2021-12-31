local M = class("FairyGUIPackageWrap",RefObj)

function M:ctor()
    self.refHandlers = false
    self.package = false 
    self.stayTime = 0           --立马移除
    self.isResident = false     --是否为常驻包
    self.onUnwrap = false       --卸载函数

    self._unloadTick = false
end

function M:getName( ... )
    return self.package.name
end

function M:isStayTimeOut()
    --当引用计数为0时,如果超过这个时间,则
    if self._unloadTick then 
        if self.stayTime >= 0 then
            local curTimestamp = millisecondNow()
            if curTimestamp >= self._unloadTick + (self.stayTime * 1000) then
                return true
            end
        end
    end
end

function M:_onRelease()
    if self.isResident then return end 

    self._unloadTick = millisecondNow()
    if self.onUnwrap then self.onUnwrap(self) end
end

function M:destroy()
    if self.refHandlers then
        for _,handler in ipairs(self.refHandlers) do 
            if handler then
                handler:release()
            end
        end
    end
end

rawset(_G, "FairyGUIPackageWrap", M)