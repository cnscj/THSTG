local M = class("TestBehaviour",LuaBehaviour)

function M:start()
    print("Hello World")
end

rawset(_G, "TestBehaviour", M)