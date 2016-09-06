//Actualiza los niveles pasados por el usuario solo de manera incremental
handlers.updateUserLevels = function(args, context)
{
  var world_keys = [];
  var incomming_worlds = [];
  var existing_worlds = [];

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

//Obtenemos los mundos existentes
	var currPlayer = server.GetUserData({
    PlayFabId: currentPlayerId,
    Keys: world_keys
  });

  //Filtrando los mundos existentes
  keys = Object.keys(currPlayer.Data);
  for (var i = 0; i < keys.length; i++)
  {
    if(String(keys[i]).includes("world_"))
    {
      existing_worlds.push(JSON.parse(currPlayer.Data[keys[i]].Value));
      existing_worlds[existing_worlds.length-1].world_key = String(keys[i]);
    }
  }

  //Obtenemos solo el upgrade de mundos
  var toSave = getOnlyUpgradedWorlds(existing_worlds, incomming_worlds);

  //Hay algo que guardar?
  if(Object.keys(toSave).length > 0)
  {
    //log.info("SAVE:"+JSON.stringify(toSave));

    //Guardamos los nuevos mundos
    var updatedUserDataResult = server.UpdateUserData({
      PlayFabId: currentPlayerId,
      Data: toSave,
      Permission:"Public"
    });


    if(!updatedUserDataResult)
    {
    	//ERROR!
      return {error:true}
    }
	else
    {
      //Nueva version
    return {DataVersion:updatedUserDataResult["DataVersion"], error: false}
    }
  }
	else
  {
    //No hay cambios y respondemos con la misma version
    return {DataVersion:currPlayer["DataVersion"], error: false}
  }
}

function getOnlyUpgradedWorlds(existing_worlds, incomming_worlds)
{
  var result = {};
  var added_worlds = [];

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

  //Retenemos los preexistentes que no se han agregado todavia
  for(var i = 0; i < existing_worlds.length; i++)
  {
    var world = getItemById(added_worlds, existing_worlds[i].id);

    if(world == null)
    {
      //No existe asi que lo guardamos (sin basura)
      var key = existing_worlds[i].world_key;
      delete existing_worlds[i].world_key;
      world = existing_worlds[i];
      result[key] = JSON.stringify(world);
    }
  }

  return result;
}

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
  //log.info("Mergin world: "+existing.id);
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
