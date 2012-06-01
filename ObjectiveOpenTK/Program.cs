using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
namespace LWisteria.StudiesOfOpenTK.ObjectiveOpenTK
{
	/// <summary>
	/// OpenGLプログラム
	/// </summary>
	public class Program
	{
		/// <summary>
		/// 描画対象のコントロール
		/// </summary>
		readonly GLControl target;

		/// <summary>
		/// プログラムID
		/// </summary>
		public readonly int ID;

		/// <summary>
		/// 頂点バッファー(VBO)のID
		/// </summary>
		public int vboID;

		/// <summary>
		/// 頂点インデックスバッファー(EBO)のID
		/// </summary>
		public int eboID;

		/// <summary>
		/// 頂点配列バッファー(VAO)のID
		/// </summary>
		public int vaoID;

		/// <summary>
		/// 描画モード
		/// </summary>
		public BeginMode BeginMode { private set; get; }

		/// <summary>
		/// インデックス数
		/// </summary>
		public int IndexCount { private set; get; }

		/// <summary>
		/// プログラムを作成する
		/// </summary>
		/// <param name="control">描画対象コントロール</param>
		public Program(GLControl control)
		{
			// コントロールを設定
			this.target = control;


			// コントロールを有効化
			this.target.MakeCurrent();

			// プログラムを作成
			this.ID = GL.CreateProgram();


			// 描画対象が削除されたら
			this.target.Disposed += (sender, e) =>
			{
				// バッファー等を削除
				GL.DeleteBuffers(1, ref this.vboID);
				GL.DeleteBuffers(1, ref this.eboID);
				GL.DeleteVertexArrays(1, ref this.vaoID);

				// プログラムを削除
				GL.DeleteProgram(this.ID);
			};
		}

		/// <summary>
		/// プログラムをビルドする
		/// </summary>
		/// <param name="vertexSource">バーテックスシェーダのソース</param>
		/// <param name="geometrySource">ジオメトリシェーダのソース</param>
		/// <param name="fragmentSource">フラグメントシェーダのソース</param>
		public void Build(
			string vertexSource,
			string geometrySource,
			string fragmentSource)
		{
			// コントロールを有効化
			this.target.MakeCurrent();

			// 各シェーダーを作成して設定
			GL.AttachShader(this.ID, Program.CreateShader(vertexSource, ShaderType.VertexShader));
			GL.AttachShader(this.ID, Program.CreateShader(geometrySource, ShaderType.GeometryShader));
			GL.AttachShader(this.ID, Program.CreateShader(fragmentSource, ShaderType.FragmentShader));

			// プログラムをリンク
			GL.LinkProgram(this.ID);

			// 使用するプログラムを設定
			GL.UseProgram(this.ID);

			// インデックス数を初期化
			this.IndexCount = 0;
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
				throw new CompileShaderException(shader, type, GL.GetShaderInfoLog(shader));
			}

			// シェーダーを返す
			return shader;
		}

		/// <summary>
		/// バッファーを書き込み
		/// </summary>
		/// <typeparam name="T">書き込むオブジェクトの型</typeparam>
		/// <param name="objects">オブジェクト配列</param>
		/// <param name="indices">インデックス配列</param>
		/// <param name="beginMode">表示方法</param>
		/// <param name="attributions">属性配列</param>
		public void WriteBuffer<T>(T[] objects, uint[] indices, BeginMode beginMode, VertexAttribution[] attributions) where T : struct
		{
			// コントロールを有効化
			this.target.MakeCurrent();

			// データサイズを取得
			int sizeInByte = System.Runtime.InteropServices.Marshal.SizeOf(default(T));

			// 表示方法とインデックス数を設定
			this.BeginMode = beginMode;
			IndexCount = indices.Length;

			// 頂点バッファー(VBO)を作成して割り当て
			GL.GenBuffers(1, out this.vboID);
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.vboID);

			// 頂点インデックスバッファー(EBO)を作成して割り当て
			GL.GenBuffers(1, out this.eboID);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.eboID);

			// データを書き込み
			GL.BufferData<T>(BufferTarget.ArrayBuffer,
				new IntPtr(sizeInByte * objects.Length),
				objects,
				BufferUsageHint.StaticDraw);
			GL.BufferData<uint>(
				BufferTarget.ElementArrayBuffer,
				new IntPtr(sizeof(uint) * this.IndexCount),
				indices,
				BufferUsageHint.StaticDraw);


			// 頂点配列バッファー(VAO)を作成して割り当て
			GL.GenVertexArrays(1, out this.vaoID);
			GL.BindVertexArray(this.vaoID);

			// 各頂点属性について
			foreach(var attribution in attributions)
			{
				// シェーダー内の位置の場所を取得
				int location = GL.GetAttribLocation(this.ID, attribution.Name);

				// 位置を有効化
				GL.EnableVertexAttribArray(location);

				// 各パラメーターのオフセットを設定
				GL.VertexAttribPointer(
					location,
					attribution.Size,
					attribution.Type,
					attribution.Normalized,
					sizeInByte,
					attribution.Offset);
			}

			// 作成した頂点および頂点配列を設定
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.eboID);
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

			// 設定
			GL.UniformMatrix4(GL.GetUniformLocation(this.ID, name), transpose, ref value);
		}

		/// <summary>
		/// 性能を設定
		/// </summary>
		/// <param name="capability">性能</param>
		public void Enable(EnableCap capability)
		{
			// コントロールを有効化
			this.target.MakeCurrent();

			// 設定
			GL.Enable(capability);
		}

		/// <summary>
		/// ビューポートを設定する
		/// </summary>
		/// <param name="x">原点のx座標</param>
		/// <param name="y">原点のy座標</param>
		/// <param name="width">表示幅</param>
		/// <param name="height">表示高さ</param>
		public void Viewport(double x, double y, double width, double height)
		{
			// コントロールを有効化
			this.target.MakeCurrent();

			// 設定
			GL.Viewport((int)x, (int)y, (int)width, (int)height);
		}
	}
}