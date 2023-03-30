namespace minibus
{
	public sealed class RelayPacket
	{
		private byte[] m_blob;

		public static int HeaderSize => RelayPacketHeader.Size;

		public RelayPacketHeader Header { get; private set; }

		public byte[] Blob
		{
			get => this.m_blob;
			set
			{
				this.m_blob = value;
				this.ExtractHeader();
			}
		}

		public bool RowsetPacketLimitReached { get; set; }

		public int PhysicalDataSize => RelayPacket.GetPhysicalDataSize(this.LogicalDataSize);

		public int LogicalDataSize => this.Header.CompressedDataSize;

		public static int GetPhysicalDataSize(int logicalDataSize) => logicalDataSize + RelayPacketHeader.Size;

		public static int GetLogicalDataSize(int physicalDataSize) => physicalDataSize - RelayPacketHeader.Size;

		public static int GetLogicalDataSize(byte[] packetBlob) => RelayPacket.GetLogicalDataSize(packetBlob.Length);

		private void ExtractHeader() => this.Header = RelayPacketHeader.Deserialize(this.Blob);
	}
}
