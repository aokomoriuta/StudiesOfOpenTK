using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using LWisteria.StudiesOfOpenTK.ObjectiveTK;

using Buffer = LWisteria.StudiesOfOpenTK.ObjectiveTK.Buffer;

namespace LWisteria.StudiesOfOpenTK.PointSprite
{
	/// <summary>
	/// 最上位ウインドウ
	/// </summary>
	partial class MainWindow : System.Windows.Window
	{
		/// <summary>
		/// ポイントスプライトプログラム
		/// </summary>
		Program programPoint;

		/// <summary>
		/// 粒子群バッファー
		/// </summary>
		Buffer particlesBuffer = null;

		/// <summary>
		/// 最上位ウインドウを作成
		/// </summary>
		public MainWindow()
		{
			// コンポーネント初期化
			InitializeComponent();

			// ポイントスプライトプログラムを作成
			this.programPoint = new Program(
				this.Viewport,
				Properties.Resources.point_vertex,
				Properties.Resources.point_geometry,
				Properties.Resources.point_fragment);

			// ビューポートにプログラムを追加
			this.Viewport.Programs.Add(this.programPoint);

			// 点の大きさを変更可能に設定
			this.programPoint.Enable(EnableCap.ProgramPointSize);

			// ポイントスプライト用
			this.programPoint.Enable(EnableCap.PointSprite);

			// パラメーター再設定
			Action setParameters = () =>
			{
				// 視野変換および投影変換行列を設定
				this.programPoint.SetUniform("view", this.Viewport.View);
				this.programPoint.SetUniform("projection", this.Viewport.Projection);

				// 表示する大きさ（拡大率）を設定
				this.programPoint.SetUniform("size", (float)this.Viewport.Camera.R);

				// カメラの位置を設定
				this.programPoint.SetUniform("cameraPosition", this.Viewport.Camera.Position);
			};

			// ウインドウが読みこまれたら
			this.Loaded += (sender, e) =>
			{
				// 真上からの平行光線を設定
				this.programPoint.SetUniform("sunLight", new Vector3(0, 0, -0.6f));

				// 環境光強度を割り当て
				this.programPoint.SetUniform("ambient", 0.1f);

				// カメラからの光源の強度を割り当て
				this.programPoint.SetUniform("cameraLight", 0.3f);
			};

			// 再描画時に
			this.Viewport.Invalidated += (sender, e) =>
			{
				// パラメーター再設定
				setParameters();
			};

			// カメラが動いたら
			this.Viewport.Camera.Changed += (sender, e) =>
			{
				// パラメーター再設定
				setParameters();
			};

			// 終了時に
			this.Closed += (sender, e) =>
			{
				// プログラムを削除
				this.programPoint.Dispose();
			};
		}

		/// <summary>
		/// 粒子を描画
		/// </summary>
		/// <param name="particles">描画する粒子の配列</param>
		public void Draw(Particle[] particles)
		{
			// バッファーがなければ
			if(particlesBuffer == null)
			{
				// 動的バッファーを作成
				particlesBuffer = Buffer.CreateDynamic<Particle>(
						this.Viewport, BeginMode.Points,
						particles.Length,
						MainWindow.CreatePointsIndices(particles.Length));

				// ポイントスプライトプログラムにバッファーを割り当て
				programPoint.AttachBuffer(particlesBuffer,
					new[]
					{
						new VertexAttribution("particleX", VertexAttribPointerType.Float, 3, 0),
						new VertexAttribution("particleD", VertexAttribPointerType.Float, 1, Vector3.SizeInBytes),
						new VertexAttribution("particleColor", VertexAttribPointerType.Float, 4, Vector3.SizeInBytes + sizeof(float)),
					});
			}

			// データを書き込み
			particlesBuffer.WriteData(particles);

			// 再描画
			this.Viewport.Invalidate();
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