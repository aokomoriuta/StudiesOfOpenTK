namespace LWisteria.StudiesOfOpenTK.CubeWithShader
{
	/// <summary>
	/// メインクラス
	/// </summary>
	partial class CubeWithShaderMain : System.Windows.Application
	{
		/// <summary>
		/// 最上位ウインドウ
		/// </summary>
		MainWindow mainWindow;

		/// <summary>
		/// メインクラスを開始
		/// </summary>
		public CubeWithShaderMain()
		{
			// 最上位ウインドウを作成して設定
			this.mainWindow = new MainWindow();
			base.MainWindow = this.mainWindow;
			base.MainWindow.Show();
		}
	}
}