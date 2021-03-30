--定时器实体
---@class TimerProfiler
local M = {
    enable = false,
    frameId = -1,
    cache = nil,
}

function M.setEnable(enable)
    M.enable = enable
end

--清除所有数据
function M.start(frameId)
    if not M.enable then
        return
    end

    M.frameId = frameId
    M.cache = {}
end

--记录：重复key会覆盖
function M.record(key, timeMs)
    if not M.enable then
        return
    end

    M.cache = M.cache or {}
    table.insert(M.cache, {key, timeMs})
end

--打印或者写入文件
function M.stop()
    if not M.enable then
        return
    end
end

function M.getDataList()
    return M.cache, M.frameId
end

rawset(_G, "TimerProfiler", M)
