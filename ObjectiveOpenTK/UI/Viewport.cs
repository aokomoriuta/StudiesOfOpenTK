using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LWisteria.StudiesOfOpenTK.ObjectiveOpenTK
{
	/// <summary>
	/// OpenTK.GLControlのWPF用ラッパーコントロール
	/// </summary>
	public partial class Viewport : System.Windows.Controls.UserControl
	{
		/// <summary>
		/// プログラムオブジェクト
		/// </summary>
		public Program Program { private set; get; }

		/// <summary>
		/// コントロールを作成
		/// </summary>
		public Viewport()
		{
			// コンポーネント初期化
			this.InitializeComponent();

			// プログラムを作成
			this.Program = new Program(this.glControl);

			// コントロールの描画時には
			this.glControl.Paint += (sender, e) =>
			{
				// インデックスがあれば
				if(this.Program.IndexCount != 0)
				{
					// コントロールを有効化
					this.glControl.MakeCurrent();

					// 画面をクリア
					GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

					// 描画
					GL.DrawElements(this.Program.BeginMode, this.Program.IndexCount, DrawElementsType.UnsignedInt, 0);

					// バッファーを交換
					this.glControl.SwapBuffers();
				}
			};

			// 大きさが変わったら
			this.SizeChanged += (sender, e) =>
			{
				// コントロールを有効化
				this.glControl.MakeCurrent();

				// 画面全体を表示
				this.Program.Viewport(0, 0, this.ActualWidth, this.ActualHeight);

				// コントロールを再描画
				this.glControl.Invalidate();
			};
		}
	}
}