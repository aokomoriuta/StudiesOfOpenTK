using OpenTK.Graphics.OpenGL;
using OpenTK;
using System;
namespace LWisteria.StudiesOfOpenTK.ObjectiveTK
{
	/// <summary>
	/// カメラ
	/// </summary>
	public class Camera
	{
		/// <summary>
		/// カメラの位置
		/// </summary>
		public Vector3 Position { get; private set; }

		/// <summary>
		/// カメラの距離
		/// </summary>
		double r;

		/// <summary>
		/// 水平角
		/// </summary>
		double theta;

		/// <summary>
		/// 仰角
		/// </summary>
		double phi;

		/// <summary>
		/// カメラを作成する
		/// </summary>
		public Camera()
		{
			// パラメーターを初期化
			this.r = 100;
			this.theta = 1;
			this.phi = 1;

			// 位置を更新
			this.UpdatePosition();
		}

		/// <summary>
		/// 位置を更新する
		/// </summary>
		private void UpdatePosition()
		{
			// カメラ位置を設定
			this.Position = new Vector3(
				(float)(Math.Cos(this.Theta) * Math.Cos(this.Phi)),
				(float)(Math.Sin(this.Theta) * Math.Cos(this.Phi)),
				(float)(Math.Sin(this.Phi)));
		}

		/// <summary>
		/// カメラが動いた時に発生するイベント
		/// </summary>
		public event EventHandler Changed;

		/// <summary>
		/// カメラが動いたことを通知する
		/// </summary>
		protected void OnCameraChanged()
		{
			// イベントが有効なら
			if(this.Changed != null)
			{
				// 通知
				this.Changed(this, new EventArgs());
			}
		}

		/// <summary>
		/// カメラの注視点からの距離を取得または設定する
		/// </summary>
		public double R
		{
			get
			{
				return this.r;
			}
			set
			{
				// 設定
				this.r = Math.Max(value, 0);

				// カメラ変更を通知
				this.OnCameraChanged();
			}
		}

		/// <summary>
		/// 水平角を取得または設定する
		/// </summary>
		public double Theta
		{
			get
			{
				return this.theta;
			}
			set
			{
				// 設定
				this.theta = value;

				// 水平角は0から2πまで
				this.theta = (this.theta >= 0) ? this.theta : 2 * Math.PI + this.theta;
				this.theta = (this.theta <= 2 * Math.PI) ? this.theta : this.theta - 2 * Math.PI;

				// カメラの位置を計算
				this.UpdatePosition();

				// カメラ変更を通知
				this.OnCameraChanged();
			}
		}

		/// <summary>
		/// 仰角を取得または設定する
		/// </summary>
		public double Phi
		{
			get
			{
				return this.phi;
			}
			set
			{
				// 設定
				this.phi = value;

				// 仰角は-π/2からπ/2まで
				this.phi = Math.Max(-Math.PI / 2, this.phi);
				this.phi = Math.Min(this.phi, Math.PI / 2);

				// カメラの位置を計算
				this.UpdatePosition();

				// カメラ変更を通知
				this.OnCameraChanged();
			}
		}
	}
}