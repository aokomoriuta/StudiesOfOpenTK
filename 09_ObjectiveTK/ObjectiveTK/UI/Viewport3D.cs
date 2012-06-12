using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using MouseButtons = System.Windows.Forms.MouseButtons;

namespace LWisteria.StudiesOfOpenTK.ObjectiveTK
{
	/// <summary>
	/// 3次元カメラ操作付きビューポート
	/// </summary>
	public class Viewport3D : Viewport
	{
		/// <summary>
		/// カメラ
		/// </summary>
		public readonly Camera Camera;

		/// <summary>
		/// コントロールを作成する
		/// </summary>
		public Viewport3D()
		{
			// カメラを作成
			this.Camera = new Camera();

			// コントロール上でマウスホイールされたら
			this.glControl.MouseWheel += (sender, e) =>
			{
				// 距離を変更
				this.Camera.R *= Math.Pow(1.5, Math.Sign(e.Delta));
			};

			// 以前のマウス位置
			Vector2? oldMouseLocation = null;

			// マウスが動いたら
			this.glControl.MouseMove += (sender, e) =>
			{
				// 左ボタンか中央ボタンが押されていたら
				if((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Middle))
				{
					// マウス位置を取得
					var thisMouseLocation = new Vector2(e.X, e.Y);

					// 一番最初でなければ
					if(oldMouseLocation != null)
					{
						// 移動量を計算
						var delta = thisMouseLocation - oldMouseLocation.Value;

						// 左ドラッグなら
						if(e.Button == MouseButtons.Left)
						{
							// 角度を変更
							this.Camera.Theta += -2 * Math.PI * delta.X / this.glControl.Width;
							this.Camera.Phi += Math.PI * delta.Y / this.glControl.Height;
						}
						// 中央ボタンなら
						else if(e.Button == MouseButtons.Middle)
						{
							// ずれ量を調整
							delta.X /= (float)this.Camera.R;
							delta.Y /= (float)this.Camera.R;

							// カメラの方向余弦取得
							double cTheta = System.Math.Cos(this.Camera.Theta);
							double sTheta = System.Math.Sin(this.Camera.Theta);
							double cPhi = System.Math.Cos(this.Camera.Phi);
							double sPhi = System.Math.Sin(this.Camera.Phi);

							// 水平方向に移動
							this.Camera.LookAtX += +delta.X * sTheta - delta.Y * cTheta * sPhi;
							this.Camera.LookAtY += -delta.X * cTheta - delta.Y * sTheta * sPhi;
							this.Camera.LookAtZ +=                   + delta.Y * cPhi;
						}
					}

					// 前の位置を覚えておく
					oldMouseLocation = thisMouseLocation;
				}
				// それ以外の場合
				else
				{
					// マウス位置を初期化
					oldMouseLocation = null;
				}
			};
		}

		/// <summary>
		/// ビューを取得する
		/// </summary>
		public Matrix4 View
		{
			get
			{
				// カメラの位置から計算して作成
				return Matrix4.LookAt(this.Camera.Position,
					this.Camera.LookAt,
					new Vector3(
						(float)(-System.Math.Cos(this.Camera.Theta)*System.Math.Sin(this.Camera.Phi)),
						(float)(-System.Math.Sin(this.Camera.Theta)*System.Math.Sin(this.Camera.Phi)),
						(float)System.Math.Cos(this.Camera.Phi)));
			}
		}

		/// <summary>
		/// 投影法を取得する
		/// </summary>
		public Matrix4 Projection
		{
			get
			{
				// カメラの距離から計算して作成
				return Matrix4.CreateOrthographic(
					(float)(this.ActualWidth / this.Camera.R),
					(float)(this.ActualHeight / this.Camera.R),
					-20, 20);
			}
		}
	}
}