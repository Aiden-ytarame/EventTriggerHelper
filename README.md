# **Project Arrhythmia Event Trigger Helper**

**Work In Progress**

A mod used to facilitate making new Custom Events, and using them with other Custom Events mods.

The main point of this is to make sure there isnt any problems when using multiple mods that add new events.

If you're using a mod that makes a new event that doesnt use this mod, it might break some things.

Its meant to be used with BepInEx 6.0.0 il2cpp bleeding Edge build, tho I didnt test if the non bleeding edge build works.

(The mod is called trigger helper and yet has 0 support for custom triggers, just events. bruh)

## **Instalation**
**(WIP)**

* Download the [BepInEx 6.0.0 il2cpp bleeding Edge build.](https://builds.bepinex.dev/projects/bepinex_be)
* Follow the BepInEx installation guide available on their [Github](https://github.com/BepInEx/BepInEx).
* Put the TriggersAPI.dll file in the generated BepInNex->Plugins folder.


## Development

Set this plugin as a dependency in your plugin with:

[BepInDependency("me.ytarame.TriggerHelper")]



### Making a new Event

 Make a new Class Inheriting from the abstract class CustomEvent, and implement it.

**EventName:** Its Name of your Event, it's also the the Enum entry. Because of that, the name should follow the rules of naming variables, such as no spaces or not starting with a number

Example: Spawn_Prefab 

**EventTriggered(List<string> data):** Is the function called when the event is triggered. The Param [data] is the data defined by the user in the level editor



### Registering a new CustomEvent

To register your custom Event call TriggerAPI.RegisterTriggerEvents.RegisterCustomEvent() from your Plugin.Load() function.

#### Params

* **customEvent:** Pass in an instance of a class that inherits from the abstract class CustomEvent.
*  **eventData:** Its the Event Data field names that show up in the Level Editor.
  
Example: RegisterCustomEvent(new ExampleLogEvent(), new List{string}(){"Message to Log"});

### Making a new Trigger

Triggers work by calling GameManager2.CallEvent(Trigger) when they want to. the function is called for every trigger using 

In the example "Player_Moved" it calls the function CallEvent when a player just started moving. 

### Registering a new Trigger

## Usage

Just have this with any other mods that uses this mod to register their Events. you should be able to use their events without any overlap issues.

