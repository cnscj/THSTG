--LuaTaskAwaiter
--@module LuaTaskAwaiter
local TaskAwaiter = simpleClass("TaskAwaiter")

function TaskAwaiter:ctor()
	self.isCompleted = false
	self.packaged = false
	self.actions = {}
end

function TaskAwaiter:getException()
	return self.exception
end

function TaskAwaiter:getResult()
	if not self.isCompleted then
		error("The task is not finished yet")
	end
	if self.exception then
		error(self.exception)
	end
	
	return self.result
end

function TaskAwaiter:setResult(result,exception,packaged)	
	if exception then
		self.exception = exception
	else
		self.result = result
	end
	
	self.isCompleted = true
	self.packaged = packaged
	
	if not self.actions then
		return
	end
	
	for _, v in pairs(self.actions) do
		if v then
			xpcall(v,function(err)
					log:error("%s \n%s",err,debug.traceback())
				end)
		end
	end
end

function TaskAwaiter:onCompleted(action)
	if not action then
		return
	end
	
	if self.isCompleted then
		xpcall(action,function(err)
				log:error("%s \n%s",err,debug.traceback())
			end)
		return
	end
	
	table.insert(self.actions,action)
end

---
--AsyncTask
--@module AsyncTask
local AsyncTask = simpleClass("AsyncTask",TaskAwaiter)

function async(action)
	return function(...)
		local task = AsyncTask.new()
		if type(action)~='function' then
			task:setResult(nil,"please enter a function")
			return task
		end
		
		local co = coroutine.create(function(...)
			local results = table.pack(xpcall(action,function(err)
						task:setResult(nil,err,false)
						log:error("%s \n%s",err,debug.traceback())
					end,...))
			
			local status = results[1]				
			if status then
				table.remove(results,1)
				if #results <=1 then				
					task:setResult(results[1],nil,false)
				else
					task:setResult(results,nil,true)
				end
			end	
		end)
		coroutine.resume(co,...)
		return task
	end
end

function await(result)
	assert(result ~= nil,"The result is nil")
	
	local status, awaiter
	if type(result)=='table' and iskindof(result,"TaskAwaiter") then	
		awaiter = result
	elseif type(result) == 'userdata' or type(result) == 'table' then
		status, awaiter = pcall(result.GetAwaiter,result)
		if not status then
			error("The parameter of the await() is error,not found the GetAwaiter() in the "..tostring(result))
		end
	else
		error("The parameter of the await() is error, this is a function, please enter a table or userdata")
	end
	
	if awaiter.isCompleted then
		local value = awaiter:getResult()
		if type(value) == 'table' and awaiter.packaged then
			return table.unpack(value)
		else
			return value
		end
	end
	
	local id = coroutine.running()
	local isYielded= false
	awaiter:onCompleted(function()			
			if isYielded then
				coroutine.resume(id)
			end
		end)
	
	if not awaiter.isCompleted then
		isYielded = true
		coroutine.yield()
	end
	
	local value = awaiter:getResult()
	if type(value) == 'table' and awaiter.packaged then
		return table.unpack(value)
	else
		return value
	end
end

function try(block)
	local main = block[1]
	local catch = block.catch
	local finally = block.finally
	
	local results = table.pack(pcall(main))
	local status = results[1]
	local e = results[2]
	table.remove(results,1)
	local result = results
	local catched = false
	if (not status) and catch and type(catch)=='function' then
		catched = true
		local results = table.pack(pcall(catch,e))
		if results[1] then
			table.remove(results,1)
			result = results
			e = nil
		else
			e = results[2]
		end		
	end
	
	if finally and 	type(finally)=='function' then
		pcall(finally)
	end	
	
	if status then
		return table.unpack(result)
	elseif catched then
		if not e then
			return table.unpack(result)
		else
			error(e)
		end
	else
		error(e)
	end
end


