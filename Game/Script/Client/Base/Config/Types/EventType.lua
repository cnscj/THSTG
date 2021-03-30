local _eventId = 1000000000
local function getEventUID()
    _eventId = _eventId + 1
    return _eventId
end

local M = {
    -- 测试
    TEST_1 = getEventUID(),
    TEST_2 = getEventUID(),
    TEST_3 = getEventUID(),
    
}

rawset(_G, "EventType", false)
EventType = M