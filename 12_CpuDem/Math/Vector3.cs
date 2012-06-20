namespace LWisteria.StudiesOfOpenTK.Math
{
	/// <summary>
	/// 3次元ベクトル
	/// </summary>
	sealed public class Vector
	{
		/// <summary>
		/// x成分
		/// </summary>
		public double X;

		/// <summary>
		/// y成分
		/// </summary>
		public double Y;
		
		/// <summary>
		/// z成分
		/// </summary>
		public double Z;

		/// <summary>
		/// ゼロベクトルを生成する
		/// </summary>
		public Vector()
		{
			// 各成分ゼロ
			this.X = 0;
			this.Y = 0;
			this.Z = 0;
		}

		/// <summary>
		/// 各成分を指定してベクトルを生成する
		/// </summary>
		/// <param name="x">x成分</param>
		/// <param name="y">y成分</param>
		/// <param name="z">z成分</param>
		public Vector(double x, double y, double z)
		{
			// 各成分に代入
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		/// <summary>
		/// OpenVectorK.Vector3へ変換する
		/// </summary>
		/// <param name="source">変換する元</param>
		/// <returns>変換した値</returns>
		public static explicit operator OpenTK.Vector3(LWisteria.StudiesOfOpenTK.Math.Vector source)
		{
			// floatにして作成して返す
			return new OpenTK.Vector3((float)source.X, (float)source.Y, (float)source.Z);
		}


		#region メソッド
		/// <summary>
		/// Equalsメソッドのオーバーライド
		/// </summary>
		/// <param name="obj">比較するオブジェクト</param>
		/// <returns>等しければtrue</returns>
		public override bool Equals(object obj)
		{
			// ベクトルを取得
			Vector vector = obj as Vector;

			// オブジェクトがベクトルでなければ
			if(obj == null)
			{
				// 基底クラスでの比較
				return base.Equals(obj);
			}

			// 要素同士の比較
			return (this.X == vector.X) && (this.Y == vector.Y) && (this.Z == vector.Z);
		}

		/// <summary>
		/// GetHashGode()のオーバーライド
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}
		#endregion

		#region 演算子メソッド
		/// <summary>
		/// 加算
		/// </summary>
		/// <param name="vector">ベクトル</param>
		/// <returns>各成分同士の和</returns>
		public Vector Add(Vector vector)
		{
			return new Vector(this.X + vector.X, this.Y + vector.Y, this.Z + vector.Z);
		}

		/// <summary>
		/// スカラーとの積
		/// </summary>
		/// <param name="scalar">スカラー</param>
		/// <returns>各成分にスカラーをかけた値</returns>
		public Vector Product(double scalar)
		{
			return new Vector(this.X * scalar, this.Y * scalar, this.Z * scalar);
		}

		/// <summary>
		/// ドット積（内積）
		/// </summary>
		/// <param name="vector">ベクトル</param>
		/// <returns>各要素の積の総和</returns>
		public double Dot(Vector vector)
		{
			return this.X * vector.X + this.Y * vector.Y + this.Z * vector.Z;
		}
		#endregion

		#region 演算子・コンバータメソッド
		/// <summary>
		/// -単項演算子
		/// </summary>
		/// <param name="vector">ベクトル</param>
		/// <returns>vectorの符号が反転したもの</returns>
		public static Vector operator -(Vector vector)
		{
			// -1をかけたものを返す
			return -1 * vector;
		}

		/// <summary>
		/// +二項演算子（加法）
		/// </summary>
		/// <param name="leftVector">演算子の左のベクトル</param>
		/// <param name="rightVector">演算子の右のベクトル</param>
		/// <returns>leftVectorとrightVectorの和</returns>
		public static Vector operator +(Vector leftVector, Vector rightVector)
		{
			// 左辺と右辺を足して返す
			return leftVector.Add(rightVector);
		}

		/// <summary>
		/// -二項演算子（減法）
		/// </summary>
		/// <param name="leftVector">演算子の左のベクトル</param>
		/// <param name="rightVector">演算子の右のベクトル</param>
		/// <returns>leftVectorとrightVectorの差</returns>
		public static Vector operator -(Vector leftVector, Vector rightVector)
		{
			// 右辺に-1をかけたものを左辺に足す
			return leftVector + -1 * rightVector;
		}


		/// <summary>
		/// スカラーとの*二項演算子（乗法）-左がスカラーの場合
		/// </summary>
		/// <param name="scalar">スカラー</param>
		/// <param name="vector">ベクトル</param>
		/// <returns>scalarとvectorの積</returns>
		public static Vector operator *(double scalar, Vector vector)
		{
			// 結果を返す
			return vector.Product(scalar);
		}

		/// <summary>
		/// スカラーとの*二項演算子（乗法）-右がスカラーの場合
		/// </summary>
		/// <param name="vector">ベクトル</param>
		/// <param name="scalar">スカラー</param>
		/// <returns>scalarとvectorの積</returns>
		public static Vector operator *(Vector vector, double scalar)
		{
			// 結果を返す
			return vector.Product(scalar);
		}

		/// <summary>
		/// スカラーとの/二項演算子（除法）
		/// </summary>
		/// <param name="vector">ベクトル</param>
		/// <param name="scalar">スカラー</param>
		/// <returns>vectorとscalarの商</returns>
		public static Vector operator /(Vector vector, double scalar)
		{
			// 与えられたスカラーの逆数をベクトルにかける
			return 1 / scalar * vector;
		}


		/// <summary>
		/// ==演算子（比較演算子）
		/// </summary>
		/// <param name="vec1">比較するベクトル</param>
		/// <param name="vec2">比較されるベクトル</param>
		/// <returns>2つのベクトルが等しいかどうか</returns>
		public static bool operator ==(Vector vec1, Vector vec2)
		{
			return vec1.Equals(vec2);
		}

		/// <summary>
		/// !=演算子（比較演算子）
		/// </summary>
		/// <param name="vec1">比較するベクトル</param>
		/// <param name="vec2">比較されるベクトル</param>
		/// <returns>2つのベクトルが等しくないかどうか</returns>
		public static bool operator !=(Vector vec1, Vector vec2)
		{
			return !vec1.Equals(vec2);
		}

		#endregion

		#region プロパティ
		/// <summary>
		/// 自分自身との内積を取得する
		/// </summary>
		/// <returns>大きさの2乗</returns>
		public double Length2
		{
			get
			{
				// 自分自身との内積
				return this.Dot(this);
			}
		}

		/// <summary>
		/// 大きさを取得する
		/// </summary>
		/// <returns>絶対値（大きさ）</returns>
		public double Length
		{
			get
			{
				// 自分自身との内積のルート
				return System.Math.Sqrt(this.Length2);
			}
		}

		/// <summary>
		/// ゼロベクトルかどうかを取得する
		/// </summary>
		public bool IsZero
		{
			get
			{
				// 大きさがゼロかどうか
				return this.Length2 == 0;
			}
		}
		#endregion
	}
}