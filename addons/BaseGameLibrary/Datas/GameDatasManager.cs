using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.Datas;

//Fill it with all your singleton that you wish initialized on startup
public partial class GameDatasManager : Singleton<GameDatasManager>
{
    private Array<Node> m_AllManager = new Array<Node>();

    private void AddManager<TType>() where TType : Node, new()
    {
        TType sType;
        AddChild(sType = new());
        m_AllManager.Add(sType);
    }
}