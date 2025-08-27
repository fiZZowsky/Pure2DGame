using System;
using NAudio.Wave;

namespace PureGame.Engine.Audio
{
    /// <summary>
    /// Owija WaveStream tak, by po dojściu do końca wracał do początku (loop).
    /// </summary>
    public sealed class LoopStream : WaveStream
    {
        private readonly WaveStream _sourceStream;

        public bool EnableLooping { get; set; } = true;

        public LoopStream(WaveStream sourceStream) => _sourceStream = sourceStream;

        public override WaveFormat WaveFormat => _sourceStream.WaveFormat;
        public override long Length => _sourceStream.Length;
        public override long Position
        {
            get => _sourceStream.Position;
            set => _sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (_sourceStream.Position == 0 || !EnableLooping)
                        break;
                    // Restart
                    _sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _sourceStream.Dispose();
            base.Dispose(disposing);
        }
    }
}