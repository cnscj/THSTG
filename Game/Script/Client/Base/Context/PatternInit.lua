--ECS模块
rawset(_G, "ECS", {})
ECS = {
    World = require("Context.Patterns.ECS.Core.World"),
    Entity = require("Context.Patterns.ECS.Core.Entity"),
    Component = require("Context.Patterns.ECS.Core.Component"),
    System = require("Context.Patterns.ECS.Core.System"),
}
require("Context.Patterns.ECS.Base.BitNum")
require("Context.Patterns.ECS.Base.Archetype")
require("Context.Patterns.ECS.ECSManager")


--MVC模块
rawset(_G, "MVC", {})
MVC = {
    Cache = require("Context.Patterns.MVC.Core.Cache"),
    Controller = require("Context.Patterns.MVC.Core.Controller"),
    View = require("Context.Patterns.MVC.Core.View")
}
require("Context.Patterns.MVC.MVCManager")

