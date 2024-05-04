# **Project Arrhythmia Event Trigger Helper**

**Work In Progress**

A mod used to facilitate making new Custom Events and Triggers. Also to use them with other Custom Events/Trigger mods.

The main point of this is to make sure there isnt any problems when using multiple mods that add new events.

If you're using a mod that makes a new event that doesnt use this mod, it might break some things.

Its meant to be used with BepInEx 6.0.0 il2cpp bleeding Edge build, tho I didnt test if the non bleeding edge build works.

I dont think anyone will ever use this tbh


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

**EventName:** Its Name of your Event, it's also the the Enum entry. Because of that, the name should follow the rules of naming variables, such as no spaces or not starting with a number.

Example: Spawn_Prefab 

**EventTriggered(List<string> data):** Is the function called when the event is triggered. The Param [data] is the data defined by the user in the level editor.



### Registering a new CustomEvent

To register your custom Event call, **TriggerAPI.RegisterTriggerEvents.RegisterCustomEvent()** from your **Plugin.Load()** function.

#### Params

* **customEvent:** Pass in an instance of a class that inherits from the abstract class CustomEvent.
*  **eventData:** Its the Event Data field names that show up in the Level Editor.
  
Example: RegisterCustomEvent(new ExampleLogEvent(), new List{string}(){"Message to Log"});

### Making a new Trigger

Triggers work by calling GameManager2.CallEvent(Trigger) pretty much when they want to. the function is called for every trigger using the TriggerType of the custom trigger.

examples:

* Player_Hit trigger binds a lambda(?) of each trigger that uses the EventType Player_Hit to the PlayerHitDelegate of every player. When that delegate is fired, it calls the lambda of every Trigger, that lambda calls GameManager2.CallEvent(Trigger).
  
* Player_Moved CustomTrigger (check IsPlayerMovingTrigger.cs in examples folder) checks every frame for the players velocity. If the velocity just got out of zero it goes trough a list of TriggerEvents that use the Player_Moved trigger and calls GameManager2.CallEvent(Trigger) for each one.

  you get the idea.


### Registering a new Trigger
You register your triggers so it shows up in the level editor and gets saved.

To register your custom Trigger, call **TriggerAPI.RegisterTriggerEvents.RegisterCustomTrigger(NameOfYourTrigger)** from your **Plugin.Load()** function.

Example: RegisterCustomTrigger("Player_Moved");

**Note:** The name of your Trigegr, is also the the Enum entry. Because of that, the name should follow the rules of naming variables, such as no spaces or not starting with a number.


## Usage

Just have this with any other mods that uses this mod to register their Events. you should be able to use their events without any overlap issues.

