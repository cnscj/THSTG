local M = class("CoroutineTest")
local util = Util
function M:ctor()
    local coroutineFun = function()
        print(15,"StartCoroutine ")
        for i = 1, 10 do
            -- yield_return(CS.UnityEngine.WaitForSeconds(1))
            coroutine.yield(CS.UnityEngine.WaitForSeconds(1))
            print(15,'Wait for 1 seconds')
            
            if i == 3 then
                print(15,"StopCoroutine")
            end
        end
    end

    local coroutineCall = util.coroutine_call(coroutineFun)
    coroutineCall()
end

return M