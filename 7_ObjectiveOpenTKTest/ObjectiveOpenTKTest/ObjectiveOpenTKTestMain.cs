using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LWisteria.StudiesOfOpenTK.ObjectiveOpenTK.Test
{
	/// <summary>
	/// メインクラス
	/// </summary>
	partial class ObjectiveOpenTKTestMain : System.Windows.Application
	{
		/// <summary>
		/// メインクラスを開始
		/// </summary>
		public ObjectiveOpenTKTestMain()
		{
			// 最上位ウインドウを作成して設定
			MainWindow mainWindow = new MainWindow(
					LWisteria.StudiesOfOpenTK.ObjectiveOpenTK.Test.Properties.Resources.vertex,
					LWisteria.StudiesOfOpenTK.ObjectiveOpenTK.Test.Properties.Resources.geometry,
					LWisteria.StudiesOfOpenTK.ObjectiveOpenTK.Test.Properties.Resources.fragment);
			base.MainWindow = mainWindow;

			mainWindow.viewport.R = 100;
			mainWindow.viewport.Theta = 0;
			mainWindow.viewport.Phi = 0;


			// ウインドウを表示
			mainWindow.Show();

			// データを転送
			mainWindow.ProgramGL.WriteBuffer(
				// 立方体配列
				new Cube[]
				{
					new Cube(new Vector3(0, 0, 0), 0.2f, new Color4(255,   0,   0, 255)),
					new Cube(new Vector3(2, 0, 0), 0.4f, new Color4(  0, 255,   0, 255)),
					new Cube(new Vector3(4, 0, 0), 0.6f, new Color4(  0,   0, 255, 255)),
					new Cube(new Vector3(0, 2, 0), 0.8f, new Color4(255, 255,   0, 255)),
					new Cube(new Vector3(0, 4, 0), 1.0f, new Color4(  0, 255, 255, 255)),
					new Cube(new Vector3(0, 6, 0), 1.2f, new Color4(255,   0, 255, 255)),
				},

				// 各立方体は1つずつ
				new uint[]
				{
					0,
					1,
					2,
					3,
					4,
					5
				},

				// 点描画
				BeginMode.Points,

				// 属性値を設定
				new VertexAttribution[]
				{
					new VertexAttribution("cubePosition", VertexAttribPointerType.Float, 3, 0),
					new VertexAttribution("cubeSize",     VertexAttribPointerType.Float, 1, sizeof(float)*3),
					new VertexAttribution("cubeColor",    VertexAttribPointerType.Float, 3, sizeof(float)*4),
				});

			// カメラや光源などその他の設定
			{
				// 深度を有効化
				mainWindow.ProgramGL.Enable(EnableCap.DepthTest);

				// シェーダー側で点の大きさを変更可能に
				mainWindow.ProgramGL.Enable(EnableCap.ProgramPointSize);

				// 画面全体を表示
				mainWindow.ProgramGL.Viewport(0, 0, mainWindow.viewport.ActualWidth, mainWindow.viewport.ActualHeight);

				// 環境光強度を割り当て
				mainWindow.ProgramGL.SetUniform("ambient", 0.3f);
			}
		}
	}
}