using OpenTK;
using OpenTK.Graphics;
using System.Threading;
using System;
namespace LWisteria.StudiesOfOpenTK.PointSprite
{
	/// <summary>
	/// メインクラス
	/// </summary>
	partial class PointSpriteMain : System.Windows.Application
	{
		/// <summary>
		/// メインクラスを開始
		/// </summary>
		public PointSpriteMain()
		{
			// 最上位ウインドウを作成して設定
			MainWindow mainWindow = new MainWindow();
			base.MainWindow = mainWindow;

			// ウインドウ表示
			base.MainWindow.Show();

			// 立方体数
			int particlesCount = 100;

			// 立方体配列を初期化
			var particles = new Particle[particlesCount * particlesCount * particlesCount];

			// 3次元配列番号から1次元配列番号に変換する処理
			Func<int, int, int, int> index3To1 = (i, j, k) => ((i * particlesCount + j) * particlesCount + k);

			// 時刻と時間刻み
			double t = 0;
			double dt = 1.0 / 25;

			// 各立方体を
			for(int i = 0; i < particlesCount; i++)
			{
				for(int j = 0; j < particlesCount; j++)
				{
					for(int k = 0; k < particlesCount; k++)
					{
						// 作成
						particles[index3To1(i, j, k)] = new Particle(0.5f + 0.5f * k / particlesCount, new Color4((byte)(255 * i / particlesCount), 0, 255, 255));

						// 座標設定
						particles[index3To1(i, j, k)].X.X = i;
						particles[index3To1(i, j, k)].X.Y = j;
						particles[index3To1(i, j, k)].X.Z = k;
					}
				}
			}

			// 処理中かどうか
			bool isProccessing = false;

			// 一定間隔での処理
			Timer timer = new Timer((TimerCallback)((state) =>
			{
				// 処理中でなければ
				if(!isProccessing)
				{
					// 処理開始
					isProccessing = true;

					// 各粒子を
					for(int i = 0; i < particlesCount; i++)
					{
						for(int j = 0; j < particlesCount; j++)
						{
							for(int k = 0; k < particlesCount; k++)
							{
								// 動かす
								particles[index3To1(i, j, k)].X.Z = k+ (float)(10 * Math.Sin(10 * i * t / particlesCount) * j / particlesCount);
							}
						}
					}

					// 画面に
					mainWindow.Dispatcher.BeginInvoke((Action)(() =>
					{
						// 粒子を描画
						mainWindow.Draw(particles);

						// 処理完了
						isProccessing = false;
					}));
				}

				// 時刻を進める
				t += dt;
			}));

			// 描画を開始
			timer.Change(0, (int)(dt * 1000));

		}
	}
}