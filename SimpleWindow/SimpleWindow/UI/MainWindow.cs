using OpenTK.Graphics.OpenGL;
using System.Drawing;
namespace LWisteria.StudiesOfOpenTK.SimpleWindow
{
	/// <summary>
	/// 最上位ウインドウ
	/// </summary>
	partial class MainWindow : System.Windows.Window
	{
		/// <summary>
		/// 最上位ウインドウを作成
		/// </summary>
		public MainWindow()
		{
			// コンポーネント初期化
			InitializeComponent();

			// ウインドウが読みこまれたら
			this.Loaded += (sender, e) =>
			{
				// バッファーをクリア
				GL.ClearColor(System.Drawing.Color.DarkGray);

				// 投影法の設定開始
				GL.MatrixMode(MatrixMode.Projection);

				// 単位行列を生成
				GL.LoadIdentity();

				// コントロールの幅で垂直投影法を設定
				GL.Ortho(0, glControl.Width, 0, glControl.Height, -1, 1);

				// ビューは全体が見えるように
				GL.Viewport(0, 0, glControl.Width, glControl.Height);
			};

			// コントロールの描画時には
			this.glControl.Paint += (sender, e) =>
			{
				// 色と深度バッファーをクリア
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				// モーダルビューに設定
				GL.MatrixMode(MatrixMode.Modelview);

				// 単位行列を生成
				GL.LoadIdentity();

				// 黄色で
				GL.Color3(Color.Yellow);
				
				// 描画開始
				GL.Begin(BeginMode.Triangles);
				
				// 三角形の頂点を設定
				GL.Vertex2(10, 20);
				GL.Vertex2(100, 20);
				GL.Vertex2(100, 50);
				
				// 描画完了
				GL.End();

				// バッファーを交換
				glControl.SwapBuffers();
			};
		}
	}
}