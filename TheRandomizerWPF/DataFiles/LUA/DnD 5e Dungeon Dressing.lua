local inspect = require('inspect')

local errors = false

function PrintError(message)
	print('<span style="color:red">' .. message .. '</span>')
end

function Exists(find, array)
	for key, value in pairs(array) do
		if find == value then return true end
	end
	return false
end

if type(structureType) == 'nil' or structureType == '' then 
	PrintError('Structure Type is required.')
	errors = true
elseif not Exists(structureType, {'Any','General','DeathTrap','Lair','Maze','Mine','PlanarGate','Stronghold','Temple','Tomb','TreasureVault'}) then 
	PrintError('Invalid Structure Type (' .. structureType .. ')')
	errors = true
end

if type(cr) == 'nil' or cr == '' then 
	cr = -1 
elseif type(cr) ~= 'string'	and tonumber(cr) == nil then
	PrintError('Challenge Rating must be blank or a number.')
	errors = true
end

if errors then return end

