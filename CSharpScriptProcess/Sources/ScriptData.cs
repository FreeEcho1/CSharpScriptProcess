namespace CSharpScriptProcess;

/// <summary>
/// スクリプトデータ
/// </summary>
public class ScriptData
{
    /// <summary>
    /// スクリプトの文字列
    /// </summary>
    public string ScriptString { get; set; } = "";
    /// <summary>
    /// スクリプト
    /// </summary>
    public Script<object> Script { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="scriptString">スクリプトの文字列</param>
    public ScriptData(
        string scriptString
        )
    {
        ScriptString = scriptString;
        Script = CSharpScript.Create(ScriptString, globalsType: typeof(ScriptUseData));
    }
}
