using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace LWisteria.StudiesOfOpenTK.ObjectiveOpenTK.Test
{
	/// <summary>
	/// 最上位ウインドウ
	/// </summary>
	partial class MainWindow : System.Windows.Window
	{
		/// <summary>
		/// 最上位ウインドウを作成
		/// </summary>
		/// <param name="vertexSource">バーテックスシェーダのソース</param>
		/// <param name="geometrySource">ジオメトリシェーダのソース</param>
		/// <param name="fragmentSource">フラグメントシェーダのソース</param>
		public MainWindow(
			string vertexSource,
			string geometrySource,
			string fragmentSource)
		{
			// コンポーネント初期化
			InitializeComponent();

			// ウインドウが読みこまれたら
			this.Loaded += (sender, e) =>
			{
				// シェーダー群を使ってビルド
				this.viewport.Program.Build(vertexSource, geometrySource, fragmentSource);

				// 再描画
				this.viewport.Invalidate();
			};
		}

		/// <summary>
		/// OpenGLのプログラムを取得する
		/// </summary>
		public Program ProgramGL
		{
			// 取得
			get
			{
				// ビューポートのプログラムを返す
				return this.viewport.Program;
			}
		}
	}
}