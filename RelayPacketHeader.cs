using System;
using System.Runtime.InteropServices;

namespace minibus
{
	[Flags]
	public enum ControlFlags : byte
	{
		None = 0,
		EndOfData = 1,
		HasTelemetry = 2,
		HasCorrectDataSize = 4,
	}

	public enum XPress9Level
	{
		None = 0,
		Level6 = 6,
		Level9 = 9,
	}

	public enum DeserializationDirective
	{
		Json = 1,
		BinaryRowset = 2,
		BinaryVarData = 3,
	}

	[StructLayout(LayoutKind.Explicit, Size = 21, Pack = 1)]
	public sealed class RelayPacketHeader
	{
		[MarshalAs(UnmanagedType.I1)]
		[FieldOffset(0)]
		private ControlFlags m_controlFlags;
		[FieldOffset(1)]
		private int m_index;
		[FieldOffset(5)]
		private int m_uncompressedDataSize;
		[FieldOffset(9)]
		private int m_compressedDataSize;
		[MarshalAs(UnmanagedType.I4)]
		[FieldOffset(13)]
		private XPress9Level m_compressionAlgorithm;
		[MarshalAs(UnmanagedType.I4)]
		[FieldOffset(17)]
		private DeserializationDirective m_deserializationDirective;

		public static int Size => Marshal.SizeOf(typeof(RelayPacketHeader));

		public ControlFlags ControlFlags
		{
			get => this.m_controlFlags;
			set => this.m_controlFlags = value;
		}

		public bool IsLast
		{
			get => (this.m_controlFlags & ControlFlags.EndOfData) == ControlFlags.EndOfData;
			set
			{
				if (value)
					this.m_controlFlags |= ControlFlags.EndOfData;
				else
					this.m_controlFlags &= ~ControlFlags.EndOfData;
			}
		}

		public bool HasTelemetry
		{
			get => (this.m_controlFlags & ControlFlags.HasTelemetry) == ControlFlags.HasTelemetry;
			set
			{
				if (value)
					this.m_controlFlags |= ControlFlags.HasTelemetry;
				else
					this.m_controlFlags &= ~ControlFlags.HasTelemetry;
			}
		}

		public bool HasCorrectDataSize
		{
			get => (this.m_controlFlags & ControlFlags.HasCorrectDataSize) == ControlFlags.HasCorrectDataSize;
			set
			{
				if (value)
					this.m_controlFlags |= ControlFlags.HasCorrectDataSize;
				else
					this.m_controlFlags &= ~ControlFlags.HasCorrectDataSize;
			}
		}

		public int Index
		{
			get => this.m_index;
			set => this.m_index = value;
		}

		public int UncompressedDataSize
		{
			get => this.m_uncompressedDataSize;
			set => this.m_uncompressedDataSize = value;
		}

		public int CompressedDataSize
		{
			get => this.m_compressedDataSize;
			set => this.m_compressedDataSize = value;
		}

		public XPress9Level CompressionAlgorithm
		{
			get => this.m_compressionAlgorithm;
			set => this.m_compressionAlgorithm = value;
		}

		public DeserializationDirective DeserializationDirective
		{
			get => this.m_deserializationDirective;
			set => this.m_deserializationDirective = value;
		}

		public bool IsInfra => this.Index == -1;

		public bool IsError => this.IsInfra && this.IsLast;

		public bool IsTelemetry => this.IsInfra && this.HasTelemetry && !this.IsLast;

		public void Serialize(byte[] packet)
		{
			GCHandle gcHandle = GCHandle.Alloc((object)packet, GCHandleType.Pinned);
			Marshal.StructureToPtr<RelayPacketHeader>(this, gcHandle.AddrOfPinnedObject(), false);
			gcHandle.Free();
		}

		public static RelayPacketHeader Deserialize(byte[] packet)
		{
			GCHandle gcHandle = GCHandle.Alloc((object)packet, GCHandleType.Pinned);
			RelayPacketHeader structure = (RelayPacketHeader)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(RelayPacketHeader));
			gcHandle.Free();
			return structure;
		}
	}
}
