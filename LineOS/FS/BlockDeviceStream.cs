using System;
using System.IO;
using Cosmos.HAL.BlockDevice;

namespace LineOS.FS
{
    public class BlockDeviceStream : Stream
    {
        private Partition blockDevice;

        private long BlockSize => (long)blockDevice.BlockSize;

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length { get; }
        public override long Position { get; set; }

        private ulong currentBlockId;
        private ulong currentBlockOffset;
        private byte[] currentBlock;

        public BlockDeviceStream(Partition blockDevice, long length)
        {
            this.blockDevice = blockDevice;
            Length = length;
            currentBlock = blockDevice.NewBlockArray(1);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = 0;
            while (read < count)
            {
                buffer[offset + read] = currentBlock[currentBlockOffset];
                currentBlockOffset++;
                if (currentBlockOffset >= (ulong)currentBlock.Length)
                {
                    currentBlockId++;
                    currentBlockOffset = 0;
                    LoadBlock();
                }
                read++;
                Position++;
            }
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length - 1 - offset;
                    break;
                default:
                    throw new ArgumentException("origin");
            }
            currentBlockId = (ulong)Math.Floor((double)(Position / BlockSize));
            currentBlockOffset = (ulong)(Position % BlockSize);
            LoadBlock();
            return Position;
        }

        private void LoadBlock()
        {
            blockDevice.ReadBlock(currentBlockId, 1, currentBlock);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new Exception("Unable to SetLength on BLOCK_DEVICE_STREAM");
        }

    }
}
