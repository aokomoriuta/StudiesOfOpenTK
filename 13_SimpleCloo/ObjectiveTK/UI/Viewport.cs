﻿using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace LWisteria.StudiesOfOpenTK.ObjectiveTK
{
	/// <summary>
	/// OpenTK.GLControlのWPF用ラッパーコントロール
	/// </summary>
	public partial class Viewport : System.Windows.Controls.UserControl
	{
		/// <summary>
		/// このコントロールで使用するプログラム群
		/// </summary>
		public List<Program> Programs { private set; get; }

		/// <summary>
		/// コントロールを作成
		/// </summary>
		public Viewport()
		{
			// コンポーネント初期化
			this.InitializeComponent();

			// プログラム群を初期化
			this.Programs = new List<Program>();

			// コントロールの初期設定
			{
				// コントロールを有効化
				this.glControl.MakeCurrent();

				// 深度を有効化
				GL.Enable(EnableCap.DepthTest);
			}


			// コントロールの描画時には
			this.glControl.Paint += (sender, e) =>
			{
				// コントロールを有効化
				this.glControl.MakeCurrent();

				// 画面をクリア
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				// 全プログラムについて
				foreach(var program in this.Programs)
				{
					// 描画
					program.Draw();
				}

				// 画面に表示
				this.glControl.SwapBuffers();
			};

			// 大きさが変わったら
			this.SizeChanged += (sender, e) =>
			{
				// コントロールを有効化
				this.glControl.MakeCurrent();

				// 画面全体を表示
				GL.Viewport(0, 0, (int)this.ActualWidth, (int)this.ActualHeight);

				// 再描画が必要であることを通知
				this.OnInvalidated();
			};

			// フォーカスを当てておく
			this.glControl.Focus();
		}

		/// <summary>
		/// 再描画する
		/// </summary>
		public void Invalidate()
		{
			// コントロールを再描画する
			this.glControl.Invalidate();
		}

		/// <summary>
		/// 再描画時に発生するイベント
		/// </summary>
		public event EventHandler Invalidated;

		/// <summary>
		/// 再描画を通知
		/// </summary>
		protected void OnInvalidated()
		{
			// イベントが空でなければ
			if(this.Invalidated != null)
			{
				// 通知
				this.Invalidated(this, new EventArgs());
			}

			// 再描画処理を実行
			this.Invalidate();
		}
	}
}