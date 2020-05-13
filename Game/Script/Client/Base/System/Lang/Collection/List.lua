--------------------------双向链表类----------------------------------
--Left--  |head <--> node <--> ... <--> node <--> tail|  --Right--

local ListNode = simpleClass("ListNode")
function ListNode:ctor( value, prev, next )
	self.value = value
	self.prev = prev
	self.next = next
end
rawset(_G, "ListNode", ListNode)


local List = simpleClass("List")

function List:ctor()
	self._tail = nil
	self._head = nil
	self._size = 0
end

function List:clear()
	self._head = nil
	self._tail = nil
	self._size = 0
end

function List:size()
	return self._size
end

function List:head()
	if self._head then
		return self._head.value
	else
		return nil
	end
end

function List:tail()
	if self._tail then
		return self._tail.value
	else
		return nil
	end
end

function List:clone()
	local list = List.new()
	for v in ilist(self) do
		list:pushRight(v)
	end
	return list
end

function List:pushLeft(value)
	local node = ListNode.new(value)

	if not self._head and not self._tail then
		self._head = node
		self._tail = node
		self._size = self._size + 1
		return node
	end

	node.next = self._head
	self._head.prev = node

	self._head = node  --new head

	self._size = self._size + 1

	return node
end

function List:popLeft()
	local head = self._head
	if not head then return end

	self:_remove(head)
	return head.value
end

function List:pushRight(value)
	local node = ListNode.new(value)

	if not self._head and not self._tail then
		self._head = node
		self._tail = node
		self._size = self._size + 1
		return node
	end

	node.prev = self._tail
	self._tail.next = node

	self._tail = node  --new tail

	self._size = self._size + 1

	return node
end

function List:popRight()
	local tail = self._tail
	if not tail then return end

	self:_remove(tail)
	return tail.value
end

function List:contains(value)
	local node = self:_find(value)
	return node and true or false
end

function List:remove(value)
	local node = self:_find(value)
	if node then
		return self:_remove(node)
	end
end

function List:_find(value)
	local node = self._head
	while node do
		if value == node.value then
			return node
		else
			node = node.next
		end
	end
	return nil
end

function List:_findlast(value)
	local node = self._tail
	while node do
		if value == node.value then
			return node
		else
			node = node.prev
		end
	end
	return nil
end

function List:_remove(node)
	if not node then return node end

	local _prev = node.prev
	local _next = node.next

	if _prev then
		_prev.next = _next
	end
	if _next then
		_next.prev = _prev
	end

	if self._head == node then
		self._head = _next
	end
	if self._tail == node then
		self._tail = _prev
	end

	self._size = math.max(0, self._size - 1)

	return node
end


--iterator
local function ilist(list)
	local node = list._head

	return function()
		if node then
			local value = node.value
			node = node.next
			return value
		end
		return nil
	end
end
rawset(_G, "ilist", ilist)

--reverse iterator
local function rilist(list)
	local node = list._tail
	
	return function()
		if node then
			local value = node.value
			node = node.prev
			return value
		end
		return nil
	end
end
rawset(_G, "rilist", rilist)

rawset(_G, "List", List)
