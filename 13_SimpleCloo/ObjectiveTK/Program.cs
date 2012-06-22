using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LWisteria.StudiesOfOpenTK.ObjectiveTK
{
	/// <summary>
	/// プログラム本体
	/// </summary>
	public class Program : IDisposable
	{
		/// <summary>
		/// 描画対象のコントロール
		/// </summary>
		readonly GLControl target;

		/// <summary>
		/// プログラム本体のID
		/// </summary>
		readonly int ID;

		/// <summary>
		/// このプログラムで描画するバッファー群
		/// </summary>
		readonly List<Buffer> buffers;


		/// <summary>
		/// プログラムを作成する
		/// </summary>
		/// <param name="viewport">描画対象</param>
		/// <param name="vertexSource">バーテックスシェーダのソース</param>
		/// <param name="geometrySource">ジオメトリシェーダのソース</param>
		/// <param name="fragmentSource">フラグメントシェーダのソース</param>
		public Program(
			Viewport viewport,
			string vertexSource,
			string geometrySource,
			string fragmentSource)
		{
			// コントロールを設定
			this.target = viewport.glControl;

			// コントロールを有効化
			this.target.MakeCurrent();

			// プログラムを作成
			this.ID = GL.CreateProgram();

			// 各シェーダーを作成して設定
			GL.AttachShader(this.ID, Program.CreateShader(vertexSource, ShaderType.VertexShader));
			GL.AttachShader(this.ID, Program.CreateShader(geometrySource, ShaderType.GeometryShader));
			GL.AttachShader(this.ID, Program.CreateShader(fragmentSource, ShaderType.FragmentShader));

			// プログラムをリンク
			GL.LinkProgram(this.ID);

			// バッファー群を初期化
			this.buffers = new List<Buffer>();
		}

		/// <summary>
		/// シェーダーを作成する
		/// </summary>
		/// <param name="source">シェーダーのソース</param>
		/// <param name="type">シェーダーの種類</param>
		/// <returns>シェーダーID</returns>
		public static int CreateShader(string source, ShaderType type)
		{
			// シェーダーを作成
			int shader = GL.CreateShader(type);

			// ソースから読み込んでコンパイル
			GL.ShaderSource(shader, source);
			GL.CompileShader(shader);

			// シェーダーのコンパイル状態を取得
			int status;
			GL.GetShader(shader, ShaderParameter.CompileStatus, out status);

			// コンパイルに成功していなかったら
			if(status != 1)
			{
				// 例外
				var ex = new CompileShaderException(shader, type, GL.GetShaderInfoLog(shader));
				throw ex;
			}

			// シェーダーを返す
			return shader;
		}

		/// <summary>
		/// バッファーにこのプログラムを割り当てる
		/// </summary>
		/// <param name="buffer">割り当てるバッファー</param>
		/// <param name="attribution">頂点属性の設定値</param>
		public void AttachBuffer(Buffer buffer, VertexAttribution[] attributions)
		{
			// バッファー群に追加
			this.buffers.Add(buffer);

			// 自分のプログラムを使用
			GL.UseProgram(this.ID);

			// 全頂点属性について
			foreach(var attribution in attributions)
			{
				// その位置を設定
				buffer.SetLocation(attribution, GL.GetAttribLocation(this.ID, attribution.Name));
			}
		}

		/// <summary>
		/// バッファーをすべて除去する
		/// </summary>
		public void ClearBuffer()
		{
			// 全バッファーについて
			foreach(var buffer in this.buffers)
			{
				// バッファーを削除
				buffer.Dispose();
			}

			// バッファー群から全除去
			buffers.Clear();
		}


		/// <summary>
		/// floatスカラーのUniform変数を設定する
		/// </summary>
		/// <param name="name">変数名</param>
		/// <param name="value">設定値</param>
		public void SetUniform(string name, float value)
		{
			// コントロールを有効化
			this.target.MakeCurrent();

			// 自分のプログラムを使用
			GL.UseProgram(this.ID);

			// 設定
			GL.Uniform1(GL.GetUniformLocation(this.ID, name), value);
		}

		/// <summary>
		/// 3次元ベクトルのUniform変数を設定する
		/// </summary>
		/// <param name="name">変数名</param>
		/// <param name="value">設定値</param>
		public void SetUniform(string name, Vector3 value)
		{
			// コントロールを有効化
			this.target.MakeCurrent();

			// 自分のプログラムを使用
			GL.UseProgram(this.ID);

			// 設定
			GL.Uniform3(GL.GetUniformLocation(this.ID, name), value);
		}

		/// <summary>
		/// 4x4行列のUniform変数を設定する
		/// </summary>
		/// <param name="name">変数名</param>
		/// <param name="value">設定値</param>
		/// <param name="transpose"></param>
		public void SetUniform(string name, Matrix4 value, bool transpose = false)
		{
			// コントロールを有効化
			this.target.MakeCurrent();

			// 自分のプログラムを使用
			GL.UseProgram(this.ID);

			// 設定
			GL.UniformMatrix4(GL.GetUniformLocation(this.ID, name), transpose, ref value);
		}

		/// <summary>
		/// 設定を有効化または無効化する
		/// </summary>
		/// <param name="capabilities">設定</param>
		public void Enable(EnableCap capabilities)
		{
			// コントロールを有効化
			this.target.MakeCurrent();

			// 自分のプログラムを使用
			GL.UseProgram(this.ID);

			// 設定する
			GL.Enable(capabilities);
		}

		/// <summary>
		/// このプログラムですべてのデータを描画する
		/// </summary>
		public void Draw()
		{
			// プログラムを有効化
			GL.UseProgram(this.ID);

			// 全バッファーに対して
			foreach(var buffer in this.buffers)
			{
				// 描画
				buffer.Draw();
			}
		}

		/// <summary>
		/// リソースを解放する
		/// </summary>
		public void Dispose()
		{
			// 全バッファーを削除
			this.ClearBuffer();

			// プログラムを削除する
			GL.DeleteProgram(this.ID);
		}
	}
}