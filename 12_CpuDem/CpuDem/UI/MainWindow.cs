using System;
using System.Threading;
using LWisteria.StudiesOfOpenTK.ObjectiveTK;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Buffer = LWisteria.StudiesOfOpenTK.ObjectiveTK.Buffer;

namespace LWisteria.StudiesOfOpenTK.CpuDem
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
		/// 1秒間の描画回数
		/// </summary>
		const int fps = 1000 / 25;


		/// <summary>
		/// 最上位ウインドウを作成
		/// </summary>
		public MainWindow(DemComputerCpu computer)
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



			// 描画中かどうか
			bool isRendering = false;

			// 出力用粒子配列を初期化
			var outputParticles = new OutputParticle[0];

			// 描画処理
			Timer renderer = new Timer((TimerCallback)((state) =>
			{
				// 描画中でなければ
				if(!isRendering)
				{
					// 描画開始
					isRendering = true;

					// 計算空間画面に
					this.Viewport.Dispatcher.BeginInvoke((Action)(() =>
					{
						// 現在の粒子を取得
						var thisParticles = computer.GetParticles();

						// 出力用と粒子数が違えば
						if(outputParticles.Length != thisParticles.Length)
						{
							// 出力用粒子群を再生成
							outputParticles = new OutputParticle[thisParticles.Length];

							// 動的バッファーを作成
							particlesBuffer = Buffer.CreateDynamic<OutputParticle>(
								this.Viewport, BeginMode.Points,
								outputParticles.Length,
								MainWindow.CreatePointsIndices(outputParticles.Length));

							// ポイントスプライトプログラムにバッファーを割り当て
							programPoint.AttachBuffer(particlesBuffer,
								new[]
							{
								new VertexAttribution("particleX", VertexAttribPointerType.Float, 3, 0),
								new VertexAttribution("particleD", VertexAttribPointerType.Float, 1, Vector3.SizeInBytes),
								new VertexAttribution("particleColor", VertexAttribPointerType.Float, 4, Vector3.SizeInBytes + sizeof(float)),
							});
						}

						// すべての粒子を
						int i = 0;
						foreach(var particle in thisParticles)
						{
							// 出力用粒子に変換
							outputParticles[i++] = new OutputParticle((OpenTK.Vector3)particle.X, (float)particle.D, particle.Material.Color);
						}

						// 出力粒子を書き込み
						particlesBuffer.WriteData(outputParticles);

						// 再描画
						this.Viewport.Invalidate();


						// 処理完了
						isRendering = false;
					}));
				}

				// 時刻表示パネルに
				this.TimePanel.Dispatcher.BeginInvoke((Action)(() =>
				{
					// 時刻・タイムステップ数・時間刻みを表示
					this.TBox.Text = computer.T.ToString("G5");
					this.StepsBox.Text = computer.TimeStep.ToString();
					this.DtBox.Text = computer.Dt.ToString("e");

					// 粒子数を表示
					this.ParticleCountBox.Text = computer.ParticleCount.ToString();
				}));
			}));

			// 描画を開始
			renderer.Change(0, fps);


			// 速度計測間隔
			double measureInterval = 1.0;

			// 前の時刻と時間ステップ
			double oldT = 0;
			double oldTimeStep = 0;

			// 速度計測
			Timer speedMeasure = new Timer((TimerCallback)((state) =>
			{
				// 速度表示パネルに
				this.TimePanel.Dispatcher.BeginInvoke((Action)(() =>
				{
					// 現在時刻と時間ステップを取得
					var thisTimeStep = computer.TimeStep;
					var thisT = computer.T;

					// 各データを表示
					this.StepPerRealBox.Text = ((thisTimeStep - oldTimeStep)/measureInterval).ToString();
					this.StepPerRealPerParticleBox.Text = ((thisTimeStep - oldTimeStep) / measureInterval / computer.ParticleCount).ToString("G5");
					this.ComputationalPerRealBox.Text = ((thisT - oldT)/measureInterval).ToString("G5");

					// 前の時刻と時間ステップを設定
					oldT = thisT;
					oldTimeStep = thisTimeStep;
				}));
			}));

			// 1秒感覚で速度計測を開始
			speedMeasure.Change(0, (int)(measureInterval*1000));
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