using System;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using LWisteria.StudiesOfOpenTK.Math;
namespace LWisteria.StudiesOfOpenTK.CpuDem
{
	/// <summary>
	/// メインクラス
	/// </summary>
	partial class CpuDemMain : System.Windows.Application
	{
		/// <summary>
		/// メインクラスを開始
		/// </summary>
		public CpuDemMain()
		{
			// 時間刻み
			const double maxDt = 0.01;

			// 粒子の大きさ
			const double diameter = 50e-3;
			const double diameterWall = diameter * 0.8;
			const double length0 = diameter * 1.001;
			const double length0Wall = diameterWall * 0.75;

			// 計算領域
			const double sizeX = 4500e-3;
			const double sizeZ = 2200e-3;
			const double extraSize = diameter;

			// 物性値
			const double rho = 2.7e3;
			const double E = 30e6;
			const double nu = 0.2;

			// 重力加速度
			Vector g = new Vector(0, 0, -9.8);


			// 立方体配列を初期化
			var computer = new DemComputerCpu(maxDt, g);

			// 材質を生成
			var materials = new []
			{
				// 移動粒子
				new Material(0, rho, E, nu, new Color4( 50,  50, 255, 255)),

				// 壁
				new Material(1, rho, E, nu, new Color4(200, 200, 200, 255))
			};

			ulong particleID = 0;

			// 領域内に
			for(double x = 0; x < sizeX; x += length0)
			{
				for(double z = 0; z < sizeZ; z += length0)
				{
					// 移動粒子作成して追加
					computer.AddParticle(new Particle(particleID++, diameter * (0.4 + 0.6 * z / sizeZ), materials[0], ParticleType.FreeMovable)
					{
						X = new Vector(x, 0, z)
					});
				}
			}

			// 床の部分に
			for(double x = -extraSize; x < sizeX + extraSize; x += length0Wall)
			{
				// 作成して追加
				computer.AddParticle(new Particle(particleID++, diameterWall, materials[1], ParticleType.Fixed)
				{
					X = new Vector(x, 0, -extraSize)
				});
			}

			// 壁の部分に
			for(double z = -extraSize; z < sizeZ + extraSize; z += length0Wall)
			{
				// 作成して追加
				computer.AddParticle(new Particle(particleID++, diameterWall, materials[1], ParticleType.Fixed)
				{
					X = new Vector(-extraSize, 0, z)
				});
				computer.AddParticle(new Particle(particleID++, diameterWall, materials[1], ParticleType.Fixed)
				{
					X = new Vector(sizeX+extraSize, 0, z)
				});
			}

			// 計算中かどうか
			bool isComputing = true;

			// 個別要素法の自動実行スレッドを生成
			Thread demWorker = new Thread((ThreadStart)(() =>
			{
				// 計算中の間
				while(isComputing)
				{
					// 次ステップに進める
					computer.Next();
				}
			}));

			// アプリケーションの終了時に
			this.Exit += (sender, e) =>
			{
				// 計算終了
				isComputing = false;

				// 実行中断
				demWorker.Abort();
			};

			// 自動実行スレッド開始
			demWorker.Start();


			// 最上位ウインドウを作成して設定
			MainWindow mainWindow = new MainWindow(computer);
			base.MainWindow = mainWindow;

			// ウインドウ表示
			base.MainWindow.Show();
		}
	}
}