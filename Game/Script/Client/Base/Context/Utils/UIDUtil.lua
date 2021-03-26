---@class UIDUtil
local M = {}

local _uid = 0
local _eventId = 1000000000
local _timerId = 0

--获取全局唯一的id（一般可以用这个)
function M.getUID()
    _uid = _uid + 1
    return _uid
end

--事件名的唯一id
function M.getEventUID()
    _eventId = _eventId + 1
    return _eventId
end

function M.getTimerId()
    _timerId = _timerId + 1
    return _timerId
end

rawset(_G, "UIDUtil", M)
