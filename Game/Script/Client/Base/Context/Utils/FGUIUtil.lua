local M = {}

--NOTE:应该用一个table装载
function M.createComp(obj,compType,args)
    local cls = GComponent
    if type(compType) == "string" then

    elseif type(compType) == "table" then
        cls = compType
    end

    if cls == nil then
        error(string.format("getCompType error type:%s not exist.", compType))
    end

    return self:convertComponent(obj,cls,args)
end

function M.getObjPath( obj)

end

rawset(_G, "FGUIUtil", M)