using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using LWisteria.StudiesOfOpenTK.Math;
using System.Collections.ObjectModel;
using Cloo;
namespace LWisteria.StudiesOfOpenTK.SimpleCloo
{
	/// <summary>
	/// OpenCLでの計算プログラム
	/// </summary>
	public sealed class ComputerCL : Computer
	{
		/// <summary>
		/// このプログラムで使用するプラットフォーム
		/// </summary>
		public readonly ComputePlatform Platform;

		/// <summary>
		/// このプログラムで使用するデバイス群
		/// </summary>
		public readonly ReadOnlyCollection<ComputeDevice> Devices;

		/// <summary>
		/// 実行カーネル
		/// </summary>
		readonly ComputeKernel sinAccelerationKernel;

		/// <summary>
		/// コマンドキュー
		/// </summary>
		readonly ComputeCommandQueue queue;

		/// <summary>
		/// 粒子数
		/// </summary>
		long particleCount;

		/// <summary>
		/// 粒子の位置バッファー
		/// </summary>
		ComputeBuffer<Vector4> bufferX;

		/// <summary>
		/// 粒子の速度バッファー
		/// </summary>
		ComputeBuffer<Vector4> bufferU;

		/// <summary>
		/// 粒子の加速度バッファー
		/// </summary>
		ComputeBuffer<Vector4> bufferA;

		/// <summary>
		/// 粒子の直径（粒径）バッファー
		/// </summary>
		ComputeBuffer<float> bufferD;


		/// <summary>
		/// 粒径データ配列
		/// </summary>
		float[] particlesD;

		/// <summary>
		/// 材質データ配列
		/// </summary>
		Material[] particlesMaterial;

		/// <summary>
		/// 粒子種データ配列
		/// </summary>
		ParticleType[] particlesType;


		/// <summary>
		/// 準備処理
		/// </summary>
		Action prepare;

		/// <summary>
		/// OpenCLでの計算プログラムを作成する
		/// </summary>
		/// <param name="maxDt">初期時間刻み</param>
		/// <param name="a">振幅</param>
		/// <param name="omega">角速度</param>
		public ComputerCL(double maxDt, double a, double omega)
			: base(maxDt, a, omega)
		{
			// プラットフォームとデバイス群を取得
			this.Platform = ComputePlatform.Platforms[0];
			this.Devices = this.Platform.Devices;

			// コンテキストを作成
			var context = new ComputeContext(this.Devices, new ComputeContextPropertyList(this.Platform), null, IntPtr.Zero);

			// キューを作成
			this.queue = new ComputeCommandQueue(context, this.Devices[0], ComputeCommandQueueFlags.None);

			// プログラムを作成
			var program = new ComputeProgram(context, Properties.Resources.SinAcceleration);

			// ビルドしてみて
			try
			{
				program.Build(this.Devices, null, null, IntPtr.Zero);
			}
			// 失敗したら
			catch(BuildProgramFailureComputeException ex)
			{
				// 例外を投げる
				throw new BuildCLException(program.Source[0], program.GetBuildLog(this.Devices[0]));
			}

			// カーネルを作成
			this.sinAccelerationKernel = program.CreateKernel("SinAcceleration");

			// 準備処理は何もしない
			this.prepare = () => { };

			// 粒子が追加された時に
			base.ParticleAdded += (sender, e) =>
			{
				// 準備処理の時の処理を実装
				this.prepare = () =>
				{
					// 粒子数を設定
					this.particleCount = this.inputParticles.Count;

					// バッファーを作成
					this.bufferX = new ComputeBuffer<Vector4>(context, ComputeMemoryFlags.ReadWrite, this.particleCount);
					this.bufferU = new ComputeBuffer<Vector4>(context, ComputeMemoryFlags.ReadWrite, this.particleCount);
					this.bufferA = new ComputeBuffer<Vector4>(context, ComputeMemoryFlags.ReadWrite, this.particleCount);
					this.bufferD = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly, this.particleCount);

					// 入力データを確保
					var particlesX = new Vector4[this.particleCount];
					var particlesU = new Vector4[this.particleCount];
					var particlesA = new Vector4[this.particleCount];
					this.particlesD = new float[this.particleCount];
					this.particlesMaterial = new Material[this.particleCount];
					this.particlesType = new ParticleType[this.particleCount];

					// 全粒子について
					int i = 0;
					foreach(var particle in this.inputParticles)
					{
						// データをコピー
						particlesX[i] = new Vector4((Vector3)particle.X, 0);
						particlesU[i] = new Vector4((Vector3)particle.U, 0);
						particlesA[i] = new Vector4((Vector3)particle.A, 0);
						this.particlesD[i] = (float)particle.D;
						this.particlesMaterial[i] = particle.Material;
						this.particlesType[i] = particle.Type;

						i++;
					}

					// バッファーへ転送
					this.queue.WriteToBuffer(particlesX, this.bufferX, false, null);
					this.queue.WriteToBuffer(particlesU, this.bufferU, false, null);
					this.queue.WriteToBuffer(particlesA, this.bufferA, false, null);
					this.queue.WriteToBuffer(this.particlesD, this.bufferD, false, null);

					// 入力粒子群を空にする
					this.inputParticles.Clear();

					// 準備処理は空
					this.prepare = () => { };

					// ここまで完了を待機
					queue.Finish();
				};
			};
		}

		/// <summary>
		/// 計算を1ステップ進める
		/// </summary>
		public override void Next()
		{
			// 準備処理
			this.prepare();

			// 初期時間刻みを設定
			double dt = this.MaxDt/10;

			// 引数を設定
			//  # 粒子数
			//  # 位置ベクトル配列
			//  # 速度ベクトル配列
			//  # 加速度ベクトル配列
			//  # 粒径配列
			//  # 振幅
			//  # 角速度
			//  # 現在時刻
			//  # 時間刻み
			this.sinAccelerationKernel.SetValueArgument(0, this.particleCount);
			this.sinAccelerationKernel.SetMemoryArgument(1, this.bufferX);
			this.sinAccelerationKernel.SetMemoryArgument(2, this.bufferU);
			this.sinAccelerationKernel.SetMemoryArgument(3, this.bufferA);
			this.sinAccelerationKernel.SetMemoryArgument(4, this.bufferD);
			this.sinAccelerationKernel.SetValueArgument(5, (float)this.A);
			this.sinAccelerationKernel.SetValueArgument(6, (float)this.Omega);
			this.sinAccelerationKernel.SetValueArgument(7, (float)this.T);
			this.sinAccelerationKernel.SetValueArgument(8, (float)dt);

			// 実行
			this.queue.Execute(this.sinAccelerationKernel, null, new long[] { this.particleCount }, null, null);

			// ここまで待機
			this.queue.Finish();

			// 時刻を進める
			base.UpdateTime(dt);
		}
		/// <summary>
		/// 現在の粒子を取得する
		/// </summary>
		/// <returns>現在の粒子の複製</returns>
		public override Particle[] GetParticles()
		{
			// 準備が出来ていなかったら
			if(this.bufferX != null)
			{
				// 出力データを確保
				var particlesX = new Vector4[this.particleCount];
				var particlesU = new Vector4[this.particleCount];
				var particlesA = new Vector4[this.particleCount];

				// バッファーから転送
				this.queue.ReadFromBuffer(this.bufferX, ref particlesX, false, null);
				this.queue.ReadFromBuffer(this.bufferU, ref particlesU, false, null);
				this.queue.ReadFromBuffer(this.bufferA, ref particlesA, false, null);

				// 出力粒子を作製
				var particles = new Particle[this.particleCount];

				// ここまで待機
				this.queue.Finish();

				// 全粒子について
				for(ulong i = 0; i < (ulong)this.particleCount; i++)
				{
					particles[i] = new Particle(i,
						this.particlesD[i],
						this.particlesMaterial[i],
						this.particlesType[i])
						{
							X = new Vector(particlesX[i].X, particlesX[i].Y, particlesX[i].Z),
							U = new Vector(particlesU[i].X, particlesU[i].Y, particlesU[i].Z),
							A = new Vector(particlesA[i].X, particlesA[i].Y, particlesA[i].Z),
						};
				};

				// 粒子群を返す
				return particles;
			}

			// 空の配列を返す
			return new Particle[0];
		}

		/// <summary>
		/// 粒子数を取得する
		/// </summary>
		public override ulong ParticleCount
		{
			get
			{
				// 現在の粒子群の数を返す
				return (ulong)this.particleCount;
			}
		}
	}
}