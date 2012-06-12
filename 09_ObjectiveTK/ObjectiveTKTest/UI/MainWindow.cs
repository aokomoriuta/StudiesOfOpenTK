using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace LWisteria.StudiesOfOpenTK.ObjectiveTK.Test
{
	/// <summary>
	/// 最上位ウインドウ
	/// </summary>
	partial class MainWindow : System.Windows.Window
	{
		/// <summary>
		/// 立方体プログラム
		/// </summary>
		Program programCube;

		/// <summary>
		/// 四角錐プログラム
		/// </summary>
		Program programPyramid;

		/// <summary>
		/// 最上位ウインドウを作成
		/// </summary>
		public MainWindow()
		{
			// コンポーネント初期化
			InitializeComponent();

			// 立方体プログラムを作成
			this.programCube = new Program(
				this.Viewport,
				Properties.Resources.cube_vertex,
				Properties.Resources.cube_geometry,
				Properties.Resources.cube_fragment);

			// 四角錐プログラムを作成
			this.programPyramid = new Program(
				this.Viewport,
				Properties.Resources.pyramid_vertex,
				Properties.Resources.pyramid_geometry,
				Properties.Resources.pyramid_fragment);

			// ビューポートにプログラムを追加
			this.Viewport.Programs.Add(this.programCube);
			this.Viewport.Programs.Add(this.programPyramid);

			// 再描画処理
			Action redraw = () =>
			{
				// ビュー（カメラ）を作成
				Matrix4 view = this.Viewport.View;

				// 投影法を作成
				Matrix4 projection = this.Viewport.Projection;

				// カメラからの光源の方向を計算
				var light = this.Viewport.Camera.LookAt - this.Viewport.Camera.Position;
				light.Normalize();


				// 立方体プログラムに割り当て
				this.programCube.SetUniform("view", view);
				this.programCube.SetUniform("light", light);
				this.programCube.SetUniform("projection", projection);

				// 四角錐プログラムに割り当て
				this.programPyramid.SetUniform("view", view);
				this.programPyramid.SetUniform("light", light);
				this.programPyramid.SetUniform("projection", projection);

				// 再描画
				this.Viewport.Invalidate();
			};

			// ウインドウが読みこまれたら
			this.Loaded += (sender, e) =>
			{
				// 立方体プログラムに環境光強度を割り当て
				this.programCube.SetUniform("ambient", 0.3f);

				// 四角錐プログラムに環境光強度を割り当て
				this.programPyramid.SetUniform("ambient", 0.3f);
			};

			// カメラが動いたら
			this.Viewport.Camera.Changed += (sender, e) =>
			{
				// 再描画
				redraw();
			};

			// 大きさが変わったら
			this.SizeChanged += (sender, e) =>
			{
				// 再描画
				redraw();
			};

			// 終了時に
			this.Closed += (sender, e) =>
			{
				// プログラムを削除
				this.programCube.Dispose();
				this.programPyramid.Dispose();
			};
		}

		/// <summary>
		/// 立方体を描画
		/// </summary>
		/// <param name="cubes">描画する立方体の配列</param>
		public void Draw(Cube[] cubes)
		{
			// 立方体プログラムにバッファーを割り当て
			programCube.AttachBuffer(
				Buffer.Create(
					this.Viewport, BeginMode.Points,
					cubes,
					MainWindow.CreatePointsIndices(cubes.Length)),
				new[]
				{
					new VertexAttribution("cubePosition", VertexAttribPointerType.Float, 3, 0),
					new VertexAttribution("cubeSize", VertexAttribPointerType.Float, 1, Vector3.SizeInBytes),
					new VertexAttribution("cubeColor", VertexAttribPointerType.Float, 4, Vector3.SizeInBytes + sizeof(float)),
				});
		}

		/// <summary>
		/// 四角錐を描画
		/// </summary>
		/// <param name="pyramids">描画する四角錐の配列</param>
		public void Draw(Pyramid[] pyramids)
		{
			// 四角錐プログラムにバッファーを割り当て
			programPyramid.AttachBuffer(
				Buffer.Create(
					this.Viewport, BeginMode.Points,
					pyramids,
					MainWindow.CreatePointsIndices(pyramids.Length)),
				new[]
				{
					new VertexAttribution("cubePosition", VertexAttribPointerType.Float, 3, 0),
					new VertexAttribution("cubeSize", VertexAttribPointerType.Float, 1, Vector3.SizeInBytes),
					new VertexAttribution("cubeColor", VertexAttribPointerType.Float, 4, Vector3.SizeInBytes + sizeof(float)),
				});
		}

		/// <summary>
		/// 点用のインデックス配列を作成する
		/// </summary>
		/// <param name="count">要素数</param>
		/// <returns>インデックス配列</returns>
		static uint[] CreatePointsIndices(int count)
		{
			// 配列を初期化
			var indices = new uint[count];

			// 要素数分
			for(uint i = 0; i < count; i++)
			{
				// インデックスを設定
				indices[i] = i;
			}

			// 配列を返す
			return indices;
		}
	}
}