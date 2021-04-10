
local t_ipairs = ipairs
local t_insert = table.insert
local t_remove = table.remove
local t_sort = table.sort

--用于权重排序
local function __sortListeners(a, b)
    if a.priority == b.priority then
        return a.index < b.index
    else
        return a.priority > b.priority
    end
end

local M = class("Dispatcher")

--判断侦听器是否存在
function M:__isListenerExist(name, listener)
    local eventT = self.__eventMap__[name]
    if eventT and eventT.__listeners__[listener] then
        return true
    end
    return false
end

--[[
添加事件侦听
@name           [string]事件名
@listener       [function]侦听器
@caller         [Object]侦听函数调用者
@priority       [int]权重，值越大越先被执行，为0时按添加的先后顺序执行(默认为0)
--]]
function M:__addEventListenerInner(name, listener, caller, priority)
    assert(type(name) == "string" or type(name) == "number", string.format("事件没有注册id === Invalid event name of argument 1 (%s), need a string or number!", name))
    assert(type(listener) == "function", "事件没有实现对应的方法 === Invalid listener function!")

    priority = priority or 0
    assert(type(priority) == "number", "Invalid priority value, need a int!")

    local eventT = self.__eventMap__[name]
    if eventT == nil then
        eventT = {
            __index__ = 0, --索引值，用于权重值相同的项能按添加顺序进行排序
            __listeners__ = {}, --用于快速判断某事件对应的侦听器是否存在
            __isLocked__ = false, --是否被锁住，用于防止在dispatch事件的过程中，对该事件的侦听器列表进行增减操作导致的异常
            __operations__ = nil    --被锁住时添加或删除的事件列表，与__isLocked__一起使用
        }
        self.__eventMap__[name] = eventT
    end

    if eventT.__isLocked__ then
        local operations = eventT.__operations__
        if not operations then
            operations = {}
            eventT.__operations__ = operations
        end
        if not operations.add then
            operations.add = {}
        end
        t_insert(operations.add, { name = name, listener = listener, caller = caller, priority = priority })
        return
    end

    local isExist = false
    local needSort = false

    for k, v in t_ipairs(eventT) do
        if v.listener == listener and v.caller == caller then
            isExist = true

            if v.priority ~= priority then
                v.priority = priority

                needSort = true
            end

            break
        end
    end

    if not isExist then
        eventT.__index__ = eventT.__index__ + 1
        eventT.__listeners__[listener] = true
        t_insert(eventT, { listener = listener, caller = caller, priority = priority, index = eventT.__index__ })
        needSort = true
    end

    if needSort and #eventT > 1 then
        t_sort(eventT, __sortListeners)
    end
end



--[[
移除事件侦听
@name           [string]    事件名
@listener       [function]  侦听器
@caller         [Object]    侦听函数调用者，为空时会删除所有侦听函数
--]]
function M:__removeEventListenerInner(name, listener, caller)
    assert(type(name) == "string" or type(name) == "number", "Invalid event name of argument 1, need a string or number!")
    assert(type(listener) == "function", "Invalid listener function!")

    local eventT = self.__eventMap__[name]
    if eventT ~= nil then
        if eventT.__isLocked__ then
            local operations = eventT.__operations__
            if not operations then
                operations = {}
                eventT.__operations__ = operations
            end
            if not operations.remove then
                operations.remove = {}
            end

            local opListeners = operations.remove[listener]
            if not opListeners then
                opListeners = {}
                operations.remove[listener] = opListeners
            end
            if caller then
                opListeners[caller] = true
            end

            t_insert(operations.remove, { name = name, listener = listener, caller = caller })
            return
        end

        local clearListener = true
        for i = #eventT, 1, -1 do
            if eventT[i].listener == listener then
                if not caller or eventT[i].caller == caller then
                    t_remove(eventT, i)
                else
                    clearListener = false
                end
            end
        end
        if clearListener then
            eventT.__listeners__[listener] = nil
        end
    end
end

-----------------------------------------------------------
function M:ctor()
    self.__eventMap__ = {}
end

--清空所有事件侦听数据
function M:clear()
    self.__eventMap__ = {}
end

function M:addEventListener(name, listenerOrCaller, caller, priority)
    if type(listenerOrCaller) == "table" then
        -- assert(EventType[listener],"必须在EventType中定义")
        self:__addEventListenerInner(name, listenerOrCaller[name], listenerOrCaller, priority)
    else
        self:__addEventListenerInner(name, listenerOrCaller, caller, priority)
    end
end

function M:removeEventListener(name, listenerOrCaller, caller)
    if type(listenerOrCaller) == "table" then
        self:__removeEventListenerInner(name, listenerOrCaller[name], listenerOrCaller)
    else
        self:__removeEventListenerInner(name, listenerOrCaller, caller)
    end
end

--[[
发布事件
@name		[string]事件名
@...		其它参数
--]]
function M:dispatchEvent(name, ...)
    if type(name) ~= "string" and type(name) ~= "number" then
        printTraceback()
        assert(type(name) == "string" or type(name) == "number", "Invalid event name of argument 1, need a string or number!")
    end

    local eventT = self.__eventMap__[name]
    if eventT ~= nil then
        eventT.__isLocked__ = true
        for k, v in t_ipairs(eventT) do            
            if eventT.__listeners__[v.listener] then
                -- 处理事件发布过程中移除了该事件的部分回调
                local needCall = true
                -- 这里得每次循环取一次，因为在某次回调中可能让其有值了
                local operations = eventT.__operations__
                if operations and operations.remove then
                    local opListeners = operations.remove[v.listener]
                    if opListeners and ( 
                        not v.caller
                        or opListeners[v.caller] 
                    ) then
                        needCall = false
                    end
                end
                if needCall then
                    v.listener(v.caller, name, ...)
                end
            end
        end
        eventT.__isLocked__ = false

        -- 得重新获取一次
        local operations = eventT.__operations__
        if operations then
            -- 增加
            if operations.add then
                for k, v in t_ipairs(operations.add) do
                    self:addEventListener(v.name, v.listener, v.caller, v.priority)
                end
            end
            -- 删除
            if operations.remove then
                for k, v in t_ipairs(operations.remove) do
                    self:removeEventListener(v.name, v.listener, v.caller)
                end
            end
            eventT.__operations__ = nil
        end
    end
end

--[[
是否存在该事件侦听
@name	事件名
--]]
function M:hasEventListener(name)
    assert(type(name) == "string" or type(name) == "number", "Invalid event name of argument 1, need a string or number!")
    return self.__eventMap__[name] ~= nil
end

rawset(_G, "Dispatcher", M)
Dispatcher = M.new()