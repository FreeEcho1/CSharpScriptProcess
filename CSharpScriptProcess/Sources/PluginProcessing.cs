namespace CSharpScriptProcess;

public class PluginProcessing : IPlugin
{
    /// <summary>
    /// Disposed
    /// </summary>
    private bool Disposed;
    /// <summary>
    /// プラグインの名前
    /// </summary>
    public string PluginName { get; } = "CSharpScriptProcess";
    /// <summary>
    /// ウィンドウが存在するかの値
    /// </summary>
    public bool IsWindowExist { get; } = true;
    /// <summary>
    /// ウィンドウハンドルがウィンドウの場合のみイベント処理 (処理しない「false」/処理する「true」)
    /// </summary>
    public bool IsWindowOnlyEventProcessing { get; } = true;
    /// <summary>
    /// 取得するウィンドウイベントの種類 (なし「0」)
    /// </summary>
    public GetWindowEventType GetWindowEventType
    {
        get
        {
            return GetWindowEventType.Foreground | GetWindowEventType.MoveSizeStart
                | GetWindowEventType.MoveSizeEnd | GetWindowEventType.MinimizeStart
            | GetWindowEventType.MinimizeEnd | GetWindowEventType.Create | GetWindowEventType.Destroy
            | GetWindowEventType.Show | GetWindowEventType.Hide | GetWindowEventType.LocationChange
            | GetWindowEventType.NameChange;
        }
    }
    /// <summary>
    /// 取得するウィンドウイベントの種類の変更イベントのデータ
    /// </summary>
    public ChangeGetWindowEventTypeData ChangeGetWindowEventTypeData
    {
        get;
    } = new();
    /// <summary>
    /// イベント処理のデータ
    /// </summary>
    public EventProcessingData EventProcessingData
    {
        get;
    } = new();

    /// <summary>
    /// 設定ウィンドウ
    /// </summary>
    private SettingsWindow? SettingsWindow;

    /// <summary>
    /// クラス名の最大文字数
    /// </summary>
    private static int ClassNameMaxLength { get; } = 256;
    /// <summary>
    /// ファイル名の最大文字数
    /// </summary>
    private static int FileNameMaxLength { get; } = 256;
    /// <summary>
    /// プラグインファイルの拡張子
    /// </summary>
    private static string PluginFileExtension = ".csx";

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PluginProcessing()
    {
    }

    /// <summary>
    /// デストラクタ
    /// </summary>
    ~PluginProcessing()
    {
        Dispose(false);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// 非公開Dispose
    /// </summary>
    /// <param name="disposing">disposing</param>
    protected virtual void Dispose(
        bool disposing
        )
    {
        if (Disposed)
        {
            return;
        }
        if (disposing)
        {
            Destruction();
        }
        Disposed = true;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="settingDirectory">設定のディレクトリ</param>
    /// <param name="language">言語</param>
    public void Initialize(
        string settingDirectory,
        string language
        )
    {
        PluginData.SettingDirectoryPath = settingDirectory;
        SettingFileProcessing.ReadSettings();
        ReadScriptFiles();
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Destruction()
    {
        if (SettingsWindow != null)
        {
            SettingsWindow.Close();
            SettingsWindow = null;
        }

        ChangeGetWindowEventTypeData.DoChangeEventType(0);
        EventProcessingData.EventProcessing -= EventProcessingData_EventProcessing;
        PluginData.ScriptData.Clear();
    }

    /// <summary>
    /// ウィンドウを表示
    /// </summary>
    public void ShowWindow()
    {
        if (SettingsWindow == null)
        {
            SettingsWindow = new(this);
            SettingsWindow.Closed += Window_Closed;
            SettingsWindow.Show();
        }
    }

    /// <summary>
    /// ウィンドウの「Closed」イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Closed(
        object? sender,
        EventArgs e
        )
    {
        try
        {
            SettingsWindow = null;
        }
        catch
        {
        }
    }

    /// <summary>
    /// 「EventProcessing」イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EventProcessingData_EventProcessing(
        object? sender,
        EventProcessingArgs e
        )
    {
        try
        {
            if (PluginData.ScriptData.Count == 0)
            {
                return;
            }

            string titleName = "";      // タイトル名
            string className = "";      // クラス名
            string fileName = "";       // ファイル名

            // タイトル名取得
            try
            {
                int length = NativeMethods.GetWindowTextLength(e.Hwnd) + 1;
                StringBuilder getString = new(length);
                _ = NativeMethods.GetWindowText(e.Hwnd, getString, getString.Capacity);
                titleName = getString.ToString();
            }
            catch
            {
            }

            // クラス名取得
            try
            {
                StringBuilder getString = new(ClassNameMaxLength);
                _ = NativeMethods.GetClassName(e.Hwnd, getString, getString.Capacity);
                className = getString.ToString();
            }
            catch
            {
            }

            // ファイル名取得
            try
            {
                _ = NativeMethods.GetWindowThreadProcessId(e.Hwnd, out int id);       // プロセスID
                IntPtr process = NativeMethods.OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead, false, id);
                if (process != IntPtr.Zero)
                {
                    if (NativeMethods.EnumProcessModules(process, out IntPtr pmodules, (uint)Marshal.SizeOf(typeof(IntPtr)), out _))
                    {
                        StringBuilder getString = new(FileNameMaxLength);
                        _ = NativeMethods.GetModuleFileNameEx(process, pmodules, getString, getString.Capacity);
                        fileName = getString.ToString();
                    }
                    NativeMethods.CloseHandle(process);
                }
            }
            catch
            {
            }

            NativeMethods.GetWindowRect(e.Hwnd, out RECT windowRect);
            ScriptUseData scriptUseData = new(e.Hwnd, titleName, className, fileName, windowRect.Left, windowRect.Top, windowRect.Right, windowRect.Bottom, e.EventType);

            foreach (ScriptData scriptData in PluginData.ScriptData)
            {
                Task<ScriptState<object>> scriptStateTask = scriptData.Script.RunAsync(scriptUseData);
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// スクリプトファイルを読み込む
    /// </summary>
    public void ReadScriptFiles()
    {
        PluginData.ScriptData.Clear();
        EventProcessingData.EventProcessing -= EventProcessingData_EventProcessing;

        GetWindowEventType eventType = 0;

        if (string.IsNullOrEmpty(PluginData.Settings.ScriptFileDirectoryPath) == false)
        {
            if (Directory.Exists(PluginData.Settings.ScriptFileDirectoryPath))
            {
                try
                {
                    IEnumerable<string> paths = Directory.EnumerateFiles(PluginData.Settings.ScriptFileDirectoryPath, "*" + PluginFileExtension, SearchOption.TopDirectoryOnly);

                    if (paths.Any())
                    {
                        foreach (string path in paths)
                        {
                            using StreamReader sr = new(path);
                            PluginData.ScriptData.Add(new(sr.ReadToEnd()));
                        }
                        eventType = GetWindowEventType;
                        EventProcessingData.EventProcessing += EventProcessingData_EventProcessing;
                    }
                }
                catch
                {
                    MessageBox.Show("スクリプトファイルの読み込み、初期化に失敗しました。", "CSharpScriptProcess - エラー");
                }
            }
            else
            {
                MessageBox.Show("スクリプトファイルのディレクトリが存在しません。", "CSharpScriptProcess - エラー");
            }
        }

        ChangeGetWindowEventTypeData.DoChangeEventType(eventType);
    }
}
