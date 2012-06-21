using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using LWisteria.StudiesOfOpenTK.Math;
namespace LWisteria.StudiesOfOpenTK.SimpleCloo
{
	/// <summary>
	/// CPUでの計算プログラム
	/// </summary>
	public class ComputerCpu
	{
		/// <summary>
		/// 現在の粒子群
		/// </summary>
		Particle[] particles;

		/// <summary>
		/// 入力粒子群
		/// </summary>
		List<Particle> inputParticles;

		/// <summary>
		/// 現在時刻
		/// </summary>
		public double T { get; private set; }

		/// <summary>
		/// タイムステップ数
		/// </summary>
		public long TimeStep { get; private set; }

		/// <summary>
		/// 時間刻み
		/// </summary>
		public double Dt { get; private set; }

		/// <summary>
		/// 振幅
		/// </summary>
		public readonly double A;

		/// <summary>
		/// 角速度
		/// </summary>
		public readonly double Omega;

		/// <summary>
		/// 最大時間刻み
		/// </summary>
		public readonly double MaxDt;

		/// <summary>
		/// 準備処理
		/// </summary>
		Action prepare;

		/// <summary>
		/// 粒子に対して実行する操作
		/// </summary>
		/// <param name="particle">操作対象の粒子</param>
		/// <param name="index">操作対象粒子の配列上の番号</param>
		delegate void ParticleAction(Particle particle, ulong index);

		/// <summary>
		/// CPUでの計算プログラムを作成する
		/// </summary>
		/// <param name="maxDt">初期時間刻み</param>
		/// <param name="a">振幅</param>
		/// <param name="omega">角速度</param>
		public ComputerCpu(double maxDt, double a, double omega)
		{
			// 粒子群を初期化
			this.inputParticles = new List<Particle>();
			this.particles = new Particle[0];

			// 時刻と時間刻みを設定
			this.T = 0;
			this.Dt = maxDt;
			this.TimeStep = 0;
			this.MaxDt = maxDt;

			// 振幅と角速度を設定
			this.A = a;
			this.Omega = omega;

			// 準備処理は何もしない
			this.prepare = () => { };
		}

		/// <summary>
		/// 粒子を追加する
		/// </summary>
		/// <param name="particle">追加する粒子</param>
		public void AddParticle(Particle particle)
		{
			// 入力粒子に追加
			this.inputParticles.Add(particle);

			// 準備処理の時に
			this.prepare = () =>
			{
				// 新しい粒子群配列を生成
				var newParticles = new Particle[this.particles.Length + this.inputParticles.Count];


				// 古い粒子群を新しい粒子群に複製
				this.particles.CopyTo(newParticles, 0);

				// 入力粒子群を新しい粒子群に複製 
				this.inputParticles.CopyTo(newParticles, this.particles.Length);

				// 新しい粒子群を今の粒子群とする
				this.particles = newParticles;


				// 入力粒子群を空にする
				this.inputParticles.Clear();

				// 準備処理は空
				this.prepare = () => { };
			};
		}

		/// <summary>
		/// 全粒子に対して操作を実行する
		/// </summary>
		/// <param name="action">操作</param>
		void EachParticle(ParticleAction action)
		{
			// 全粒子に対して実行
			this.EachParticle(0, this.ParticleCount, action);
		}

		/// <summary>
		/// 指定した番号の間の粒子に対して操作を実行する
		/// </summary>
		/// <param name="first">開始番号</param>
		/// <param name="last">終了番号</param>
		/// <param name="action">操作</param>
		void EachParticle(ulong first, ulong last, ParticleAction action)
		{
			// 各粒子について
			for(ulong i = first; i < last; i++)
			{
				// 操作を実行
				action(this.particles[i], i);
			}
		}


		/// <summary>
		/// 計算を1ステップ進める
		/// </summary>
		public void Next()
		{
			// 準備処理
			this.prepare();

			// 初期時間刻みを設定
			double dt = this.MaxDt;


			// 各粒子について
			this.EachParticle((particle, i) =>
			{
				// 加速度を設定
				particle.A.X = A * System.Math.Cos(this.Omega * T);
				particle.A.Z = A * System.Math.Cos(1 / this.Omega * T);
			});

			// 各粒子について
			this.EachParticle((particle, i) =>
			{
				// 大きさがゼロでなければ
				if(!particle.U.IsZero)
				{
					// クーラン数から計算した時間刻みを設定
					dt = System.Math.Min(dt, 0.2 * particle.D / particle.U.Length);
				}
			});

			// 各粒子について
			this.EachParticle((particle, i) =>
			{
				// 移動粒子なら
				if(particle.Type == ParticleType.FreeMovable)
				{
					// 等加速度直線運動
					particle.X += particle.U * dt + particle.A * dt * dt / 2;
					particle.U += particle.A * dt;
				}
			});

			// 時刻を進める
			this.Dt = dt;
			this.T += dt;
			this.TimeStep++;
		}

		/// <summary>
		/// 2つの値の調和平均を計算する
		/// </summary>
		/// <param name="value1">1つめの値</param>
		/// <param name="value2">2つめの値</param>
		/// <returns>調和平均値</returns>
		static double HermonicMean(double value1, double value2)
		{
			// どちらかが0であればゼロ、それ以外なら2/(1/a + 1/b)を返す
			return (value1 * value2 != 0) ?
				2.0 / (1.0 / value1 + 1.0 / value2)
				: 0;
		}

		/// <summary>
		/// 現在の粒子を取得する
		/// </summary>
		/// <returns>現在の粒子の複製</returns>
		public Particle[] GetParticles()
		{
			// 複製配列を作成して、粒子群を複製
			var clone = new Particle[this.particles.Length];
			this.particles.CopyTo(clone, 0);

			// 複製したものを返す
			return clone;
		}

		/// <summary>
		/// 粒子数を取得する
		/// </summary>
		public ulong ParticleCount
		{
			get
			{
				// 現在の粒子群の数を返す
				return (ulong)this.particles.LongLength;
			}
		}
	}
}