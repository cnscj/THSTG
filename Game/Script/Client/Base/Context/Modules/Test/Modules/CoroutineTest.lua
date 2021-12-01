local M = class("CoroutineTest")
function M:ctor()
    -- local coroutineFun = function()
    --     print(15,"StartCoroutine ")
    --     for i = 1, 10 do
    --         -- yield_return(CS.UnityEngine.WaitForSeconds(1))
    --         coroutine.yield(CS.UnityEngine.WaitForSeconds(1))
    --         print(15,'Wait for 1 seconds')
            
    --         if i == 3 then
    --             print(15,"StopCoroutine")
    --         end
    --     end
    -- end

    -- -- local coroutineCall = util.coroutine_call(coroutineFun)
    -- -- coroutineCall()

    -- local cor = util.cs_generator(function ( ... )
    --     for i = 1, 10 do
    --         -- yield_return(CS.UnityEngine.WaitForSeconds(1))
    --         coroutine.yield(CS.UnityEngine.WaitForSeconds(1))
    --         print(15,'Wait for 1 seconds')
            
    --         if i == 3 then
    --             print(15,"StopCoroutine")
    --         end
    --     end
    -- end)

    -- Coroutine.start(cor)

    -- local iEnu = coroutine.call(function (a)
    --     for i = 1, 10 do
    --         coroutine.yield(CS.UnityEngine.WaitForSeconds(1))
    --         print(15,'Wait for 1 seconds')
            
    --         if i == 3 then
    --             print(15,"StopCoroutine")
    --         end
    --         print(15,a,"a")
    --     end
    -- end,"11")

    local loadABFileAsync = coroutine.generator(function ()
        print(15,"!!!")
        local request = CS.UnityEngine.AssetBundle.LoadFromFileAsync("/Users/cnscj/UnityWorkspace/THSTG/Game/Resource/pc/effect/60100002.ab")
		while (not request.isDone) do
			coroutine.yield() 
		end

        coroutine.yield(request)

        local ab = request.assetBundle
        local req = ab:LoadAssetAsync("assets/gameassets/effect/60100002.prefab")

        coroutine.yield(req)
        local data = req.asset
        
        print(15,data,"的D是多少")

    end) 
    -- local id = coroutine.start(loadABFileAsync)
    -- coroutine.stop(id)
    local abRootPath = "/Users/cnscj/UnityWorkspace/THSTG/Game/Resource/pc/"
    AssetLoaderManager:getOrCreateBundlerLoader():loadManifest(PathTool.combine(abRootPath,"pc"))
    -- local task = AssetLoaderManager:loadBundleAssetAsync("effect/60100002.ab|assets/gameassets/effect/60100002.prefab")
    -- task.onSuccess = function (result)
    --     local data = result.data
    --     CS.UnityEngine.Object.Instantiate(data)
    -- end

    -- task:release()

    local task = AssetLoaderManager:loadBundleAssetSync("effect/60100002.ab|assets/gameassets/effect/60100002.prefab",false,function ( result )
        local data = result.data
        CS.UnityEngine.Object.Instantiate(data)
        dump(15,result)
    end)

    -- dump(15,task)
    -- dump(15,task:getData())
end

return M