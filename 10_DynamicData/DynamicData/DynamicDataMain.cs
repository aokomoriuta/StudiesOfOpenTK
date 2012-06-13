using OpenTK;
using OpenTK.Graphics;
using System.Threading;
using System;
namespace LWisteria.StudiesOfOpenTK.DynamicData
{
	/// <summary>
	/// メインクラス
	/// </summary>
	partial class DynamicDataMain : System.Windows.Application
	{
		/// <summary>
		/// メインクラスを開始
		/// </summary>
		public DynamicDataMain()
		{
			// 最上位ウインドウを作成して設定
			MainWindow mainWindow = new MainWindow();
			base.MainWindow = mainWindow;

			// ウインドウ表示
			base.MainWindow.Show();

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

			// 立方体数
			int cubeCount = 300;

			// 立方体配列を初期化
			var cubes = new Cube[cubeCount * cubeCount];

			// 時刻と時間刻み
			double t = 0;
			double dt = 1.0/25;

			// 各立方体を
			for(int i = 0; i < cubeCount; i++)
			{
				for(int j = 0; j < cubeCount; j++)
				{
					// 作成
					cubes[i * cubeCount + j] = new Cube(0.8f, new Color4((byte)(255 * i / cubeCount), 0, 255, 255));

					// 座標設定
					cubes[i * cubeCount + j].PositionX = i;
					cubes[i * cubeCount + j].PositionY = j;
					cubes[i * cubeCount + j].PositionZ = 0;
				}
			}

			bool isProccessing = false;

			// 一定間隔での処理
			Timer timer = new Timer((TimerCallback)((state)=>
			{
				if(!isProccessing)
				{
					isProccessing = true;

					// 各立方体を
					for(int i = 0; i < cubeCount; i++)
					{
						for(int j = 0; j < cubeCount; j++)
						{
							cubes[i * cubeCount + j].PositionZ = (float)(10 * Math.Sin(10 * i * t / cubeCount) * j / cubeCount);
						}
					}

					// 立方体を描画
					mainWindow.Dispatcher.BeginInvoke((Action)(() =>
					{
						mainWindow.Draw(cubes);

						isProccessing = false;
					}));
				}

				// 時刻を進める
				t += dt;
			}));

			// 動的描画を開始
			timer.Change(0, (int)(dt*1000));

		}
	}
}