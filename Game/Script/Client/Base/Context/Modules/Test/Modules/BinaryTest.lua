local M = class("BinaryTest")
function M:ctor()
    -- local task = AssetLoaderManager:loadBytesAssetSync("/Users/cnscj/UnityWorkspace/THSTG/Game/Resource/README.md")
    -- local bytes = task:getData()
    -- print(15,bytes)

    local task = AssetLoaderManager:loadBytesAssetAsync("/Users/cnscj/UnityWorkspace/THSTG/Game/Resource/README.md")
    task.onSuccess = function ( result )
        print(15,result.data)
    end
end

return M