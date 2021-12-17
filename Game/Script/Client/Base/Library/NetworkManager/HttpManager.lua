local M = class("HttpManager")
local HttpManagerIns = CS.XLibGame.HttpManager.GetInstance()
function M:post(args,onSuccess,onFailed)
    args = args or {}
    args.onSuccess = args.onSuccess or onSuccess
    args.onFailed = args.onFailed or onSuccess
    args.isGetOrPost = false

    return self:send(args)
end

function M:get(args,onSuccess,onFailed)
    args = args or {}
    args.onSuccess = args.onSuccess or onSuccess
    args.onFailed = args.onFailed or onSuccess
    args.isGetOrPost = true

    return self:send(args)
end

--[[
-- ip			#string		地址，参考AgentReader.centerURL
-- action		#string		要执行的操作，一般使用HttpActionType枚举中的值
-- key			#string		加密用key，一般使用AgentReader.phpKey
-- params		#table		执行该条http请求的参数
-- isGetOrPost  #boolean	是否使用get，默认使用post
-- timeout		#number		超时时间(s)，TimeoutForRead等于该值，TimeoutForConnect等于该值的一半，默认值30s
-- onSuccess	#function	请求成功的回调，如：local function onSuccess(data) end
-- onFailed		#function	请求失败的回调，如：local function onFailed(data) end
]]
function M:send(args)
    if not args then return end

    local ip = args.ip
    if not ip then return end 

    local url = ip
    local action = args.action
    if not string.isEmpty(action) then 
        local endChar = string.sub(ip,string.len(ip))
        if endChar == "/" then
            url = string.format("%s%s?",ip,action) 
        else
            url = string.format("%s/%s?",ip,action) 
        end
    end
    local isGetOrPost = args.isGetOrPost

    local httpForm = CS.XLibGame.HttpForm()
    local params = args.params
    if params then
        for k,v in pairs(params) do 
            httpForm:Add(k,v)
        end
    end

    local httpParam = CS.XLibGame.HttpParams()
    httpParam.url = url
    httpParam.formData = httpForm
    httpParam.timeout = args.timeout or 30
    httpParam.onSuccess = args.onSuccess
    httpParam.onFailed = args.onFailed
    httpParam.onCallback = args.onCallback

    if isGetOrPost then
        return HttpManagerIns:Get(httpParam)
    else
        return HttpManagerIns:Post(httpParam)
    end
end

rawset(_G, "HttpManager", false)
HttpManager = M.new()
