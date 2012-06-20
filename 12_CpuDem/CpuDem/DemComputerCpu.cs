using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using LWisteria.StudiesOfOpenTK.Math;
namespace LWisteria.StudiesOfOpenTK.CpuDem
{
	/// <summary>
	/// CPUでの個別要素法プログラム
	/// </summary>
	public class DemComputerCpu
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
		/// 重力加速度
		/// </summary>
		public readonly Vector G;

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
		/// CPUでの個別要素法プログラムを作成する
		/// </summary>
		/// <param name="maxDt">初期時間刻み</param>
		/// <param name="g">重力加速度</param>
		public DemComputerCpu(double maxDt, Vector g)
		{
			// 粒子群を初期化
			this.inputParticles = new List<Particle>();
			this.particles = new Particle[0];

			// 時刻と時間刻みを設定
			this.T = 0;
			this.Dt = maxDt;
			this.TimeStep = 0;
			this.MaxDt = maxDt;

			// 重力加速度を設定
			this.G = g;

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
			
			// 各粒子について
			this.EachParticle((particle, i) =>
			{
				// 重力加速度に設定
				particle.A = this.G;
			});

			// 各粒子について
			this.EachParticle(0, this.ParticleCount - 1, (thisParticle, i) =>
			{
				// 自分の質量を計算
				double thisM = thisParticle.M;

				// 自分より後ろの番号の粒子について
				this.EachParticle(i + 1, this.ParticleCount, (anotherParticle, j) =>
				{
					// 粒子間の距離を計算
					double interparticleDistance = (thisParticle.X - anotherParticle.X).Length - (thisParticle.D + anotherParticle.D) / 2;

					// 距離が負なら 
					if(interparticleDistance < 0)
					{
						// 変位を計算
						double delta = -interparticleDistance;

						// 各平均値を計算
						//  * ヤング係数（調和平均）
						//  * ポアソン比（調和平均）
						//  * 質量（相加平均）
						double E = DemComputerCpu.HermonicMean(thisParticle.Material.E, anotherParticle.Material.E);
						double Nu = DemComputerCpu.HermonicMean(thisParticle.Material.Nu, anotherParticle.Material.Nu);
						double M = (thisParticle.M + anotherParticle.M) / 2;


						// 逓減率
						double s0 = 1.0 / 2 / (1 + Nu);

						// ヘルツ理論および1次元臨界減衰条件から、バネ・ダッシュポットの係数を設定：
						// * 法線方向バネ：kn = √δ * 2/3 * E/(1-ν^2)*√(d1*d2/2(d1 + d2))
						// * 接線方向バネ：ks = kn・s0
						// * 法線方向ダッシュポット：cn = 2√(M・kn)
						// * 接線方向ダッシュポット：cs = cn/√s0
						double k_n = System.Math.Sqrt(System.Math.Abs(delta)) * 2.0f / 3 * E / (1 - Nu * Nu) * System.Math.Sqrt(thisParticle.D * anotherParticle.D / 2 / (thisParticle.D + anotherParticle.D));
						double k_s = System.Math.Abs(k_n) * s0;
						double c_n = 1000*2 * System.Math.Sqrt(M * System.Math.Abs(k_n));
						double c_s = c_n * System.Math.Sqrt(s0);

						// 粒子2から粒子1を向く法線方向単位ベクトルを計算
						Vector n = (thisParticle.X - anotherParticle.X);

						// 相対速度を計算
						Vector deltaU = anotherParticle.U - thisParticle.U;// +cross(thisOmega, thisD * norm) + cross(anotherOmega, anotherD * norm);
						Vector u_n = deltaU.Dot(n) * n;

						// 法線・接線方向の各バネ・ダッシュポット力を計算
						// * 法線方向バネ：en = kn・δ
						// * 接線方向バネ：es = ks・δs
						// * 法線方向ダッシュポット：dn = cn・法線方向の相対速度
						// * 接線方向ダッシュポット：ds = cs・接線方向の相対速度
						// * 接線方向バネ：
						Vector e_n = k_n * delta * n;
						Vector d_n = c_n * u_n;

						// 粒子間力を計算 
						Vector force = e_n + d_n;

						// 粒子間力を更新
						thisParticle.A += force/thisM;
						anotherParticle.A += -force/anotherParticle.M;
					}
				});
			});

			// 時間刻みを設定
			double dt = this.MaxDt;

			// 各粒子について
			this.EachParticle((particle, i) =>
			{
				// 大きさがゼロでなければ
				if(!particle.U.IsZero)
				{
					// クーラン数から計算した時間刻みを設定
					dt = System.Math.Min(dt, 0.02 * particle.D / particle.U.Length);
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