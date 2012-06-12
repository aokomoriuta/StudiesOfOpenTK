namespace LWisteria.StudiesOfOpenTK.MultiBuffer
{
	/// <summary>
	/// メインクラス
	/// </summary>
	partial class MultiBufferMain : System.Windows.Application
	{
		/// <summary>
		/// 最上位ウインドウ
		/// </summary>
		MainWindow mainWindow;

		/// <summary>
		/// メインクラスを開始
		/// </summary>
		public MultiBufferMain()
		{
			// 最上位ウインドウを作成して設定
			this.mainWindow = new MainWindow();
			base.MainWindow = this.mainWindow;
			base.MainWindow.Show();
		}
	}
}