using System.Threading;
using LWisteria.StudiesOfOpenTK.Math;
using OpenTK.Graphics;
using System.Threading.Tasks;
using Cloo;
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
			const double sizeX = 4500e-3;
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
			var computer = new ComputerCpu(maxDt, A, omega);


			// 最上位ウインドウを作成して設定
			MainWindow mainWindow = new MainWindow(computer);
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

			mainWindow.LogLine("完了");

			// 計算中かどうか
			bool isComputing = true;

			// 個別要素法の自動実行スレッドを生成
			Task demWorker = new Task(() =>
			{
				// 計算中の間
				//while(isComputing)
				{
					// 次ステップに進める
					computer.Next();
				}
			}, TaskCreationOptions.LongRunning);

			// アプリケーションの終了時に
			this.Exit += (sender, e) =>
			{
				// 計算終了
				isComputing = false;
			};

			// 自動実行スレッド開始
			demWorker.Start();

			// OS名
			mainWindow.OSNameBox.Text = System.Environment.OSVersion.ToString();

			// CPUの
			foreach(var processor in new System.Management.ManagementClass("Win32_Processor").GetInstances())
			{
				// データを表示
				mainWindow.CpuNameBox.Text = string.Format("{0}", processor["Name"]);
				mainWindow.CpuVenderBox.Text = string.Format("{0}", processor["Manufacturer"]);
				mainWindow.CpuMaxClockBox.Text = string.Format("{0:0.00}", double.Parse(processor["MaxClockSpeed"].ToString())/1024);
				mainWindow.CpuCoreBox.Text = string.Format("{0}", processor["NumberOfCores"]);
			}
			foreach(var os in new System.Management.ManagementClass("Win32_OperatingSystem").GetInstances())
			{
				// メモリを表示
				mainWindow.CpuMemoryBox.Text = string.Format("{0:0.00}", double.Parse(os["MaxProcessMemorySize"].ToString()) / 1024 / 1024);
			}

			// OpenCLプラットフォームとデバイスを取得
			var platform = ComputePlatform.Platforms[0];
			var devices = platform.Devices;

			// プラットフォームデータを表示
			mainWindow.PlatformNameBox.Text = platform.Name;
			mainWindow.PlatformVenderBox.Text = platform.Vendor;
			mainWindow.PlatformVersionBox.Text = platform.Version;
			mainWindow.PlatformProfileBox.Text = platform.Profile;

			// デバイスデータを表示
			mainWindow.DeviceNameBox.ItemsSource = devices;
			mainWindow.DeviceNameBox.SelectionChanged += (sender, e) =>
			{
				var device = mainWindow.DeviceNameBox.SelectedItem as ComputeDevice;

				mainWindow.DeviceVenderBox.Text = device.Vendor;
				mainWindow.DeviceDriverVersionBox.Text = device.DriverVersion;
				mainWindow.DeviceOpenCLVersionBox.Text = device.OpenCLCVersionString;
				mainWindow.DeviceCUBox.Text = string.Format("{0}", device.MaxComputeUnits);
				mainWindow.DeviceGlobalMemoryBox.Text = string.Format("{0}", device.GlobalMemorySize);
				mainWindow.DeviceLocalMemoryBox.Text = string.Format("{0}", device.LocalMemorySize);
			};
			mainWindow.DeviceNameBox.SelectedIndex = 0;

		}
	}
}