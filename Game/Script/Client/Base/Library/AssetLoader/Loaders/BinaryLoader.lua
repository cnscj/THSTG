
local M = class("AssetBinaryLoader",AssetBaseLoader)
local IO = CS.System.IO
function M:ctor()
    
end

function M:_loadFileBytesAsync(loaderHandler)
    --每次加载1024个字直至加载完
    local path = loaderHandler.path
    local byteData = false
    local fsRead = false
    if pcall(function() 
        fsRead = IO.FileStream(path,IO.FileMode.Open)
    end) then
        local fsLen = fsRead.Length
        local stLen = 1024 * 1024
        local heBytes = CS.XLua.BytesRef(fsLen) 
        local hadReadLen = 0
        while true do 
            local needReadLen = (hadReadLen + stLen) < fsLen and stLen or (fsLen - hadReadLen)
            local r = fsRead:Read(heBytes, hadReadLen, needReadLen)
            hadReadLen = hadReadLen + r
    
            if hadReadLen >= fsLen then 
                break
            else
                coroutine.yield()
            end 
        end
        byteData = heBytes.Target
    else
        printError(string.format("Unable to open file %s",path))
    end

    local loaderResult = AssetLoaderResult.new()
    loaderResult.isDone = true
    loaderResult.data = byteData 

    loaderHandler.result = loaderResult
    loaderHandler:finish()
end

function M:_loadFileBytesSync(loaderHandler)
    local path = loaderHandler.path
    local bytes = false
    if not pcall(function() 
        bytes = IO.File.ReadAllBytes(path) 
    end) 
    then
        printError(string.format("Unable to open file %s",path))
    end

    local loaderResult = AssetLoaderResult.new()
    loaderResult.isDone = true
    loaderResult.data = bytes

    loaderHandler.result = loaderResult
    loaderHandler:finish()

    return bytes
end

function M:_onLoadAsync(loaderHandler)    
    local iEnu = coroutine.generator(self._loadFileBytesAsync,self,loaderHandler)
    coroutine.start(iEnu)
end

function M:_onLoadSync(loaderHandler)
    return self:_loadFileBytesSync(loaderHandler)
end

rawset(_G, "AssetBinaryLoader", M)
return M