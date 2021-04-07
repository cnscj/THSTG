
local M = class("GTweenerHelper")

--[[
创建的Tweener调用kill(false)时偶尔会将正在播放的其他动效暂停
GTweener是重用的。注意检查你的所有代码，不要重用或者误用GTweener实例，
也就是说，Tween一旦结束，GTweener实例就不要再使用了，
更加不要去kill。一般建议不要保存GTweener实例。
]]

-- 简单的说，就是不能复用
function M:ctor(comp)
	if comp:getTweenerHelper() then
		comp:getTweenerHelper():stopAction()
	end
	comp:setTweenerHelper(self)

	self._tweener = false
	self._array = false
	self.index = 0
end

function M:runNext()
	self._tweener = false

	self.index = self.index + 1
	if not self._array then
		return
	end

	local func = self._array[self.index]
	if func then
		local tweener = func()
		if tweener then
			self._tweener = tweener

			tweener:OnComplete(function()
				-- 用完必须马上清除
				self._tweener = false

				if not self._array or self.index == #self._array then
					-- 结束了
					self._array = false
				else
					self:runNext()
				end
			end)
		else
			-- 用完必须马上清除
			self._tweener = false

			if not self._array or self.index == #self._array then
				-- 结束了
				self._array = false
			else
				self:runNext()
			end
		end
	end
end

function M:runAction(t)
	if self._tweener then
		self:stopAction()
	end

	self._array = t or {}

	self.index = 0
	self:runNext()
end

function M:stopAction()
	if self._tweener then
		-- 用完必须马上清除
		self._tweener:Kill() -- fgui
		self._tweener = false
	end

	self._array = false
end

function M:destroy()
	self:stopAction()
end

rawset(_G, "GTweenerHelper", M)
