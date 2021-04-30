local M = class("ItemData")

--itemInfo:{type,code,amount}
function M.createFromInfo(itemInfo)

end

function M.createFromCode(code)
    return M.createFromInfo({type = 0, code = code, amount = 1})
end

function M:ctor()

end

rawset(_G, "ItemData", false)
ItemData = M