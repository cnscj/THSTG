local M = {}
local XLuaUtil = Util

function M.hotfix( ... )
    return xlua.hotfix(...)
end

--与原来的区别在于可以调用原来的函数
function M.hotfixEx( ... )
    return XLuaUtil.hotfix_ex(...)
end

function M.extendMember(csClass, stateTb)
    xlua.hotfix(csClass, 
    {
        ['.ctor'] = function(self)
            return XLuaUtil.state(self, stateTb)
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

--扩展 MonoBehaviour
--csClass: 类的完整路径，如 CS.GYGame.ModelFxLoader
--stateTb: 扩展的变量和函数集合
 function M.extendMonoBehaviour(csClass, stateTb)
    --创建测试实例
    local go = CSharp.GameObject()
    local comp = go:AddComponent(typeof(csClass))
    --拿到实例的公有metatable
    local csobj_mt = getmetatable(comp)

    --修改公有metatable
    local oldIndex = rawget(csobj_mt, "__index")
    local oldNewIndex = rawget(csobj_mt, "__newindex")

    rawset(csobj_mt, "__index", function(t, k)
        return rawget(stateTb, k) or oldIndex(t, k)
    end)
    rawset(csobj_mt, "__newindex", function(t, k, v)
        if rawget(stateTb, k) ~= nil then
            rawset(stateTb, k, v)
        else
            oldNewIndex(t, k, v)
        end
    end)

    --用完删除测试实例
    CSharp.GameObject.Destroy(go)
end

rawset(_G, "XLuaUtil", M)