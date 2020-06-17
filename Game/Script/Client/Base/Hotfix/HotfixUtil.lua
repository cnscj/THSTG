local M = {}
local XLuaUtil = Util

function M.hotfix( ... )
    return xlua.hotfix(...)
end

--与原来的区别在于可以调用原来的函数
function M.hotfixEx( ... )
    return XLuaUtil.hotfix_ex(...)
end

--扩展C#类成员变量或方法，就是在构造函数里设置新的metatable，不要直接修改类，只能修改构造函数之后的实例。
function M.extendMember(csClass, stateTb)
    xlua.hotfix(csClass, 
    {
        ['.ctor'] = function(csobj)
            return XLuaUtil.state(csobj, stateTb)
        end,
    })
end

--扩展实例的成员变量或方法，因为它的构造函数早就调了，动态扩展的直接修改实例的metatable。不能直接修改类的metatable，否则变成改静态变量或方法了
function M.extendInstanceMember(csInstance, stateTb)
    XLuaUtil.state(csInstance, stateTb)
end

--类本身有一个metatable，可以直接修改静态函数和静态变量，跟实例无关，所以可以改
function M.extendStaticMember(csClass, stateTb)
    XLuaUtil.state(csClass, stateTb)
end

rawset(_G, "XLuaUtil", M)