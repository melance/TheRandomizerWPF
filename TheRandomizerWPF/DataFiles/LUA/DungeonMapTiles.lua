-- Requires registry setting: HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION\TheRandomizer DWORD32 = 11000
--[[local inspect = require('DataFiles\\LUA\\inspect')--]]

-- Validate parameters

mazeMaker = require('Maze Builder')

local maze = buildMaze(50,1)

size = (size / 100) * 265

function shallowcopy(orig)
    local orig_type = type(orig)
    local copy
    if orig_type == 'table' then
        copy = {}
        for orig_key, orig_value in pairs(orig) do
            copy[orig_key] = orig_value
        end
    else -- number, string, boolean, etc
        copy = orig
    end
    return copy
end

local debugMode = false
local map = {}

Tiles = calc:XMLFileToTable('DataFiles\\LUA\\DungeonMapTiles.xml')

printif(debugMode, inspect(Tiles,{newline="<br />"}))

local tilesCopy = shallowcopy(Tiles)

for index,value in ipairs(tilesCopy) do
	-- Create rotations
	local rot90 = shallowcopy(value)
	local rot180 = shallowcopy(value)
	local rot270 = shallowcopy(value)
	
	value.Rotation = 0
	
	rot90.Rotation = 90
	rot90.North = value.West
	rot90.East = value.North
	rot90.South = value.East
	rot90.West = value.South
	
	rot180.Rotation = 180
	rot180.North = value.South
	rot180.East = value.West
	rot180.South = value.North
	rot180.West = value.East
	
	rot270.Rotation = 270
	rot270.North = value.East
	rot270.East = value.South
	rot270.South = value.West
	rot270.West = value.North
	
	table.insert(Tiles,rot90)
	table.insert(Tiles,rot180)
	table.insert(Tiles,rot270)
end

local tileCount = #Tiles

for y = 1, height do
	for x = 1, width do
		local north = '0'
		local west = '0'
		local index = calc:Roll(1,tileCount)
		local count = 1
		local found = false
		local loopLimit = 100
		if type(map[y-1]) ~= 'nil' and type(map[y-1][x]) ~= 'nil' then north = map[y-1][x].South end
		if type(map[y]) ~= 'nil' and type(map[y][x-1]) ~= 'nil' then west = map[y][x-1].East end
		while not found do
			if count > tileCount then 
				count = 1 
				loopLimit = loopLimit - 1
				if loopLimit == 0 then 
					found = true 
				end
			end
			if Tiles[count].North == north and Tiles[count].West == west then 
				index = index - 1 
			end
			if index == 0 then
				found = true
				if type(map[y]) == 'nil' then map[y] = {} end
				map[y][x] = shallowcopy(Tiles[count])
			end
			count = count + 1
		end
	end
end

printif(debugMode, inspect(map))

local result = '<table style="border-collapse: collapse;border: 0px;">'

for indexY,valueY in ipairs(map) do
	result = result .. '\n\t<tr>'
	for indexX,valueX in ipairs(valueY) do
		result = result .. '\n\t\t<td><image src="' .. calc:ExpandPath(valueX.Tile) .. '" width="' .. size .. '" height="' .. size .. '" class="rot' .. valueX.Rotation .. '" style="display: inline; padding: 0; margin: 0; border: 0;" />'
	end
	result = result .. '\n\t</tr>'
end

result = result .. '</table>'

print(result)
print(inspect(map,{newline="<br />"}))