
local M = class("GAccordion", GComponent)

function M:ctor()
    -- self.btnUrl = ""
    -- self.listUrl = ""

    self.compUrl = "ui://Friend/FriendMenu"
    self.friendMenu = {}

    self._clickItemFunc = false
end

function M:init()
    self.friendMenu = {
        self:getChild("n0", "FriendMenu"),
        self:getChild("n1", "FriendMenu"),
        self:getChild("n2", "FriendMenu"),
        -- self:getChild("n3", "FriendMenu"),
    }

    local function clickHead(context)
        MenuUtil.showAvatar(context.sender.data, context.sender)
    end

    local _height = self:getHeight() - self.friendMenu[1].btn:getHeight() * #self.friendMenu

    for k, v in ipairs(self.friendMenu) do
        v:shrink()
        v:setListHeight(_height)
        v.btn:setMode(FairyGUI.ButtonMode.Check)
        v.btn:onClick(function ()
            local tar = v.btn:isSelected()
            for kk, vv in ipairs(self.friendMenu) do
                if tar == true then
                    if kk == k then
                        vv:expand()
                    else
                        vv:shrink()
                    end
                else
                    if kk == k then
                        vv:shrink()
                    end
                end
            end
        end)

        v.list:setVirtual()
        v.list:onClickItem(function (context)
            for kk, vv in ipairs(self.friendMenu) do
                local tar = vv.btn:isSelected()
                if tar == false then
                    vv.list:clearSelection()
                end
            end

            if self._clickItemFunc then
                self._clickItemFunc(context)
            end
        end)
        local function intimacyOnClick(context)
            local comp =  context.sender.data.comp:hasParent()
            local intimacy = context.sender.data.intimacy
            PopUpManager.show({
                tipType = ToolTipType.Intimacy,
                data = {
                    intimacy = intimacy
                },
                sender = comp,
                offset = Vector2(comp.width,comp.height/2),
                pivot = Vector2(0, 0.5),
            })
        end
        v.list:setStencil()
        v.list:setState(function (data, index, comp, obj)
            local norLoader = comp:getChild("norLoader", "Loader")
            local playerNameBar = comp:getChild("playerNameBar", "PlayerNameBar")
            local playerHead = comp:getChild("playerHead", "PlayerHead")
            local redDot = comp:getChild("redDot", "RedDot")
            local levelLabel = comp:getChild("levelLabel","Label")
            local intimacyBtn = comp:getChild("intimacyBtn","Button")
            playerNameBar:setPlayerName({
                name = data.name,
                vipLv = data.vipLv,
                svipLv = data.svipLv,
                svipShow = data.svipShow
            })
            playerHead:setMiniPlayer(data)
            playerHead:setGrayed(not data.online)
            levelLabel:setText(PlayerUtil.getLevelStr(data.level))
            if data.online then
                norLoader:setUIUrl("UI", "gg_di_nor")
            else
                norLoader:setUIUrl("UI", "gg_di_hui")
            end
            playerHead:onClick(clickHead)
            playerHead:setData(data)
            comp:setData(data)
            intimacyBtn:setGrayed(not data.online or tonumber(data.intimacy) == 0)
		    intimacyBtn:setText(data.intimacy)
		    intimacyBtn:onClick(intimacyOnClick)
		    intimacyBtn:setData({
			    comp = comp,
			    intimacy = data.intimacy,
		    })
        end)
    end
end

function M:destroy()
    self.friendMenu = false
    self._clickItemFunc = false

    self:super("GComponent", "destroy")
end

function M:setText(index, text)
    self.friendMenu[index]:setText(text)
end

function M:setList(index, text)
    self.friendMenu[index]:setList(text)
end

function M:updateList(index)
    self.friendMenu[index]:updateList()
end

function M:setListHeight(index, height)
    self.friendMenu[index]:setListHeight(height)
end

function M:setClickItemFunc(func)
    self._clickItemFunc = func
end

function M:setSelectedIndex(index, index2, call)
    for k, v in ipairs(self.friendMenu) do
        if k == index then
            v:expand()
            if index2 then
                v.list:setSelectedIndex(index2, call)
            end
        else
            v:shrink()
        end
    end
end

function M:getListData(index)
    return self.friendMenu[index]:getListData()
end


function M:clearSelection()
     for k, v in ipairs(self.friendMenu) do
        v.list:clearSelection()
    end
end

rawset(_G, "GAccordion", M)
