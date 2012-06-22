using System.Threading;
using LWisteria.StudiesOfOpenTK.Math;
using OpenTK.Graphics;
using System.Threading.Tasks;
using Cloo;
using System;
namespace LWisteria.StudiesOfOpenTK.SimpleCloo
{
	/// <summary>
	/// メインクラス
	/// </summary>
	partial class SimpleClooMain : System.Windows.Application
	{
		/// <summary>
		/// メインクラスを開始
		/// </summary>
		public SimpleClooMain()
		{
			// 時間刻み
			const double maxDt = 0.01;

			// 粒子の大きさ
			const double diameter = 50e-3;
			const double diameterWall = diameter * 0.8;
			const double length0 = diameter * 1.001;
			const double length0Wall = diameterWall * 0.75;

			// 計算領域
			const double sizeX = 45000e-3;
			const double sizeZ = 2200e-3;
			const double extraSize = diameter;

			// 物性値
			const double rho = 2.7e3;
			const double E = 30e6;
			const double nu = 0.2;

			// 振幅と角速度
			double A = 0.5;
			double omega = 2.0;
			
			// 計算プログラムを初期化
			var computerCpu = new ComputerCpu(maxDt, A, omega);
			var computerCL = new ComputerCL(maxDt, A, omega);


			// 最上位ウインドウを作成して設定
			MainWindow mainWindow = new MainWindow(computerCpu, computerCL);
			base.MainWindow = mainWindow;

			mainWindow.Log("データ初期化中...");
			// ウインドウ表示
			base.MainWindow.Show();
			mainWindow.LogLine("完了");

			mainWindow.Log("データ作成中...");

			// 材質を生成
			var materials = new []
			{
				// 移動粒子
				new Material(0, rho, E, nu, new Color4( 50,  50, 255, 255)),

				// 壁
				new Material(1, rho, E, nu, new Color4(200, 200, 200, 255))
			};

			// 粒子番号
			ulong particleID = 0;

			// 領域内に
			for(double x = 0; x < sizeX; x += length0)
			{
				for(double z = 0; z < sizeZ; z += length0)
				{
					// 移動粒子を作成
					var particle = new Particle(particleID++, diameter * (0.4 + 0.6 * z / sizeZ), materials[0], ParticleType.FreeMovable)
					{
						X = new Vector(x, 0, z)
					};

					// 追加
					computerCpu.AddParticle(particle);
					computerCL.AddParticle(particle);
				}
			}

			// 床の部分に
			for(double x = -extraSize; x < sizeX + extraSize; x += length0Wall)
			{
				// 固定粒子を作成
				var particle = new Particle(particleID++, diameterWall, materials[1], ParticleType.Fixed)
				{
					X = new Vector(x, 0, -extraSize)
				};

				// 追加
				computerCpu.AddParticle(particle);
				computerCL.AddParticle(particle);
			}

			mainWindow.LogLine("完了");



			// 計算中かどうか
			bool isComputing = false;

			// 計算中かどうかを変更する
			Action<bool> changeIsComputing = (isComputingNow) =>
			{
				// 計算状態を設定
				isComputing = isComputingNow;
				
				// ウインドウで
				mainWindow.Dispatcher.BeginInvoke((Action)(() =>
				{
					// 1ステップ計算のボタンの有効化および無効化
					mainWindow.ComputeOneButton.IsEnabled = !isComputingNow;

					// 計算状態に合わせて自動実行ボタンの表示を変える
					mainWindow.ComputeAllButton.Content = (isComputingNow) ? "自動実行を停止" : "自動実行を開始";


					// 計算開始および終了を通知
					mainWindow.LogLine((isComputingNow) ? "計算を開始" : "計算を終了");
				}));
			};


			// 1ステップ実行がクリックされたら
			mainWindow.ComputeOneButton.Click += (sender, e) =>
			{
				// 計算中でなければ
				if(!isComputing)
				{
					// 計算開始
					changeIsComputing(true);

					// 1ステップだけ進める
					mainWindow.computer.Next();

					// 計算終了
					changeIsComputing(false);
				}
			};

			// 自動実行がクリックされたら
			mainWindow.ComputeAllButton.Click += (sender, e) =>
			{
				// 計算中でなければ
				if(!isComputing)
				{
					// 計算開始
					changeIsComputing(true);

					// 自動実行スレッド開始
					new Task(() =>
					{
						// 計算中の間
						while(isComputing)
						{
							// 次ステップに進める
							mainWindow.computer.Next();
						}

						// 計算終了
						changeIsComputing(false);

					}, TaskCreationOptions.LongRunning).Start();
				}
				// 計算中なら
				else
				{
					// 計算中断
					changeIsComputing(false);
				}
			};

			// プログラムが切り替わったら
			System.Windows.RoutedEventHandler stopWhenSwitchWith = (sender, e) =>
			{
				// 計算終了
				changeIsComputing(false);
			};

			// プログラムが切り替わった時の処理
			mainWindow.WithCpuButton.Checked += stopWhenSwitchWith;
			mainWindow.WithCLButton.Checked += stopWhenSwitchWith;

			// アプリケーションの終了時に
			this.Exit += (sender, e) =>
			{
				// 計算終了
				changeIsComputing(false);
			};

		}
	}
}