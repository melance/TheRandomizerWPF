local paramerror = false

if tonumber(cr) == nil then
	print("<span style='color:red'>CR must be a number.  Value provided was '" .. cr .. "'.</span>")
	paramerror = true
end

if paramerror then return end

local roll = calc:Roll(1,100)

if individual == 'True' then
	local treasure = calc:CSVToTable('DataFiles\\LUA\\DnD 5e Individual Treasure.csv')
	
	for key,value in pairs(treasure) do
		if tonumber(value.CRLow) <= tonumber(cr) and tonumber(value.CRHigh) >= tonumber(cr) and
		   tonumber(value.ResLow) <= roll and tonumber(value.ResHigh) >= roll then
		   print('CP: ' .. calc:Evaluate(value.CP))
		   print('SP: ' .. calc:Evaluate(value.SP))
		   print('GP: ' .. calc:Evaluate(value.GP))
		   print('EP: ' .. calc:Evaluate(value.EP))
		   print('PP: ' .. calc:Evaluate(value.PP))
		end
	end
else
	local treasureCoins = calc:CSVToTable('DataFiles\\LUA\\DnD 5e Hoard Treasure Coins.csv')
	local treasure = calc:CSVToTable('DataFiles\\LUA\\DnD 5e Hoard Treasure.csv')
	local magicItems = calc:CSVToTable('DataFiles\\LUA\\DnD 5e Magic Items.csv')
	local gems = calc:CSVToTable('DataFiles\\LUA\\DnD 5e Gems and Art.csv')
	
	-- Coins
	
	for key,value in pairs(treasureCoins) do
		if tonumber(value.CRLow) <= tonumber(cr) and tonumber(value.CRHigh) >= tonumber(cr) then
			print('CP: ' .. calc:Evaluate(value.CP))
			print('SP: ' .. calc:Evaluate(value.SP))
			print('GP: ' .. calc:Evaluate(value.GP))
			print('EP: ' .. calc:Evaluate(value.EP))
			print('PP: ' .. calc:Evaluate(value.PP))
		end
	end
	
	-- Gems and Art
	
	for key,value in pairs(treasure) do
		if tonumber(value.CRLow) <= tonumber(cr) and tonumber(value.CRHigh) >= tonumber(cr) and
		   tonumber(value.ResLow) <= roll and tonumber(value.ResHigh) >= roll then
		    if value.GemType ~= '' then
				local gemType = value.GemType
				local gemCount = calc:Evaluate(value.GemCount)
				local gemCost = value.GemCost
				local gemRoll 
				
				for i = 1 to gemCount do
					gemRoll = calc:Roll(1,100)
					for gemKey,gemValue in pairs(gems) do
						if gemCost = gemValue.GemCost and gemType = gemValue.GemType and
						   gemRoll >= gemValue.ResLow and gemRoll <= gemValue.ResHigh then
						   print(gemValue.Item .. ' (' .. gemCost .. ')')
						end
					end
				end
			end
		end
	end
	
	-- Magic Items
	
	
end