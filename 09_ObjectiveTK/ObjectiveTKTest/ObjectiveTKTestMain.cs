using OpenTK;
using OpenTK.Graphics;
namespace LWisteria.StudiesOfOpenTK.ObjectiveTK.Test
{
	/// <summary>
	/// メインクラス
	/// </summary>
	partial class ObjectiveTKTestMain : System.Windows.Application
	{
		/// <summary>
		/// メインクラスを開始
		/// </summary>
		public ObjectiveTKTestMain()
		{
			// 最上位ウインドウを作成して設定
			MainWindow mainWindow = new MainWindow();
			base.MainWindow = mainWindow;

			// 立方体を描画
			mainWindow.Draw(new[]
			{
				new Cube(new Vector3(0, 0, 0), 0.2f, new Color4(255,   0,   0, 255)),
				new Cube(new Vector3(2, 0, 0), 0.4f, new Color4(  0, 255,   0, 255)),
				new Cube(new Vector3(4, 0, 0), 0.6f, new Color4(  0,   0, 255, 255)),
			});

			// 四角錐を描画
			mainWindow.Draw(new[]
			{
				new Pyramid(new Vector3( 0,  0, 2), 0.2f, new Color4(100,   0,   0, 255)),
				new Pyramid(new Vector3(-2,  0, 2), 0.4f, new Color4(  0, 100,   0, 255)),
				new Pyramid(new Vector3(-4,  0, 2), 0.6f, new Color4(  0,   0, 100, 255)),
				new Pyramid(new Vector3( 0, -2, 2), 0.8f, new Color4(100, 100,   0, 255)),
				new Pyramid(new Vector3( 0, -4, 2), 1.0f, new Color4(  0, 100, 100, 255)),
				new Pyramid(new Vector3( 0, -6, 2), 1.2f, new Color4(100,   0, 100, 255)),
			});

			// ウインドウ表示
			base.MainWindow.Show();
		}
	}
}