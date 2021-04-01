require("Library.EventCenter.Dispatcher")

require("Library.ObjectPool.ObjectPool")
require("Library.ObjectPool.ObjectPoolManager")

require("Library.Timer.TimerNode")
require("Library.Timer.Timer")

require("Library.AssetManager.AssetManager")

--
Timer:scheduleOnce(10,function ( ... )
    print(15,"3232@@@@@@@@")
end)