# addons/

Deux plugins maison, activés dans Project Settings > Plugins :

| Plugin | Dossier | Rôle |
|---|---|---|
| `BaseGameLibraryPlugin` | `BaseGameLibrary/` | Socle du jeu : données, settings, input, logs, UI de menus |
| `GameplayTags` (`TagEditor`) | `Tags/` | Système de Gameplay Tags (comme les GameplayTags d'Unreal) + outils d'éditeur |


Namespaces : `BaseGameLibrary.*` et `Tags`.

---

## Tags/

### Concepts

- **Tag** (`Tag.cs`) : `Resource` qui stocke une clé hiérarchique dans `[Export] _StringTag` (ex. `spell.fire.01`). Le `TagNode` correspondant est retrouvé à la demande dans le `TagsManager`. Deux `Tag` de même clé sont égaux (`Equals` / `GetHashCode`). Un tag inconnu du manager est invalide (`IsValid()` faux, `FullName()` = `!!Invalid!!`).
- **TagContainer** (`TagContainer.cs`) : `Resource` contenant un `[Export] Array<Tag> _Tags` sans doublon.
- **TagsManager** (`TagsManager.cs`) : autoload `Instance_TagsManager`. Charge l'arbre de tags depuis `res://Datas/Tags.json`, le sauvegarde (`_SaveCurrentTags`) et fournit `TagsManager.RequestTag(clé, ETagFetch)`.
- **TagNode** (`TagNode.cs`) : nœud de l'arbre (clé, parent, enfants). `CompleteTagKey()` donne la clé complète.
- **TagQuery** (`TagQuery.cs`) : `Resource` de requête sur un `TagContainer` (`EQueryType` : any / exact / no match, sur des tags ou des sous-expressions).
- **TagAttribute** (`TagAttributes.cs`) : `[Tag("spell")]` sur un membre `Tag` / `TagContainer` restreint la sélection de l'inspecteur au sous-arbre `spell`.

### Obtenir un tag dans le code

```csharp
Tag tag = Tag.RequestTag("spell.fire.01");                          // ETagFetch.e_Default : tag invalide si inconnu
Tag tag = Tag.RequestTag("menu.main", ETagFetch.e_CreateOnError);   // crée le tag s'il n'existe pas
Tag tag = Tag.RequestTag("menu.main", ETagFetch.e_ThrowOnError);    // TagNotRegisteredException si inconnu
```

Une clé est insensible à la casse (passée en minuscules).

### Sauvegarde

Les membres `[Export] Tag` et `[Export] TagContainer` sont sauvegardés dans les `.tres` / `.tscn` (sous-ressources avec `_StringTag`). Les tags doivent exister dans `Datas/Tags.json` pour être valides au chargement.

Argument de lancement : `--exportTag` réécrit `Tags.json` au démarrage.

### Éditeur (`Tags/editor/`)

| Fichier | Rôle |
|---|---|
| `TagsEditorDock` | Dock « Tags » (haut gauche de la zone droite) : liste l'arbre, `+` préremplit le champ pour créer un enfant, `-` supprime le tag et ses enfants |
| `TagTreeSelector` | Arbre de tags réutilisable, 3 modes : `e_Single` (un tag), `e_Multiple` (plusieurs), `e_Create` (boutons + / - du dock). Échange toujours des clés complètes |
| `TagInspectorControl` | Adaptateur `e_Single` : `SetValue`, événement `On_ValueChanged(Tag)` |
| `TagContainerInspectorControl` | Adaptateur `e_Multiple` : `SetValue`, événement `On_ValueChanged`, liste lisible des tags sélectionnés en dessous |
| `TagEditorProperty` | `EditorProperty` d'un membre `Tag` |
| `TagContainerEditorProperty` | `EditorProperty` d'un membre `TagContainer` |
| `TagInspectorPlugin` | Branche les deux `EditorProperty` ci-dessus sur les membres de type `Tag` / `TagContainer` |
| `QueryExpressionInspectorPlugin` / `QueryExpressionEditorControl` | Éditeur visuel d'un `TagQuery` |
| `AssetRepairTool` | Menu Project > Tools > « Repair assets tags » : parcourt les scènes de `res://` et retire des `TagContainer` les tags invalides |
| `EditorUtils` | Classe vide |

### Licence

Une partie du code vient de Gamesmiths Guild (MIT, voir `Tags/ForgeCopyright.txt`).

---

## BaseGameLibrary/

### Plugin et autoloads

`BaseGameLibraryPlugin` enregistre deux autoloads à l'activation :

- `Instance_GameDatasManager` (`Datas/GameDatasManager.cs`)
- `Instance_GameSettings` (`Settings/GameSettings.cs`)

Il ajoute aussi l'inspecteur de `CW_MenuContainer`. `BasePlugin` fournit `AddCustomInspectorPlugin<T>()` aux deux plugins.

### Datas/

- `Singleton.cs` :
  - `DataNode` : à `_Ready`, essaie `_LoadFromJson()` puis `_LoadFromResource()`.
  - `Singleton<T>` : `DataNode` avec `Instance` statique.
  - `DataSingleton` : `DataNode` géré par `GameDatasManager`.
  - `RawSingleton<T>` : singleton C# simple.
  - `SingletonHelper.PATH_DATA` = `res://Datas`.
- `GameDatasManager` : au démarrage, instancie par réflexion **toute sous-classe concrète de `DataSingleton`** et l'ajoute comme enfant. `GameDatasManager.GetDatas<T>()` retourne le manager voulu (lève `DataSingletonException` s'il n'existe pas).

### Gameplay/ et MainSceneBase

- `GameModeBase` : `_GameModeBegin()` à surcharger, signal `OnGamemodeReady`.
- `MainSceneBase` (`Node3D`) : point d'entrée. Crée l'`InputNode`, instancie le `CW_MenuManager` depuis `_MenuManagerPacked`, expose `MenuManager`, `InputNode`, `GetNakedMode()` et un `RandomNumberGenerator` statique.
  - Arguments de lancement : `--testMode`, `--seed=N`.

### Settings/

- `GameSettings` : valeurs stockées par clé de tag, fichier `Datas/BaseSettings.json`. Accès : `GetSetting<T>(Tag)` et `GetSettingValue<T>(Tag)`. Les tags sont dans `GameSettings_Tags`.
- `GameSingleSetting` : `Resource` abstraite (section, type bool / int / dropdown) qui sait créer son `Control`.
- `GameSettings_Visual` : settings concrets (locale, résolution, type d'affichage, VSync).

### Input/

- `BaseInputNode` : reçoit les événements et les transmet à un `InputHandler`. `_Register(EInputList | string, delegate)` / `_Unregister`.
- `InputHandler` : `Resource` qui associe une action d'input à des délégués (`_UnhandledInput`).
- `EInputList` : liste des actions du jeu.

### Helpers/

| Fichier | Contenu |
|---|---|
| `MyLogger` | Logs par sévérité et type. Arguments : `--log=`, `--logW=`, `--logE=` |
| `Reflection` | `GetEnumerableOfType<T>()`, `GetEnumerableOfTypeCreated<T>()` |
| `TimCollection` | `_FindByPredicate`, `_ForEach`, `_IsEmpty` |
| `TimEnums` | Préfixe `e_` : parsing, traduction d'enums |
| `TimHelpers` | `FindParentOfType`, `Tr_Format`, chargement JSON, texte coloré BBCode, extensions `Tag` |
| `TimMath` | `BaseRange<T>` |
| `TimString` | `_StripBBCode` |

### UI/

- `CW_Control` : `Control` de base, prévenu quand le game mode est prêt.
- `CW_ActivatableContainer` : conteneur qui s'affiche / se masque (`_Activate`, `_Deactivate`, événements `On_Activated` / `On_Deactivated`).
- `CW_MenuContainer` : un menu, identifié par un `Tag` (`GetTag()` à surcharger), avec `CW_Header` et `CW_Footer`.
- `CW_MenuManager` : pile de menus. Indexe ses enfants `CW_MenuContainer` par tag ; `_OpenMenu(Tag)`.
- `CW_ListObjectContainer` : liste générique de widgets (`IControlListWidget` / `IControlListObject`).
- `CW_InputAction`, `CW_Tooltip` : widgets d'input et d'infobulle.
- `Settings/CW_SettingEntry`, `CW_SettingTab` : lignes et onglets du menu de settings. `CW_SettingEntry._CurrentTag` désigne le setting affiché.

### editor/

- `BaseEditorInspectorPlugin` : base des plugins d'inspecteur du projet.
- `MenuContainerInspectorPlugin` / `MenuContainerInspectorControl` : affichent « Current tag: [...] » dans l'inspecteur d'un `CW_MenuContainer`.
- `Ic_Menu.svg` : icône de `CW_MenuContainer`.

### Licence

GPLv3 (`BaseGameLibrary/License.md`). Projet .NET 8 séparé : `ElementGodot.BaseGameLibrary.csproj`.
