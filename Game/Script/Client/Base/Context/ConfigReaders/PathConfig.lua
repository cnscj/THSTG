local M = {}
local P_Resource = require("Config.Profile.P_Resource")

function M.getFGuiEditorPath()
    return P_Resource.fguiEditorPath
end

function M.getResourcePath()
    return P_Resource.resourcePath
end

--
function M.getModelPatternPath()
    return P_Resource.resDict.modelFolder
end

function M.getEffectPatternPath()
    return P_Resource.resDict.effectFolder
end

function M.getUIPatternPath()
    return P_Resource.resDict.fguiFolder
end
--

--
function M.normalizePath(path, params)
    path = M._paresKeys(path,params)
    path = string.lower(path)

    return path
end

function M._paresKeys(path,params)
    --NOTE:效率低下,不是很好
    for k,v in pairs(params) do
        local formatKey = string.format("{%s}",k)
        path = string.gsub(path,formatKey,v)
    end 
    path = string.gsub(path,"{%w+}",function ( w )
        local val = P_Resource.keyFunc[w]
        local repStr = nil
        if type(val) == "function" then
            repStr = func(params)   
        else
            repStr = val
        end

        if repStr ~= nil then
            return repStr
        end
    end)

    return path
end

rawset(_G, "PathConfig", M)
