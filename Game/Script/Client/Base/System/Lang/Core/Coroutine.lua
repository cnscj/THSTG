-- 协程中使用的一些方法
local CoroutineRunner = CS.SEGame.CoroutineRunner.GetInstance()
local Util = require("System.XLua.Util")


function coroutine.wait(seconds)
	return coroutine.yield(CS.UnityEngine.WaitForSeconds(seconds))
end

function coroutine.inter()
	return
end

function coroutine.generator(...)
	local iEnu = Util.cs_generator(...)
	return iEnu
end

function coroutine.start(iEnu)
	if (iEnu) then
		return CoroutineRunner:StartCoroutine(iEnu)	
	end
end

function coroutine.stop(cor)
	if (cor) then
		CoroutineRunner:StopCoroutine(cor)
	end
end

function coroutine.call(...)
	local iEnu = coroutine.generator(...)
	return coroutine.start(iEnu)
end

-- 改自xlua中，resume阻断
function coroutine.asyncToSync(f, callbackPos)
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