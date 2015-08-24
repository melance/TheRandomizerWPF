local paramerror = false
local debugMode = true

if tonumber(width) == nil and tonumber(width) < 10 then
	print("<span style='color:red'>Width must be a number greater than or equal to 10.  Value provided was '" .. width .. "'.</span>")
	paramerror = true
end

if tonumber(height) == nil and tonumber(height) < 10 then
	print("<span style='color:red'>Height must be a number greater than or equal to 10.  Value provided was '" .. height .. "'.</span>")
	paramerror = true
end

if tonumber(size) == nil and tonumber(size) < 1 then
	print("<span style='color:red'>Cell Size must be a number greater than or equal to 10.  Value provided was '" .. size .. "'.</span>")
end

if paramerror then return end

width = tonumber(width)
height = tonumber(height)

local function shallowcopy(orig)
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

local function buildMazeInternal(x,y,maze,braided,entrance,exit)
	
	local stack = {}
	local directions = {'North','East','South','West'}
	
	printif(debugMode,'Braided: ' .. braided)
	
	table.insert(stack,{x=x,y=y})
	maze[y][x].Visited = true
	
	while #stack > 0 do
		local index = 1
		local nextX = x
		local nextY = y
		local braid = false	
		
		for i = #directions, 2, -1 do -- backwards
			local r = calc:Roll(1,i) -- select a random number between 1 and i
			directions[i], directions[r] = directions[r], directions[i] -- swap the randomly selected item to position i
		end
		
		while index <= #directions and nextX == x and nextY == y do
			if directions[index] == 'North' and y > 1 and not maze[y-1][x].Visited then
				maze[y][x].North = true
				maze[y-1][x].South = true
				nextY = y-1
			elseif directions[index] == 'East' and x < width and not maze[y][x+1].Visited then
				maze[y][x].East = true
				maze[y][x+1].West = true
				nextX = x+1
			elseif directions[index] == 'South' and y < height and not maze[y+1][x].Visited then
				maze[y][x].South = true
				maze[y+1][x].North = true
				nextY = y+1
			elseif directions[index] == 'West' and x > 1 and not maze[y][x-1].Visited then
				maze[y][x].West = true
				maze[y][x-1].East = true
				nextX = x-1
			else
				index = index + 1
			end
		end
		
		if nextX ~= x or nextY ~= y then
			x = nextX
			y = nextY
			maze[y][x].Visited = true
			table.insert(stack,{x=x,y=y})
		else
			if braided > 0 then
				printif(debugMode,'Braid')
				if calc:Roll(1,100) <= braided then braid = true end
				
				if braid then
					index = 1
					local done = false
					while index <= #directions and not done do
						if directions[index] == 'North' and y > 1 then
							maze[y][x].North = true
							maze[y-1][x].South = true
							done = true
						elseif directions[index] == 'East' and x < width then
							maze[y][x].East = true
							maze[y][x+1].West = true
							done = true
						elseif directions[index] == 'South' and y < height then
							maze[y][x].South = true
							maze[y+1][x].North = true
							done = true
						elseif directions[index] == 'West' and x > 1 then
							maze[y][x].West = true
							maze[y][x-1].East = true
							done = true
						else
							index = index + 1
						end
					end
				end
			end
			
			x = stack[#stack].x
			y = stack[#stack].y
			table.remove(stack)
		end
	end
		
	if entrance then
		local x 
		local y
		local entrance = calc:Roll(1,#directions)
		
		if directions[entrance] == 'North' then
			x = calc:Roll(1,width)
			y = 1
			maze[y][x].North = true
		elseif directions[entrance] == 'East' then
			x = width
			y = calc:Roll(1,height)
			maze[y][x].East = true
		elseif directions[entrance] == 'South' then
			x = calc:Roll(1,width)
			y = height
			maze[y][x].South = true
		else
			x = 1
			y = calc:Roll(1,height)
			maze[y][x].West = true
		end
		if exit then
			-- Solve for the longest path that leads to an outer wall
		end
	end
end

local function printMazeHtml(maze)
	local height = #maze
	local width = #maze[1]
	local html = ''
	
	html = '<table style="border-collapse: collapse; font-size:' .. size .. 'px;">'
	
	for y = 1, height do
		html = html .. '\n\t<tr>'
		for x = 1, width do
			html = html .. '\n\t\t<td style="'
			if not maze[y][x].North then html = html .. 'border-top: 1px solid black;' end
			if not maze[y][x].South then html = html .. 'border-bottom: 1px solid black;' end
			if not maze[y][x].East then html = html .. 'border-right: 1px solid black;' end
			if not maze[y][x].West then html = html .. 'border-left: 1px solid black;' end
			html = html .. '">&nbsp;&nbsp;&nbsp;</td>'
		end
		html = html .. '\n\t</tr>'
	end
	html = html .. '\n</table>'
	print(html)
end

local function printMazeAscii(maze)
	local height = #maze
	local width = #maze[1]
	for iy,vy in ipairs(maze) do
		line1 = ''
		line2 = ''
		line3 = ''
		for ix,vx in ipairs(vy) do
			local cell1 = ''
			local cell2 = ''
			local cell3 = ''
			if vx.North then
				cell1 = '# #'
			else
				cell1 = '###'
			end
			if vx.East and vx.West then
				cell2 = '   '
			elseif vx.East then
				cell2 = '#  '
			elseif vx.West then
				cell2 = '  #'
			else
				cell2 = '# #'
			end
			line1 = line1 .. cell1
			line2 = line2 .. cell2
		end
		print(line1)
		print(line2 .. iy)
	end
	print(string.rep('###',width))
end

function printMaze(maze, html)
	if html then
		printMazeHtml(maze)
	else
		printMazeAscii(maze)
	end
end

function buildMaze(braided,entrance,exit)
	local maze = {}
	
	printif(debugMode,'Braided: ' .. braided)
	printif(debugMode,'Entrance: ' .. tostring(entrance))
	printif(debugMode,'Exit: ' .. tostring(exit))
	
	for y = 1, height do
		maze[y] = {}
		for x = 1, width do
			maze[y][x] = {Visited = false, North = false, East = false, South = false, West = false}
		end
	end
	
	local startX = calc:Roll(1,width)
	local startY = calc:Roll(1,height)
	
	startX = 1
	startY = 1
	
	buildMazeInternal(startX,startY,maze,braided,entrance,exit)
	
	return maze
end

--[[printMaze(buildMaze(50), true)--]]