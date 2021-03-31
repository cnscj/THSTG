----------------------------------对象池，为了重用TimerNode------------------------------------
--Timer.lua和OrderedTimer.lua 里面大量使用，减少table创建

--singleton
rawset(_G, "TimerNodePool", false)

require "Game.Utils.Timer.TimerNode"

--初始化单例
local params = 
{
    key = "TimerNodePool",
    createFunc = function()
        return TimerNode.new()
    end,
    releaseHandler = function(timerNode) 
        timerNode:reset() 
    end,

    timeThrehold = 60,  --这么长时间不用了，需要清理一波缓存
    countThrehold = 30,  --清缓存时，最多保留这么多个
    maxCount = 120,  --如果有这么多，不管怎么样都要清理掉一波
}

TimerNodePool = CommonSinglePool.new(params)

--添加到PoolManager，这样才能驱动自动清理的程序，也可以在PoolManager.ctor()里面配置，但必须保证该单例已经创建出来。
require "Game.AssetManage.PoolManager"
PoolManager:addPool(TimerNodePool)
