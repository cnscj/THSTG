--先缓存
local oldHotfix = xlua.hotfix


--由于xlua.hotfix()之后lua向C#注入了回调，此时调用LuaEnv.Dispose()会报异常，所以需要在调用该函数之前清除所有hotfix
local M = {
    _register = {}  --{ csPath={functionName1=bool, functionName2=bool},}
}

function M.removeAll()
    for cs, funcs in pairs(M._register) do
        oldHotfix(cs, funcs, nil)
    end
    M._register = {}
end

--这里重写，用来自动收集所有hotfix，方便统一清除
--其它地方禁止再重写该方法

--xlua.hotfix会把 field,func组成一个table，如果field是table就会忽略func
--field是table时，key即函数名，只能是字符串！
xlua.hotfix = function(cs, field, func)
    oldHotfix(cs, field, func)

    M._register[cs] = M._register[cs] or {}
    local funcs = M._register[cs]

    --不管这里是注册还是反注册，全部hotfix的funcName都记下来
    if type(field) == "table" then
        for fn, f in pairs(field) do
            funcs[fn] = f and true or false
        end
    else
        funcs[field] = func and true or false
    end
end

rawset(_G, "Hotfixer", M)