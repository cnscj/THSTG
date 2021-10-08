local M = {}

function M.createComp(obj,compType,args)
    local cls = GComponent
    if type(compType) == "string" then

    elseif type(compType) == "table" then
        cls = compType
    end

    if cls == nil then
        error(string.format("getCompType error type:%s not exist.", compType))
    end

    return cls.new(obj, args)
end

function M.getObjPath( obj)

end

rawset(_G, "FGUIUtil", M)