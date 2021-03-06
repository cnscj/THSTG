--[[
    这边放所有 hotfix 的代码，如果hotfix的函数数量太多了可以拆分出脚本放到同目录，再require进来
--]]
--///////////////////////////// Example /////////////////////////////////////

--// 官方文档  https://github.com/Tencent/xLua/blob/master/Assets/XLua/Doc/hotfix.md
--// xlua.hotfix() 里面不能调用原来的函数，需要的话改成用 XLuaUtil.hotfix_ex()，会慢一些

-- local logFunc = CSharp.Logger.LogWarning or print
-- logFunc("start Hotfix Example...")

-- --static func
-- logFunc(string.format("before: %s", CSharp.HotfixDriver.HotfixPlaceholderStatic()))
-- xlua.hotfix(CSharp.HotfixDriver, 'HotfixPlaceholderStatic', function()
--     return 5000000
-- end)
-- logFunc(string.format("after: %s", CSharp.HotfixDriver.HotfixPlaceholderStatic()))


--instance func1
--这个函数不能调用原来的方法，不然堆栈溢出
-- xlua.hotfix(CS.ASGame.ResourceLoader, 'DestroyLoader', function(self, loader)
--     logFunc("before DestroyLoader: " .. loader:GetUniqueKey())
--     if not CSharp.PublicUtil.IsNull(loader) then
--         loader:Release()
--     end
-- end)

-- --instance func2
-- --同名函数，hotfix_ex内部也会调用xlua.hotfix() 会覆盖上面的hotfix
-- XLuaUtil.hotfix_ex(CS.ASGame.ResourceLoader, "DestroyLoader", function(self, loader)
--     self:DestroyLoader(loader)
--     logFunc("after DestroyLoader: " .. loader:GetUniqueKey())
-- end)

-- logFunc("end Hotfix Example...")

--Hotfix 第一个require
--xlua 扩展方法也放这边
require "Hotfix.Hotfixer"
require "Hotfix.HotfixUtil"

--占位用的，用来测试hotfix有没有用，不要删除！
HotfixUtil.hotfix(CS.SEGame.HotfixDriver, 'HotfixPlaceholderStatic', function()
    print("checking hotfix static ok")
end)

HotfixUtil.hotfixEx(CS.SEGame.HotfixDriver, "HotfixPlaceholderInstance", function(self)
    self:HotfixPlaceholderInstance()
    print("checking hotfix instance ok")
end)


--///////////////////////// 以下是代码，或者require其它脚本 ////////////////////////////////
if __ENGINE_VERSION__ < 101 then

end

if __ENGINE_VERSION__ < 102 then

end