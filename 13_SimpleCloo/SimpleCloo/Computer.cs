using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using LWisteria.StudiesOfOpenTK.Math;
namespace LWisteria.StudiesOfOpenTK.SimpleCloo
{
	/// <summary>
	/// 計算プログラム
	/// </summary>
	public abstract class Computer
	{
		/// <summary>
		/// 入力粒子群
		/// </summary>
		protected List<Particle> inputParticles { private set; get; }

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
		/// CPUでの計算プログラムを作成する
		/// </summary>
		/// <param name="maxDt">初期時間刻み</param>
		/// <param name="a">振幅</param>
		/// <param name="omega">角速度</param>
		public Computer(double maxDt, double a, double omega)
		{
			// 粒子群を初期化
			this.inputParticles = new List<Particle>();

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
		/// 時間刻みを設定し、時刻を進める
		/// </summary>
		/// <param name="dt">時間刻み</param>
		protected void UpdateTime(double dt)
		{
			// 時刻を進める
			this.Dt = dt;
			this.T += dt;
			this.TimeStep++;
		}

		/// <summary>
		/// 粒子を追加する
		/// </summary>
		/// <param name="particle">追加する粒子</param>
		public void AddParticle(Particle particle)
		{
			// 入力粒子に追加
			this.inputParticles.Add(particle);

			// 粒子が追加されたことを通知
			this.OnParticleAdded();
		}

		/// <summary>
		/// 計算を1ステップ進める
		/// </summary>
		public abstract void Next();


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
		/// 粒子が追加された時に発生するイベント
		/// </summary>
		protected event EventHandler ParticleAdded;

		/// <summary>
		/// 粒子が追加されたことを通知する
		/// </summary>
		protected void OnParticleAdded()
		{
			// イベントが空でなければ
			if(this.ParticleAdded != null)
			{
				// 通知
				this.ParticleAdded(this, new EventArgs());
			}
		}

		/// <summary>
		/// 現在の粒子を取得する
		/// </summary>
		/// <returns>現在の粒子の複製</returns>
		public abstract Particle[] GetParticles();

		/// <summary>
		/// 粒子数を取得する
		/// </summary>
		public abstract ulong ParticleCount { get; }
	}
}