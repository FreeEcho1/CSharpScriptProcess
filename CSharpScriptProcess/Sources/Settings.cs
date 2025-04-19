namespace CSharpScriptProcess;

/// <summary>
/// 設定
/// </summary>
public class Settings
{
    /// <summary>
    /// スクリプトファイルのディレクトリのパス
    /// </summary>
    public string ScriptFileDirectoryPath { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Settings()
    {
        ScriptFileDirectoryPath = "";
    }
}
