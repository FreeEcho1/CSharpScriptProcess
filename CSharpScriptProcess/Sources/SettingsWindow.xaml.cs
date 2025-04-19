namespace CSharpScriptProcess;

/// <summary>
/// 設定ウィンドウ
/// </summary>
public partial class SettingsWindow : Window
{
    /// <summary>
    /// PluginProcessing
    /// </summary>
    private readonly PluginProcessing PluginProcessing;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SettingsWindow(
        PluginProcessing pluginProcessing
        )
    {
        InitializeComponent();

        PluginProcessing = pluginProcessing;

        ScriptFileDirectoryPathTextBox.Text = PluginData.Settings.ScriptFileDirectoryPath;

        ScriptFileDirectoryPathTextBox.TextChanged += ScriptFileDirectoryPathTextBox_TextChanged;
        SelectScriptFileDirectoryPathButton.Click += SelectScriptFileDirectoryPathButton_Click;
        ReloadScriptButton.Click += ReloadScriptButton_Click;
    }

    /// <summary>
    /// 「Close」イベント
    /// </summary>
    /// <param name="e"></param>
    protected override void OnClosed(
        EventArgs e
        )
    {
        try
        {
            base.OnClosed(e);

            SettingFileProcessing.WriteSettings();
        }
        catch
        {
        }
    }

    /// <summary>
    /// 「スクリプトファイルのディレクトリのパス」TextBoxの「TextChanged」イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ScriptFileDirectoryPathTextBox_TextChanged(
        object sender,
        System.Windows.Controls.TextChangedEventArgs e
        )
    {
        try
        {
            PluginData.Settings.ScriptFileDirectoryPath = ScriptFileDirectoryPathTextBox.Text;
        }
        catch
        {
        }
    }

    /// <summary>
    /// 「スクリプトファイルのディレクトリのパス選択」Buttonの「Click」イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectScriptFileDirectoryPathButton_Click(
        object sender,
        RoutedEventArgs e
        )
    {
        try
        {
            Microsoft.Win32.OpenFolderDialog dialog = new()
            {
                Title = "スクリプトファイルのディレクトリ選択"
            };

            if (dialog.ShowDialog() == true)
            {
                ScriptFileDirectoryPathTextBox.Text = dialog.FolderName;
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// 「スクリプト再読み込み」Buttonの「Click」イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ReloadScriptButton_Click(
        object sender,
        RoutedEventArgs e
        )
    {
        try
        {
            PluginProcessing.ReadScriptFiles();
        }
        catch
        {
        }
    }
}
