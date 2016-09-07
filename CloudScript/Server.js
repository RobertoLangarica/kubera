//Actualiza los niveles pasados por el usuario solo de manera incremental
handlers.updateUserLevels = function(args, context)
{
  var world_keys = [];//Keys para hacer la query de datos existentes
  var incomming_worlds = [];//mundos entrantes filtrados en una lista
  var existing_worlds = [];//mundos existentes filtrados en una lista

  //Filtrando los mundos entrantes
  var keys = Object.keys(args);
  for (var i = 0; i < keys.length; i++)
  {
    if(String(keys[i]).includes("world_") && String(keys[i]) != "world_count")
    {
      world_keys.push(String(keys[i]));
      incomming_worlds.push(args[keys[i]]);
      incomming_worlds[incomming_worlds.length-1].world_key = String(keys[i]);
    }
  }

//Basado en los mundos entrantes nos traemos los existentes
	var currPlayer = server.GetUserData({
    PlayFabId: currentPlayerId,
    Keys: world_keys
  });

  //Filtrando los mundos existentes en una lista
  keys = Object.keys(currPlayer.Data);
  for (var i = 0; i < keys.length; i++)
  {
    if(String(keys[i]).includes("world_"))
    {
      existing_worlds.push(JSON.parse(currPlayer.Data[keys[i]].Value));
      existing_worlds[existing_worlds.length-1].world_key = String(keys[i]);
    }
  }

  //Actualizamos las estadisticas para los leaderboards por nivel
  updateLevelsStatistics(existing_worlds, incomming_worlds);

  //Obtenemos solo el upgrade de mundos
  var toSave = getUpgradedWorlds(existing_worlds, incomming_worlds);

  //Hay algo que guardar?
  if(Object.keys(toSave).length > 0)
  {
    //Guardamos los nuevos mundos
    var updatedUserDataResult = server.UpdateUserData({
      PlayFabId: currentPlayerId,
      Data: toSave,
      Permission:"Public"
    });

    //Error en el guardado?
    if(!updatedUserDataResult)
    {
    	//ERROR!
      return {error:true}
    }
	else
    {
    //Mandamos la nueva version
    return {DataVersion:updatedUserDataResult["DataVersion"], error: false}
    }
  }
	else
  {
    //No hay cambios y respondemos con la misma version
    return {DataVersion:currPlayer["DataVersion"], error: false}
  }
}

/**
* Filtra los mundos entrantes contra los existentes y devuelve una lista con un
* merge a la version mas actualizada de los mundos
* @params existing_worlds:[], existing_worlds:[]
**/
function getUpgradedWorlds(existing_worlds, incomming_worlds)
{
  var result = {};
  var added_worlds = [];//Vamos almacenando un record de los agregados

  //Hacemos un merge de los mundos entrantes vs los existentes
  for(var i = 0; i < incomming_worlds.length; i++)
  {
    var world = getItemById(existing_worlds, incomming_worlds[i].id);

    if(world != null)
    {
      //Se actualiza algun nivel del mundo
      if(anyLevelIsUpgraded(world.levels, incomming_worlds[i].levels))
      {
          //Se obtiene un merge de ambos mundos
          world = getMergedWorld(world, incomming_worlds[i]);
          result[incomming_worlds[i].world_key] = JSON.stringify(world);
      }
    }
    else
    {
      //No existe asi que lo guardamos (sin basura)
      var key = incomming_worlds[i].world_key;
      delete incomming_worlds[i].world_key;
      world = incomming_worlds[i];
      result[key] = JSON.stringify(world);
    }

    added_worlds.push(world);
  }

  //Los existentes que no tuvieron merge tambien se agregan para no perderlos
  for(var i = 0; i < existing_worlds.length; i++)
  {
    var world = getItemById(added_worlds, existing_worlds[i].id);

    if(world == null)
    {
      //No existe asi que lo guardamos (world_key solo es un helper)
      var key = existing_worlds[i].world_key;
      delete existing_worlds[i].world_key;
      world = existing_worlds[i];
      result[key] = JSON.stringify(world);
    }
  }

  return result;
}

/**
* Evalua los niveles entrantes contra la lista de existentes y determina
* si alguno recibe un upgrade, si un nivel entrante no existe
* se toma como upgrade
* @params existing_levels:[], incomming_levels:[]
*/
function anyLevelIsUpgraded(existing_levels, incomming_levels)
{
  for(var i = 0; i < incomming_levels.length; i++)
  {
    var level = getItemById(existing_levels, incomming_levels[i].id);

    if(level != null)
    {
      if(levelIsUpgraded(level, incomming_levels[i]))
      {
        return true;
      }
    }
    else
    {
      //No existe el nivel asi que es un upgrade completo
      return true;
    }
  }

  return false;
}

/**
* Evalua el nivel existente vs el entrante para ver si el entrante representa
* un upgrade en algun valor
*  @params existing:{stars:int,points:int,passed:bool,locked:bool},
*          incomming:{stars:int,points:int,passed:bool,locked:bool}
**/
function levelIsUpgraded(existing, incomming)
{
  if(	   incomming.stars	> existing.stars
      || incomming.points	> existing.points
      || (incomming.passed	&& !existing.passed)
      || (!incomming.locked	&& existing.locked) )
  {
    return true;
  }

  return false;
}

function getMergedWorld(existing, incomming)
{
  var result = {};
  result.id       = existing.id;
  result.version  = existing.version + 1;
  result.levels   = [];

  var added_levels = [];

  for(var i = 0; i < incomming.levels.length; i++)
  {
    var level = getItemById(existing.levels, incomming.levels[i].id);

    if(level != null)
    {
      //se agrega un merge
      level = getMergedLevel(level, incomming.levels[i]);
      result.levels.push(level);
    }
    else
    {
      //se agrega
      level = incomming.levels[i];
      result.levels.push(level);
    }

    added_levels.push(level);
  }

  //Los niveles preexistentes no agregados se agregan
  for(var i = 0; i < existing.levels.length; i++)
  {
    var level = getItemById(added_levels, existing.levels[i].id);

    if(level == null)
    {
      //se agrega
      result.levels.push(existing.levels[i]);
    }
  }

  return result;
}

function getMergedLevel(existing, incomming)
{
  var level = {};
  level.id      = existing.id;
  level.version = existing.version + 1;
  level.stars   = incomming.stars > existing.stars ? incomming.stars:existing.stars;
  level.points  = incomming.points > existing.points ? incomming.points:existing.points;
  level.passed  = (incomming.passed || existing.passed);
  level.locked  = !(!incomming.locked || !existing.locked);
  level.world   = existing.world;

  return level;
}

/**
* Actualiza las estadisticas de este jugador basadas en los mundos que vienen
**/
function updateLevelsStatistics(existing_worlds, incomming_worlds)
{
  //Solo nos interesan los niveles que aumentan sus puntos
  var upgradedLevels = {};

  //Hacemos un merge de los mundos entrantes vs los existentes
  for(var i = 0; i < incomming_worlds.length; i++)
  {
    var world = getItemById(existing_worlds, incomming_worlds[i].id);

    if(world != null)
    {
       //Se obtienen los niveles que tienen upgrade de puntos
      var levels = getUpgradedLevelsByPoints(world, incomming_worlds[i]);
      for(var j = 0; j < levels.length; j++)
      {
        upgradedLevels["level_"+levels[j].id] = levels[j].points;
      }

    }
    else
    {
      //No existe asi que sus niveles todos se guardan
      for(var j = 0; j < incomming_worlds[i].levels.length; j++)
      {
        upgradedLevels["level_"+incomming_worlds[i].levels[j].id] = incomming_worlds[i].levels[j].points;
      }

    }
  }

  //Guardamos si es necesario
  var keys = Object.keys(upgradedLevels);
  if(keys.length > 0)
  {
    server.UpdateUserStatistics (
      {
          PlayFabId: currentPlayerId,
          UserStatistics: upgradedLevels
      });
  }
}

function getUpgradedLevelsByPoints(existing, incomming)
{
  var result = [];

  for(var i = 0; i < incomming.levels.length; i++)
  {
    var level = getItemById(existing.levels, incomming.levels[i].id);

    if(level != null)
    {
      //se agrega solo el entrante si tiene mas puntos
      if(level.points < incomming.levels[i].points)
      {result.push(incomming.levels[i]);}
    }
    else
    {
      //se agrega
      result.push(incomming.levels[i]);
    }
  }

  return result;
}

function getItemById(list,id)
{
	for(var i = 0; i < list.length; i++)
    {
    	if(list[i].id == id)
        {
        	return list[i];
        }
    }
  return null;
}
