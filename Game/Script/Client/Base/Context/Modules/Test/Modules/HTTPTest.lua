local M = class("HTTPTest")
function M:ctor()
    -- HttpManager:get({
    --     ip = "https://api.bilibili.com/x/relation/",
    --     action = "stat",
    --     params = {
    --         vmid = 672328094,
    --     },
    --     onCallback = function ( result )
    --         local jsonStr = result:ToString()
    --         local tb = json.decode(jsonStr)
    --         dump(15,tb)
    --         print(15,"然然我真的好喜欢你木马,为了你,我要努力学编程")
    --     end
    -- })

    HttpManager:get({
        ip = "https://api.vc.bilibili.com/dynamic_svr/v1/dynamic_svr/",
        action = "space_history",
        params = {
            host_uid = 672328094,
            offset_dynamic_id = 0,
        },
        onCallback = function ( result )
            local jsonStr = result:ToString()
            local tb = json.decode(jsonStr)
            dump(15,tb)
        end
    })
end

return M