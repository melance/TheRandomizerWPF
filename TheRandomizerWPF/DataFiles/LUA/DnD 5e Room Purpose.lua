local inspect = require('inspect')

function Pick(values)
	-- index = calc:Rnd(1, table.getn(values))
	index = math.random(1, table.getn(values))
	return values[index]
end

local structType = {}

function structType:Init()
	structType.General = {}
	structType.DeathTrap = {}
	structType.Lair = {}
	structType.Maze = {}
	structType.Mine = {}
	structType.PlanarGate = {}
	structType.StrongHold = {}
	structType.Temple = {}
	structType.Tomb = {}
	structType.TreasureVault = {}

	structType.General['001'] = 'Antechamber'
	structType.General['003'] = 'Armory'
	structType.General['004'] = 'Audience chamber'
	structType.General['005'] = 'Aviary'
	structType.General['007'] = 'Banquet room'
	structType.General['010'] = 'Barracks'
	structType.General['011'] = Pick({'Bath','Latrine'})
	structType.General['012'] = 'Bedroom'
	structType.General['013'] = 'Bestiary'
	structType.General['016'] = 'Cell'
	structType.General['017'] = 'Chantry'
	structType.General['018'] = 'Chapel'
	structType.General['020'] = 'Cistern'
	structType.General['021'] = 'Classroom'
	structType.General['022'] = 'Closet'
	structType.General['024'] = 'Conjuring room'
	structType.General['026'] = 'Court'
	structType.General['029'] = 'Crypt'
	structType.General['031'] = 'Dining room'
	structType.General['033'] = 'Divination room'
	structType.General['034'] = 'Dormitory'
	structType.General['035'] = 'Dressing room'
	structType.General['036'] = Pick({'Entry room','Vestibule'})
	structType.General['038'] = 'Gallery'
	structType.General['040'] = 'Game room'
	structType.General['043'] = 'Guardroom'
	structType.General['045'] = 'Hall'
	structType.General['047'] = 'Great hall'
	structType.General['049'] = 'Hallway'
	structType.General['050'] = 'Kennel'
	structType.General['052'] = 'Kitchen'
	structType.General['054'] = 'Laboratory'
	structType.General['057'] = 'Library'
	structType.General['059'] = 'Lounge'
	structType.General['060'] = 'Meditation chamber'
	structType.General['061'] = 'Observatory'
	structType.General['062'] = 'Office'
	structType.General['064'] = 'Pantry'
	structType.General['065'] = 'Pen'
	structType.General['066'] = 'Prison'
	structType.General['068'] = 'Reception room'
	structType.General['070'] = 'Refectory'
	structType.General['071'] = 'Robing room'
	structType.General['072'] = 'Salon'
	structType.General['074'] = 'Shrine'
	structType.General['076'] = 'Sitting room'
	structType.General['078'] = 'Smithy'
	structType.General['079'] = 'Stable'
	structType.General['081'] = 'Storage room'
	structType.General['083'] = Pick({'Strong room','Vault'})
	structType.General['085'] = 'Study'
	structType.General['088'] = 'Temple'
	structType.General['090'] = 'Throne room'
	structType.General['091'] = 'Torture chamber'
	structType.General['092'] = 'Training room'
	structType.General['093'] = 'Exercise room'
	structType.General['094'] = 'Trophy room'
	structType.General['095'] = 'Museum'
	structType.General['096'] = 'Waiting room'
	structType.General['097'] = Pick({'Nursery','Schoolroom'})
	structType.General['098'] = 'Well'
	structType.General['100'] = 'Workshop'

	structType.DeathTrap['001'] = Pick({'Antechamber','Waiting room'}) .. ' for spectators'
	structType.DeathTrap['008'] = 'Guardroom fortified against intruders'
	structType.DeathTrap['011'] = 'Vault for holding important treasures, accessible only by a ' .. Pick({'locked', 'secret'}) .. ' door ' .. (calc:Rnd(1,100) <= 75 and ' that is trapped' or '')
	structType.DeathTrap['014'] = 'Room containinga puzzle that must be solved to bypass a trap or monster'
	structType.DeathTrap['019'] = 'Trap designed to ' .. Pick({'kill', 'capture'}) .. ' creatures'
	structType.Structure['020'] = 'Observation room, allowing ' .. Pick({'guards','spectators'}) .. ' to observe creatures moving through the dungeon'
	
	structType.Lair['001'] = 'Armory stocked with weapons and armor'
	structType.Lair['002'] = 'Audience chamber, used to receive guests'
	structType.Lair['003'] = 'Banquet room for important celebrations'
	structType.Lair['004'] = 'Barracks where the lair\'s defenders are quartered'
	structType.Lair['005'] = 'Bedroom, for use by leaders'
	structType.Lair['006'] = 'Chapel where the lair\'s inhabitants worship'
	structType.Lair['007'] = Pick({'Cistern','Well'}) .. ' for drinking water'
	structType.Lair['009'] = 'Guardroom for defenders of the lair'
	structType.Lair['010'] = 'Kennel for ' .. Pick({'pets','guard beasts'})
	structType.Lair['011'] = 'Kitchen for food storage and preperation'
	structType.Lair['012'] = Pick({'Pen','Prison'}) .. ' where captives are held.'
	structType.Lair['014'] = 'Storage, mostly nonperishable goods'
	structType.Lair['015'] = 'Throne room where the lair\'s leaders hold court'
	structType.Lair['016'] = 'Torture chamber'
	structType.Lair['017'] = 'Training and exercise room'
	structType.Lair['018'] = Pick({'Trophy room','Museum'})
	structType.Lair['019'] = Pick({'Latrine','Bath'})
	structType.Lair['020'] = 'Workshop for the construction of weapons, armor, tools, and other goods'
	
	structType.Maze['001'] = 'Conjuring room, used to summon creatures that guard the maze'
	structType.Maze['005'] = 'Guardroom for sentinels that patrol the maze'
	structType.Maze['010'] = 'Lair for guard beasts that patrol the maze'
	structType.Maze['011'] = Pick({'Pen','Prison'}) .. ' accessible only by secret door, used to hold captives condemned to the maze'
	structType.Maze['012'] = 'Shrine dedicated to a god or other entity'
	structType.Maze['014'] = 'Storage for food, as well as tools used by the maze\'s guardians to keep the complex in working order'
	structType.Maze['018'] = 'Trap to ' .. Pick({'confound','kill'}) .. ' those sent into the maze'
	structType.Maze['019'] = 'Well that provides drinking water'
	structType.Maze['020'] = 'Workshop where doors, torch sconces, and other furnishings are repaired and maintained'
	
	structType.Mine['002'] = 'Barracks for miners'
	structType.Mine['003'] = 'Bedroom for a ' .. Pick({'supervisor','manager'})
	structType.Mine['004'] = 'Chapel dedicated to a patron deity of ' .. Pick({'miners','earth','protection'})
	structType.Mine['005'] = 'Cistern providing drinking water for miners'
	structType.Mine['007'] = 'Guardroom'
	structType.Mine['008'] = 'Kitchen used to feed workers'
	structType.Mine['009'] = 'Laboratory used to conduct tests on strange minerals extracted from the mine'
	structType.Mine['015'] = 'Lode where metal ore is mined ' .. (calc:Rnd(1,100) <= 75 and 'that is depleted' or '')
	structType.Mine['016'] = 'Office used by the mine supervisor'
	structType.Mine['017'] = 'Smithy for repairing damaged tools'
	structType.Mine['019'] = 'Storage for tools and other equipment'
	structType.Mine['020'] = Pick({'Strong room','Vault'}) .. ' used to store ore for transport to the surface'
	
	Purpose

end

structType:Init()

print(inspect(structType))

--[[
<item name="General" Weight="1">Antechamber</item>
		<item name="General" Weight="2">Armory</item>
		<item name="General" Weight="1">Audience Chamber</item>
		<item name="General" Weight="1">Aviary</item>
		<item name="General" Weight="2">Banquet Room</item>
		<item name="General" Weight="3">Barracks</item>
		<item name="General" Weight="1">Bath or latrine</item>
		<item name="General" Weight="1">Bedroom</item>
		<item name="General" Weight="1">Bestiary</item>
		<item name="General" Weight="3">Cell</item>
		<item name="General" Weight="1">Chantry</item>
		<item name="General" Weight="1">Chapel</item>
		<item name="General" Weight="2">Cistern</item>
		<item name="General" Weight="1">Classroom</item>
		<item name="General" Weight="1">Closet</item>
		<item name="General" Weight="2">Conjuring room</item>
		<item name="General" Weight="2">Court</item>
		<item name="General" Weight="3">Crypt</item>
		<item name="General" Weight="2">Dining room</item>
		<item name="General" Weight="2">Divination room</item>
		<item name="General" Weight="1">Dormitory</item>
		<item name="General" Weight="1">Dressing room</item>
		<item name="General" Weight="1">Entry room or vestibule</item>
		<item name="General" Weight="2">Gallery</item>
		<item name="General" Weight="2">Game room</item>
		<item name="General" Weight="3">Guardroom</item>
		<item name="General" Weight="2">Hall</item>
		<item name="General" Weight="2">Great hall</item>
		<item name="General" Weight="2">Hallway</item>
		<item name="General" Weight="1">Kennel</item>
		<item name="General" Weight="2">Kitchen</item>
		<item name="General" Weight="2">Laboratory</item>
		<item name="General" Weight="3">Library</item>
		<item name="General" Weight="2">Lounge</item>
		<item name="General" Weight="1">Meditation chamber</item>
		<item name="General" Weight="1">Observatory</item>
		<item name="General" Weight="1">Office</item>
		<item name="General" Weight="2">Pantry</item>
		<item name="General" Weight="1">Pen</item>
		<item name="General" Weight="1">Prison</item>
		<item name="General" Weight="2">Reception room</item>
		<item name="General" Weight="2">Refectory</item>
		<item name="General" Weight="1">Robing room</item>
		<item name="General" Weight="1">Salon</item>
		<item name="General" Weight="2">Shrine</item>
		<item name="General" Weight="2">Sitting room</item>
		<item name="General" Weight="2">Smithy</item>
		<item name="General" Weight="1">Stable</item>
		<item name="General" Weight="2">Storage room</item>
		<item name="General" Weight="1">Strong room</item>
		<item name="General" Weight="1">Vault</item>
		<item name="General" Weight="2">Study</item>
		<item name="General" Weight="3">Temple</item>
		<item name="General" Weight="2">Throne room</item>
		<item name="General" Weight="1">Torture chamber</item>
		<item name="General" Weight="1">Training room</item>
		<item name="General" Weight="1">Exercise room</item>
		<item name="General" Weight="1">Trophy room</item>
		<item name="General" Weight="1">Museum</item>
		<item name="General" Weight="1">Waiting room</item>
		<item name="General" Weight="1">Nursery or schoolroom</item>
		<item name="General" Weight="1">Well</item>
		<item name="General" Weight="3">Workshop</item>
]]--