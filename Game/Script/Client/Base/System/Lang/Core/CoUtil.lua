-- 协程中使用的一些方法
CoUtil = {}

local coroutinePool = {}

-- 复用协程create方法
function CoUtil.create(f)
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

-- 改自xlua中，resume阻断
function CoUtil.asyncToSync(f, callbackPos)
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

-- 暂停一会
CoUtil.delayTime = CoUtil.asyncToSync(function(time, cb)
	Timer:scheduleOnce(time, cb)
end)
