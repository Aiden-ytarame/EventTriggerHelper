# **Project Arrhythmia Event Trigger Helper**

**Work In Progress**

A mod used to facilitate making new Custom Events and Triggers. Also to use them with other Custom Events/Trigger mods.

The main point of this is to make sure there isnt any problems when using multiple mods that add new events.

If you're using a mod that makes a new event that doesnt use this mod, it might break some things.

Used with BepInEx 6.X.X il2cpp win64 bleeding Edge build (most recent build)

I dont think anyone will ever use this tbh


## **Instalation**
**(WIP)**

* Download the [BepInEx 6.0.0 il2cpp bleeding Edge build.](https://builds.bepinex.dev/projects/bepinex_be)
* Follow the BepInEx installation guide available on their [Github](https://github.com/BepInEx/BepInEx).
* Put the TriggersAPI.dll file in the generated BepInEx->Plugins folder.


## Development

Set this plugin as a dependency in your plugin with:

[BepInDependency("me.ytarame.TriggerHelper")]



### Making a new Event

 Make a new Class Inheriting from the abstract class CustomEvent, and implement it.

**EventName:** Its Name of your Event, it's also the the Enum entry. Because of that, the name should follow the rules of naming variables, such as no spaces or not starting with a number.

Example: Spawn_Prefab 

**EventTriggered(List<string> data):** Is the function called when the event is triggered. The Param [data] is the data defined by the user in the level editor.



### Registering a new CustomEvent

To register your custom Event call, **TriggerAPI.RegisterTriggerEvents.RegisterCustomEvent(CustomEvent, List<string>)** from your **Plugin.Load()** function.

#### Params

* **customEvent:** Pass in an instance of a class that inherits from the abstract class CustomEvent.
* **eventData:** Its the Event Data field names that show up in the Level Editor.
  
Example: RegisterCustomEvent(new ExampleLogEvent(), new List{string}(){"Message to Log"});

### Making a new Trigger

Triggers work by calling **TriggerAPI.RegisterTriggerEvents.CallCustomTrigger(string)** pretty much when they want to. this function calls GameManager.CallEvent() for each event bound to your trigger

* **triggerName:** name of the trigger to be triggered.
  
example: CallCustomTrigger("Player_Moved");


### Registering a new Trigger
You register your triggers so it shows up in the level editor and gets saved.

To register your custom Trigger, call **TriggerAPI.RegisterTriggerEvents.RegisterCustomTrigger(NameOfYourTrigger)** from your **Plugin.Load()** function.

Example: RegisterCustomTrigger("Player_Moved");

**Note:** The name of your Trigger, is also the the Enum entry. Because of that, the name should follow the rules of naming variables, such as no spaces or not starting with a number.

Theres example triggers and events in this repository as well.

## Usage

Just have this with any other mods that uses this mod to register their Events. you should be able to use their events without any overlap issues.


