local M = class("AssetLoaderTask",false,{
    LoadMode = {
        Sync = 1,
        Async = 2,
    },

    UseLoader = {
        Auto = 1,
        AssetBundle = 2,
        Resource = 3 
    }

})

function M:ctor()
    self._baseLoader = false

    self.loadMode = M.LoadMode.Async
    self.useLoader = M.UseLoader.Auto

    self.onSuccess = false
    self.onFailed = false
end

function M:start()
    self:clear()
    local loader = self:_getOrCrateLoader()


end

function M:stop()
    
end

function M:clear()
    self:_unload()

end

function M:_unload()

end

function M:_getOrCrateLoader()
    
end

function M:_onCompleted()

end
--

rawset(_G, "AssetLoaderTask", false)
