local M = {}
local P_Resource = require("Config.Profile.P_Resource")
local KEY_PLATFORM = "{platform}"

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
--

--
function M.normalizePath(path, params)
    path = M._paresKeys(path,params)
    path = string.lower(path)

    return path
end

function M._paresKeys(path,params)
    for k,v in pairs(params) do
        local formatKey = string.format("{%s}",k)
        path = string.gsub(path,formatKey,v)
    end 
    path = string.gsub(path,KEY_PLATFORM,"pc")

    return path
end

rawset(_G, "PathConfig", M)
