using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace bsn.ModuleStore {
	public class HashWriter: TextWriter, IEquatable<HashWriter> {
		public static bool HashEqual(byte[] x, byte[] y) {
			if ((x != null) && (y != null) && (x.Length == y.Length)) {
				for (int i = 0; i < x.Length; i++) {
					if (x[i] != y[i]) {
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public static int ToHashCode(byte[] hash) {
			if (hash == null) {
				throw new ArgumentNullException("hash");
			}
			int result = 0;
			for (int position = 0; position < hash.Length; position += 4) {
				result ^= BitConverter.ToInt32(hash, position);
			}
			return result;
		}

		private readonly HashAlgorithm hash;
		private byte[] result;

		public HashWriter(): base(CultureInfo.InvariantCulture) {
			hash = SHA1.Create();
			hash.Initialize();
		}

		public override Encoding Encoding {
			get {
				return Encoding.Unicode;
			}
		}

		public byte[] ToArray() {
			if (result == null) {
				hash.TransformFinalBlock(new byte[0], 0, 0);
				result = (byte[])hash.Hash.Clone();
			}
			return result;
		}

		public override void Write(char value) {
			AssertNoResult();
			byte[] data = Encoding.GetBytes(new[] {value});
			hash.TransformBlock(data, 0, data.Length, null, 0);
		}

		public override void Write(char[] buffer, int index, int count) {
			byte[] data = Encoding.GetBytes(buffer, index, count);
			hash.TransformBlock(data, 0, data.Length, null, 0);
		}

		public override void Write(string value) {
			byte[] data = Encoding.GetBytes(value);
			hash.TransformBlock(data, 0, data.Length, null, 0);
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				((IDisposable)hash).Dispose();
			}
			base.Dispose(disposing);
		}

		private void AssertNoResult() {
			if (result != null) {
				throw new InvalidOperationException("The hash has already been computed");
			}
		}

		public bool Equals(HashWriter other) {
			if (other != null) {
				return Equals(other.hash.Hash);
			}
			return false;
		}
	}
}