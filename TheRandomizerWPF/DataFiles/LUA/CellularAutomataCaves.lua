paramerror = false

if tonumber(xsize) == nil then
	print("<span style='color:red'>Width must be a number.  Value provided was '" .. xsize .. "'.</span>")
	paramerror = true
end
if tonumber(ysize) == nil then
	print("<span style='color:red'>Height must be a number.  Value provided was '" .. ysize .. "'.</span>")
	paramerror = true
end
if tonumber(fill) == nil then
	print("<span style='color:red'>Clear must be a number.  Value provided was '" .. fill .. "'.</span>")
	paramerror = true
end
if tonumber(csize) == nil then
	print("<span style='color:red'>Cell size must be a number.  Value provided was '" .. csize .. "'.</span>")
	paramerror = true
end

if tonumber(fill) < 1 then 
	print("<span style='color:red'>Clear must be greater than or equal to 1.  Value provided was '" .. fill .. "'.</span>")
	paramerror = true
end
if tonumber(fill) > 99 then 
	print("<span style='color:red'>Clear must be less than or equal to 99.  Value provided was '" .. fill .. "'.</span>")
	paramerror = true
end
if tonumber(xsize) < 10 then
	print("<span style='color:red'>Width must be greater than or equal to 10.  Value provided was '" .. xsize .. "'.</span>")
	paramerror = true
end
if tonumber(ysize) < 10 then 
	print("<span style='color:red'>Height must be greater than or equal to 10.  Value provided was '" .. ysize .. "'.</span>")
	paramerror = true
end
if tonumber(csize) < 6 then
	print("<span style='color:red'>Cell size must be greater than or equal to 6.  Value provided was '" .. csize .. "'.</span>")
end
if tonumber(csize) > 72 then
	print("<span style='color:red'>Cell size must be less than or equal to 72.  Value provided was '" .. csize .. "'.</span>")
end

if paramerror then return end


fill = fill / 100

debug = true

grid = {}

HALL_VALUE = 0
WALL_VALUE = 1

repeat1 = 4
repeat2 = 3

r1cutoff1 = 5
r2cutoff1 = 2

r1cutoff2 = 5
r2cutoff2 = -1

function processGrid(count, cutoff1, cutoff2)
	for c = 1, count do
		local ngrid = {}
		for x = 1, xsize do
			ngrid[x] = {}
			for y = 1, ysize do
				ngrid[x][y] = HALL_VALUE
			end
		end
		for y = 1, ysize do
			for x = 1, xsize do
				local curr1 = 0
				for curry = -1 + y, 1 + y do
					for currx = -1 + x, 1 + x do
						if grid[currx] == nil or grid[currx][curry] == nil or grid[currx][curry] == 1 then
							curr1 = curr1 + 1
						end
					end
				end
				if curr1 >= cutoff1 then
					ngrid[x][y] = WALL_VALUE
				else
					local curr2 = 0
					for currx = -2 + x, 2 + x do
						for curry = -2 + y, 2 + y, 4 do
							if grid[currx] == nil or grid[currx][curry] == nil or grid[currx][curry] == 1 then
								curr2 = curr2 + 1
							end
						end
					end
					for currx = -2 + x, 2 + x, 4 do
						for curry = -1 + y, 1 + y do
							if grid[currx] == nil or grid[currx][curry] == nil or grid[currx][curry] == 1 then
								curr2 = curr2 + 1
							end
						end
					end
					if curr2 <= cutoff2 then
						ngrid[x][y] = WALL_VALUE
					end
				end
			end
		end
		for x = 1, xsize do
			for y = 1, ysize do
				grid[x][y] = ngrid[x][y]
			end
		end
	end
end

-- populate the grid
hallcount = 0
area = xsize * ysize
for x = 1, xsize do
	grid[x] = {}
	for y = 1, ysize do
		grid[x][y] = WALL_VALUE
	end
end

repeat
	local x = calc:Rnd(1, tonumber(xsize))
	local y = calc:Rnd(1, tonumber(ysize))
	
	if grid[x][y] ~= HALL_VALUE then
		grid[x][y] = HALL_VALUE
		hallcount = hallcount + 1
	end
until hallcount / area >= fill

-- Process the grid using cellular automoton
processGrid(repeat1, r1cutoff1, r2cutoff1)
processGrid(repeat2, r1cutoff2, r2cutoff2) 

-- Create the display
tableWidth = (csize * xsize)
tableHeight = (csize * ysize)

if gridlines == "True" then
	tableWidth = tableWidth + xsize
	tableHeight = tableHeight + ysize
end

map = "<table style='width:" .. tableWidth .. "px; height:" .. tableHeight .. "px;border-collapse: collapse;'>\n"

for y = 1, ysize do
	map = map .. "<tr style='height:" .. csize .. "px;'>\n"
	for x = 1, xsize do
		map = map .. "<td style='width:" .. csize .. "px;"

		if grid[x][y] == 1 then
			map = map .. "background: black;"
			if gridlines == "True" then
				map = map .. "border: 1px solid white;"
			end
		elseif grid[x][y] == 0 then
			map = map .. "background: white;"
			if gridlines == "True" then
				map = map .. "border: 1px solid black;"
			end
		end
		map = map .. "'>&nbsp;</td>\n"
	end
	map = map .. "</tr>\n"
end

map = map .. "</table>"
print("<div style='font-family: courier; font-size: 1px;'>" .. map .. "</div>")