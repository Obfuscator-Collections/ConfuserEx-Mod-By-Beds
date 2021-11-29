using System;
using System.IO;
using dnlib.IO;

namespace Confuser.Core
{
	/// <summary>
	///     <see cref="T:System.IO.Stream" /> wrapper of <see cref="T:dnlib.IO.IImageStream" />.
	/// </summary>
	// Token: 0x0200003E RID: 62
	public class ImageStream : Stream
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ImageStream" /> class.
		/// </summary>
		/// <param name="baseStream">The base stream.</param>
		// Token: 0x06000161 RID: 353 RVA: 0x0000BE75 File Offset: 0x0000A075
		public ImageStream(IImageStream baseStream)
		{
			this.BaseStream = baseStream;
		}

		/// <summary>
		///     Gets the base stream of this instance.
		/// </summary>
		/// <value>The base stream.</value>
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000162 RID: 354 RVA: 0x0000BE84 File Offset: 0x0000A084
		// (set) Token: 0x06000163 RID: 355 RVA: 0x0000BE8C File Offset: 0x0000A08C
		public IImageStream BaseStream
		{
			get;
			private set;
		}

		/// <inheritdoc />
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000164 RID: 356 RVA: 0x0000BE95 File Offset: 0x0000A095
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		/// <inheritdoc />
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000165 RID: 357 RVA: 0x0000BE98 File Offset: 0x0000A098
		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		/// <inheritdoc />
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000166 RID: 358 RVA: 0x0000BE9B File Offset: 0x0000A09B
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		/// <inheritdoc />
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000167 RID: 359 RVA: 0x0000BE9E File Offset: 0x0000A09E
		public override long Length
		{
			get
			{
				return this.BaseStream.Length;
			}
		}

		/// <inheritdoc />
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000168 RID: 360 RVA: 0x0000BEAB File Offset: 0x0000A0AB
		// (set) Token: 0x06000169 RID: 361 RVA: 0x0000BEB8 File Offset: 0x0000A0B8
		public override long Position
		{
			get
			{
				return this.BaseStream.Position;
			}
			set
			{
				this.BaseStream.Position = value;
			}
		}

		/// <inheritdoc />
		// Token: 0x0600016A RID: 362 RVA: 0x0000BEC6 File Offset: 0x0000A0C6
		public override void Flush()
		{
		}

		/// <inheritdoc />
		// Token: 0x0600016B RID: 363 RVA: 0x0000BEC8 File Offset: 0x0000A0C8
		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.BaseStream.Read(buffer, offset, count);
		}

		/// <inheritdoc />
		// Token: 0x0600016C RID: 364 RVA: 0x0000BED8 File Offset: 0x0000A0D8
		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				this.BaseStream.Position = offset;
				break;
			case SeekOrigin.Current:
				this.BaseStream.Position += offset;
				break;
			case SeekOrigin.End:
				this.BaseStream.Position = this.BaseStream.Length + offset;
				break;
			}
			return this.BaseStream.Position;
		}

		/// <inheritdoc />
		// Token: 0x0600016D RID: 365 RVA: 0x0000BF41 File Offset: 0x0000A141
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		// Token: 0x0600016E RID: 366 RVA: 0x0000BF48 File Offset: 0x0000A148
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
	}
}
