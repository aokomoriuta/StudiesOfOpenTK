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
		/// 注視点
		/// </summary>
		Vector3 lookAt;

		/// <summary>
		/// カメラを作成する
		/// </summary>
		public Camera()
		{
			// パラメーターを初期化
			this.r = 100;
			this.theta = 0;
			this.phi = 0;
			this.lookAt = new Vector3(0, 0, 0);
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

				// カメラ変更を通知
				this.OnCameraChanged();
			}
		}

		/// <summary>
		/// 注視点のX座標を取得または設定する
		/// </summary>
		public double LookAtX
		{
			get
			{
				return this.lookAt.X;
			}
			set
			{
				// 設定
				this.lookAt.X = (float)value;

				// カメラ変更を通知
				this.OnCameraChanged();
			}
		}

		/// <summary>
		/// 注視点のY座標を取得または設定する
		/// </summary>
		public double LookAtY
		{
			get
			{
				return this.lookAt.Y;
			}
			set
			{
				// 設定
				this.lookAt.Y = (float)value;

				// カメラ変更を通知
				this.OnCameraChanged();
			}
		}

		/// <summary>
		/// 注視点のZ座標を取得または設定する
		/// </summary>
		public double LookAtZ
		{
			get
			{
				return this.lookAt.Z;
			}
			set
			{
				// 設定
				this.lookAt.Z = (float)value;

				// カメラ変更を通知
				this.OnCameraChanged();
			}
		}

		/// <summary>
		/// 注視点の座標を取得する
		/// </summary>
		public Vector3 LookAt
		{
			get
			{
				// 新しく作成して返す
				return new Vector3(this.lookAt);
			}
		}

		/// <summary>
		/// カメラの位置を取得する
		/// </summary>
		public Vector3 Position
		{
			get
			{
				// カメラ位置を計算して返す
				return this.lookAt + (float)this.R * new Vector3(
					(float)(Math.Cos(this.Theta) * Math.Cos(this.Phi)),
					(float)(Math.Sin(this.Theta) * Math.Cos(this.Phi)),
					(float)(Math.Sin(this.Phi)));
			}
		}

	}
}