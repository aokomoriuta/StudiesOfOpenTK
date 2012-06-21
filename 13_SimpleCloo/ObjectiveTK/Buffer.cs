using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LWisteria.StudiesOfOpenTK.ObjectiveTK
{
	/// <summary>
	/// バッファーオブジェクトをまとめたもの
	/// </summary>
	/// <typeparam name="T">バッファーに格納する型</typeparam>
	public class Buffer : IDisposable
	{

		/// <summary>
		/// 描画対象のコントロール
		/// </summary>
		readonly GLControl target;

		/// <summary>
		/// 頂点バッファー(VBO)
		/// </summary>
		int vertexBuffer;

		/// <summary>
		/// 要素バッファー(EBO)
		/// </summary>
		int elementBuffer;

		/// <summary>
		/// 頂点配列(VAO)
		/// </summary>
		int vertexArray;

		/// <summary>
		/// インデックス数
		/// </summary>
		readonly int indicesCount;

		/// <summary>
		/// バッファーに格納するデータの1つのサイズ
		/// </summary>
		readonly int sizeInByte;

		/// <summary>
		/// データの種類
		/// </summary>
		readonly BeginMode mode;

		/// <summary>
		/// バッファーを作成する
		/// </summary>
		/// <param name="viewport">描画対象</param>
		/// <param name="indicesCount">インデックス数</param>
		/// <param name="size">データ1つのサイズ</param>
		/// <param name="type">データの種類</param>
		Buffer(Viewport viewport, int indicesCount, int size, BeginMode type)
		{
			// 描画対象を設定
			this.target = viewport.glControl;

			// 描画対象を有効化
			this.target.MakeCurrent();


			// VBOを作成
			GL.GenBuffers(1, out this.vertexBuffer);

			// EBOを作成
			GL.GenBuffers(1, out this.elementBuffer);

			// VAOを作成
			GL.GenVertexArrays(1, out this.vertexArray);


			// インデックス数を設定
			this.indicesCount = indicesCount;

			// サイズを設定
			this.sizeInByte = size;

			// 描画モードを設定
			this.mode = type;
		}

		/// <summary>
		/// データを変更しないバッファーを作成
		/// </summary>
		/// <typeparam name="T">描画するデータの型</typeparam>
		/// <param name="viewport">描画対象</param>
		/// <param name="type">データの種類</param>
		/// <param name="objects">バッファーに格納するデータ</param>
		/// <param name="indices">インデックスの配列</param>
		public static Buffer CreateStatic<T>(Viewport viewport, BeginMode type, T[] objects, uint[] indices) where T : struct
		{
			// バッファーを作成
			var buffer = new Buffer(viewport, indices.Length, System.Runtime.InteropServices.Marshal.SizeOf(default(T)), type);

			// 描画対象を有効化
			buffer.target.MakeCurrent();


			// VAOを有効化（バインド）
			GL.BindVertexArray(buffer.vertexArray);


			// VBOを有効化（バインド）
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.vertexBuffer); 

			// VBOを確保してデータを割り当て
			GL.BufferData<T>(BufferTarget.ArrayBuffer,
				new IntPtr(buffer.sizeInByte * objects.Length),
				objects,
				BufferUsageHint.StaticDraw);


			// EBOを有効化（バインド）
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.elementBuffer);

			// EBOを確保してデータ割り当て
			GL.BufferData<uint>(
				BufferTarget.ElementArrayBuffer,
				new IntPtr(sizeof(uint) * indices.Length),
				indices,
				BufferUsageHint.StaticDraw);

			// 作成したバッファーを返す
			return buffer;
		}

		/// <summary>
		/// データを変更するバッファーを作成
		/// </summary>
		/// <typeparam name="T">描画するデータの型</typeparam>
		/// <param name="viewport">描画対象</param>
		/// <param name="type">データの種類</param>
		/// <param name="indices">インデックスの配列</param>
		public static Buffer CreateDynamic<T>(Viewport viewport, BeginMode type, int objectsCount, uint[] indices) where T : struct
		{
			// バッファーを作成
			var buffer = new Buffer(viewport, indices.Length, System.Runtime.InteropServices.Marshal.SizeOf(default(T)), type);

			// 描画対象を有効化
			buffer.target.MakeCurrent();


			// VAOを有効化（バインド）
			GL.BindVertexArray(buffer.vertexArray);


			// VBOを有効化（バインド）
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.vertexBuffer);

			// VBOを確保
			GL.BufferData(BufferTarget.ArrayBuffer,
				new IntPtr(buffer.sizeInByte * objectsCount),
				IntPtr.Zero,
				BufferUsageHint.DynamicDraw);


			// EBOを有効化（バインド）
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.elementBuffer);

			// EBOを確保してデータ割り当て
			GL.BufferData<uint>(
				BufferTarget.ElementArrayBuffer,
				new IntPtr(sizeof(uint) * indices.Length),
				indices,
				BufferUsageHint.StaticDraw);

			// 作成したバッファーを返す
			return buffer;
		}

		/// <summary>
		/// データを書き込む
		/// </summary>
		/// <typeparam name="T">描画するデータの型</typeparam>
		/// <param name="objects">書き込むデータ</param>
		public void WriteData<T>(T[] objects) where T : struct
		{
			// 描画対象を有効化
			this.target.MakeCurrent();

			// VBOを有効化（バインド）
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);

			// VBOに書き込み
			GL.BufferSubData<T>(BufferTarget.ArrayBuffer,
				IntPtr.Zero,
				new IntPtr(this.sizeInByte * objects.Length),
				objects);
		}

		/// <summary>
		/// 頂点属性のプログラム中の位置を設定する
		/// </summary>
		/// <param name="attribution">頂点属性</param>
		/// <param name="location">位置</param>
		public void SetLocation(VertexAttribution attribution, int location)
		{
			// 描画対象を有効化
			this.target.MakeCurrent();

			// VAOを有効化
			GL.BindVertexArray(this.vertexArray);

			// VBOを有効化（バインド）
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);


			// 属性値を有効化
			GL.EnableVertexAttribArray(location);

			// パラメーターの位置を設定
			GL.VertexAttribPointer(
				location,
				attribution.Size,
				attribution.Type,
				attribution.Normalized,
				this.sizeInByte,
				attribution.Offset);
		}

		/// <summary>
		/// バッファーのデータを描画する
		/// </summary>
		public void Draw()
		{
			// VAOとEBOを有効化
			GL.BindVertexArray(this.vertexArray);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.elementBuffer);

			// 描画
			GL.DrawElements(this.mode, this.indicesCount, DrawElementsType.UnsignedInt, 0);
		}


		/// <summary>
		/// バッファーを解放する
		/// </summary>
		public void Dispose()
		{
			// バッファー等を削除
			GL.DeleteBuffers(1, ref this.vertexBuffer);
			GL.DeleteBuffers(1, ref this.elementBuffer);
			GL.DeleteBuffers(1, ref this.vertexArray);
		}
	}
}