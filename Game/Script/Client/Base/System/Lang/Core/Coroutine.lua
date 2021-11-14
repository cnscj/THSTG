-- 协程中使用的一些方法
Coroutine = {}

local coroutinePool = {}

-- 复用协程create方法
function Coroutine.create(f)
	local co = table.remove(coroutinePool)
	if co == nil then
		co = coroutine.create(function(...)
			f(...)
			while true do
				f = nil
				coroutinePool[#coroutinePool+1] = co
				f = coroutine.yield("EXIT")
				f(coroutine.yield())
			end
		end)
	else
		coroutine.resume(co, f)
	end
	return co
end

function Coroutine.start(co)
	return function(...)
        assert(coroutine.resume(co, ...))
    end
end

function Coroutine.stop(co)
	
end

function Coroutine.yield(...)
	return coroutine.yield(...)
end
function Coroutine.yieldNull()
	return Coroutine.yield()
end
function Coroutine.yieldWaitForSeconds(seconds)
	return Coroutine.yield(CS.UnityEngine.WaitForSeconds(1))
end


-- 改自xlua中，resume阻断
function Coroutine.asyncToSync(f, callbackPos)
	return function(...)
		local _co = coroutine.running() or error ('this function must be run in coroutine')
		local rets
		local waiting = false
		local exist = false
		local function cb_func(...)
			if _co then        	
				if waiting then
					assert(coroutine.resume(_co, ...))
				else
					rets = {...}
				end
			-- else
			-- 	被外部resume阻断
			end
		end
		local params = {...}
		table.insert(params, callbackPos or (#params + 1), cb_func)
		f(unpack(params))
		if rets == nil then
			waiting = true
			rets = {coroutine.yield()}
			_co = nil
		end      
		return unpack(rets)
	end
end
