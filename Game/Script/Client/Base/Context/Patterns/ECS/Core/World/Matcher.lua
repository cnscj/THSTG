local M = class("Matcher")
--持有所有entity和system和负责收集对应的entity

function M:allOf(...)

    return self
end

function M:anyOf(...)

    return self
end

function M:noneOf(...)

    return self
end

---
function M.allOf(...)
    local matcher = Matcher.new()
    matcher:allOf(...)
    return matcher
end

function M.anyOf(...)
    local matcher = Matcher.new()
    matcher:anyOf(...)
    return matcher
end

function M.noneOf(...)
    local matcher = Matcher.new()
    matcher:anyOf(...)
    return matcher
end

rawset(_G, "Matcher", M)
Matcher = M