local M = {}

--
function M.normalize(path)
    return string.gsub(path, "\\", "/") 
end

function M.combine(path1,path2)
    --如果最后一个字符是/,就直接拼,不然就加上
    local newPath1 = M.normalize(path1)
    local newPath2 = M.normalize(path2)
    local endChar = string.sub(newPath1,string.len(newPath1),1)
    if endChar == "/" then
        return newPath1 .. newPath2
    end
    return newPath1 .."/".. newPath2
end

--获取路径
function M.getDirectoryName(filename)
    if string.isEmpty(filename) then
        return filename
    end
    return string.match(filename, "(.+)/[^/]*%.%w+$") or "" --*nix system
end

--获取文件名
function M.getFileName(filename)
    if string.isEmpty(filename) then
        return filename
    end
    local name = string.match(filename, ".+/([^/]*%.%w+)$")
    return name or filename
end

--去除扩展名
function M.getFileNameWithoutExtension(filename)
    if string.isEmpty(filename) then
        return filename
    end

    local name = M.getFileName(filename)
    local idx = string.match(name, ".+()%.%w+$")
    if (idx) then
        return string.sub(name, 1, idx - 1)
    else
        return name
    end
end

--获取扩展名
function M.getExtension(filename)
    if string.isEmpty(filename) then
        return filename
    end
    local ex = string.match(filename, ".+%.(%w+)$")
    if ex then
        return string.format(".%s", ex)
    end
    return ""
end

rawset(_G, "PathTool", M)