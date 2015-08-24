local paramerror = false
local debugging = false

if tonumber(minCR) == nil then
	print("<span style='color:red'>Minimum CR must be a number.  Value provided was '" .. minCR .. "'.</span>")
	paramerror = true
end

if tonumber(maxCR) == nil then
	print("<span style='color:red'>Maximum CR must be a number.  Value provided was '" .. maxCR .. "'.</span>")
	paramerror = true	
end

if paramerror then return end

local allMonsters = calc:CSVToTable('DataFiles\\LUA\\DnD 5e Monsters.csv')

local monsters = {}

printif(debugging, 'Select Random: ' .. selectRandom)
printif(debugging, 'Size: ' .. size)
printif(debugging, 'Type: ' .. creatureType)
printif(debugging, 'Alignment: ' .. alignmentLNC)
printif(debugging, 'Alignment: ' .. alignmentGNE)
printif(debugging, 'Minimum CR: ' .. minCR)
printif(debugging, 'Maximum CR: ' .. maxCR)
	
for key, value in pairs(allMonsters) do
	printif(debugging, value.Name .. ' ' .. value.Size .. ' ' .. value.Type .. ' ' .. value.AlignmentLNC .. ' ' .. value.AlignmentGNE .. ' ' .. value.CR)
	if (type(size) == 'nil' or size == 'Any' or size == value.Size) and
	   (type(creatureType) == 'nil' or creatureType == 'Any' or creatureType == value.Type) and
	   (type(alignmentLNC) == 'nil' or alignmentLNC == 'Any' or value.AlignmentLNC == 'Any' or alignmentLNC == value.AlignmentLNC) and
	   (type(alignmentGNE) == 'nil' or alignmentGNE == 'Any' or value.AlignmentGNE == 'Any' or alignmentGNE == value.AlignmentGNE) and
	   (type(minCR) == 'nil' or tonumber(minCR) < 0 or tonumber(minCR) <= tonumber(value.CR)) and
	   (type(maxCR) == 'nil' or tonumber(maxCR) < 0 or tonumber(maxCR) >= tonumber(value.CR)) then
		table.insert(monsters, value)
	end	
end

if selectRandom == 'True' then
	monsters = {calc:SelectFromTable(monsters)}
end

function FormatTags(tags)
	if type(tags) == 'nil' or tags == '' then return ''
	else return ' (' .. tags .. ')' end
end	

function FormatAlignment(lnc,gne)
	if lnc == 'Any' and gne == 'Any' then return 'Any'
	elseif lnc == 'Any' and gne ~= 'Any' then return 'Any ' .. gne .. ' alignment'
	elseif lnc ~= 'Any' and gne == 'Any' then return 'Any ' .. lnc .. ' alignment'
	elseif lnc == 'Unaligned' or gne == 'Unaligned' then return 'Unaligned'
	elseif lnc == 'Neutral' and gne == 'Neutral' then return 'Neutral'
	else return lnc .. ' ' .. gne end
end

function FormatCR(cr)
	if cr == '0.125' then return '1/8'
	elseif cr == '0.25' then return '1/4'
	elseif cr == '0.5' then return '1/2'
	else return cr end
end

if unformatted == 'True' then
	selectedMonsters = monsters
else
	local result = [[<Table style="width:100%; border-collapse: collapse; border:1px solid black;">
				<tr style="border:1px solid black; background: black; color: white;">
					<th style="text-align:left; border:1px solid black;">Name</th>
					<th style="text-align:left; border:1px solid black;">Type</th>
					<th style="text-align:left; border:1px solid black;">Alignment</th>
					<th style="text-align:left; border:1px solid black;">Challenge</th>
					<th style="text-align:left; border:1px solid black;">Source</th>
				</tr>]]

	local odd = true			

	for key, value in pairs(monsters) do
		if odd then	result = result .. [[<tr style="border:1px solid black;">]]
		else result = result .. [[<tr style="border:1px solid black; background-color:#eee;">]] end
		
		result = result .. [[
			<td style="border:1px solid black;">]] .. value.Name .. [[</td>
			<td style="border:1px solid black;">]] .. value.Type .. FormatTags(value.Tags) .. [[</td>
			<td style="border:1px solid black;">]] .. FormatAlignment(value.AlignmentLNC,value.AlignmentGNE) .. [[</td>
			<td style="border:1px solid black;">]] .. FormatCR(value.CR) .. [[</td>
			<td style="border:1px solid black;">]] .. value.Source .. [[</td>
		</tr>]]
		
		odd = not odd
	end

	result = result .. '</Table>'

	print(result)
end