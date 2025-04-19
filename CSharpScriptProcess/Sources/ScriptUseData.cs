namespace CSharpScriptProcess;

/// <summary>
/// スクリプトで使用するデータ
/// </summary>
public class ScriptUseData
{
    /// <summary>
    /// ウィンドウハンドル
    /// </summary>
    public IntPtr Handle { get; } = IntPtr.Zero;
    /// <summary>
    /// ウィンドのウタイトル
    /// </summary>
    public string TitleName { get; } = "";
    /// <summary>
    /// ウィンドウのクラス名
    /// </summary>
    public string ClassName { get; } = "";
    /// <summary>
    /// ウィンドウのファイル名
    /// </summary>
    public string FileName { get; } = "";
    /// <summary>
    /// ウィンドウのX
    /// </summary>
    public int X { get; } = 0;
    /// <summary>
    /// ウィンドウのY
    /// </summary>
    public int Y { get; } = 0;
    /// <summary>
    /// ウィンドウの幅
    /// </summary>
    public int Width { get; } = 0;
    /// <summary>
    /// ウィンドウの高さ
    /// </summary>
    public int Height { get; } = 0;
    /// <summary>
    /// イベントの種類
    /// </summary>
    public uint EventType {  get; } = 0;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="handle">ウィンドウハンドル</param>
    /// <param name="titleName">ウィンドのウタイトル</param>
    /// <param name="className">ウィンドウのクラス名</param>
    /// <param name="fileName">ウィンドウのファイル名</param>
    /// <param name="x">ウィンドウのX</param>
    /// <param name="y">ウィンドウのY</param>
    /// <param name="width">ウィンドウの幅</param>
    /// <param name="height">ウィンドウの高さ</param>
    /// <param name="eventType">イベントの種類</param>
    public ScriptUseData(
        IntPtr handle,
        string titleName,
        string className,
        string fileName,
        int x,
        int y,
        int width,
        int height,
        uint eventType
        )
    {
        Handle = handle;
        TitleName = titleName;
        ClassName = className;
        FileName = fileName;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        EventType = eventType;
    }
}
