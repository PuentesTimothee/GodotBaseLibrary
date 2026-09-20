# Godot Base Library

## Overview

A repository useable as a Template in order to kickstart [Godot Project](https://godotengine.org/fr/) faster.
Work with **4.7.2** (Tested) and laters (Untested for now)

The repository make a plugin that need enabling in your project settings. Useable outside of Repository template.

## Gameplay

  - **Mainscene**:
    - *Inheritable*
    - Root class set it in your project as the Root Node. Manage the rest of the Plugins subsystems

  - **GameModeBase**:
    - *Inheritable* 
    - Gameplay brick that can swaped around with your MainScene.

## Datas

  - **GameDatasManager**:
    - *Sealed*
    - This class is loaded on start-up and after the **TagsManager** & **GameSettings**
    - Extend this class and use AddManager to easily Load data from disk from your Singletons
  
  - **Singleton** & **DataSingleton**
    - *Inheritable* 
    - A **Node** based singleton system. Use the Data version for additional compatibility with the GameDatasManagers

  - **RawSingleton**
    - *Inheritable* 
    - Same as before minus the Node part

## Tags

  - **Tag**:
    - Sealed.
    - Use this to represent, compare premade StringName. Better to use than magic Number in editor and code.
    - Use RequestTag and NEVER Create one by hand

  - **TagEditor**:
    - A editor dock to easily manage/add/remove yours tags

## Inputs

  -InputNode
    - *Inheritable*
    - Node instantiated in the Main Scene taking care of most of the Gameplay Input
    
  -InputHandler
    - *Inheritable* 
    - Ressource loaded to represent the default behaviour to have
    - Possibility to register specific input to each instance and swap them around as you wish (Like a Menu and a Gameplay handler swapped arround as you swap the GameMode)
    - Can be hot swapped
  
  -InputList
    - *Override this*
    - Represent in a enum all the possible Input of your game

## Settings
  
  - GameSettings
    - *Inheritable* -> Override *_InitOptions* and call *_AddSettingFromTag* to add your own options
    - Use to Load&Save option in a XML  

  - GameSingleSetting
    - *Inheritable*
    - Use this class or one of it's parent to implement your custom settings

## UI

  -  **CW_Control**
    - The parent of most widgets

  - **CW_MenuManger**
    - Root of all menu
    - Make a scene out of it and add all of your menu With the associate **Tag**
   
  - **CW_Activatable**
    - Represent part of the UI that can be toggle On/Off Like a menu, a dropdown, a "More Details part"
  - **CW_MenuContainer**
    - Represent a menu

  - **CW_ListObjectContainer**
    - Represent any list with Data, you can init the list with any singular or list of **Godot.Object**
  - **CW_Header**
    - Header of menu for Menu title/CloseControl/Anything your heart desire.
  - **CW_Footer**
    - Display all active Shortcut & Action

## Helpers
  See the files for all functions
