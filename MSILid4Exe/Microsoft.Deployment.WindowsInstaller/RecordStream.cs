using System;
using System.IO;

namespace Microsoft.Deployment.WindowsInstaller
{
	internal class RecordStream : Stream
	{
		private Record record;

		private int field;

		private long position;

		public override bool CanRead => true;

		public override bool CanWrite => false;

		public override bool CanSeek => false;

		public override long Length => record.GetDataSize(field);

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		internal RecordStream(Record record, int field)
		{
			this.record = record;
			this.field = field;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			checked
			{
				if (count > 0)
				{
					byte[] sourceArray = ((offset == 0) ? buffer : new byte[count]);
					uint cbDataBuf = (uint)count;
					uint num = RemotableNativeMethods.MsiRecordReadStream((int)record.Handle, (uint)field, buffer, ref cbDataBuf);
					if (num != 0)
					{
						throw InstallerException.ExceptionFromReturnCode(num);
					}
					count = (int)cbDataBuf;
					if (offset > 0)
					{
						Array.Copy(sourceArray, 0, buffer, offset, count);
					}
					position += count;
				}
				return count;
			}
		}

		public override void Write(byte[] array, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override string ToString()
		{
			return "[Binary data]";
		}
	}
}
