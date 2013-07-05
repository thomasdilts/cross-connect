using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sword.reader
{
    using System.IO;

    using ComponentAce.Compression.Libs.zlib;

    public class SapphireStream:Stream
    {
        private Sapphire _decoder;
        private Stream _stream;
        public SapphireStream(Stream inputStream, string cipherKey)
        {
            _stream = inputStream;
            _decoder = new Sapphire(Encoding.UTF8.GetBytes(cipherKey));
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var encryptedBuffer = new byte[count];
            var len = _stream.Read(encryptedBuffer, 0, (int)count);
            for (int i = 0; i < count; i++)
            {
                buffer[i + offset] = _decoder.cipher(encryptedBuffer[i]);
            }
            return len;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer,offset,count);
        }

        public override bool CanRead
        {
            get
            {
                return _stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return _stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return _stream.Length;
            }
        }

        public override long Position { get
            {
                return _stream.Position;
            }
            set
            {
                _stream.Position = value;
            }
        }
    }

    public class Sapphire
    {

        /**
         * Construct a Sapphire Stream Cipher from a key, possibly null or empty.
         */
        public Sapphire(byte[] aKey)
        {
            byte[] key = aKey;
            if (key == null)
            {
                key = new byte[0];
            }
            cards = new int[256];
            if (key.Count() > 0)
            {
                initialize(key);
            }
            else
            {
                hashInit();
            }
        }

        /// <summary>
        /// Decipher a single byte, presumably the next.
        /// </summary>
        /// <param name="b">the next byte to decipher</param>
        /// <returns></returns>
        public byte cipher(byte b)
        {
            // Picture a single enigma rotor with 256 positions, rewired
            // on the fly by card-shuffling.

            // This cipher is a variant of one invented and written
            // by Michael Paul Johnson in November, 1993.

            // Shuffle the deck a little more.

            // Convert from a byte to an int, but prevent sign extension.
            // So -16 becomes 240
            int bVal = b & 0xFF;
            ratchet += cards[rotor++];
            // Keep ratchet and rotor in the range of 0-255
            // The C++ code relied upon overflow of an unsigned char
            ratchet &= 0xFF;
            rotor &= 0xFF;
            int swaptemp = cards[lastCipher];
            cards[lastCipher] = cards[ratchet];
            cards[ratchet] = cards[lastPlain];
            cards[lastPlain] = cards[rotor];
            cards[rotor] = swaptemp;
            avalanche += cards[swaptemp];
            // Keep avalanche in the range of 0-255
            avalanche &= 0xFF;

            // Output one byte from the state in such a way as to make it
            // very hard to figure out which one you are looking at.
            lastPlain = bVal ^ cards[(cards[ratchet] + cards[rotor]) & 0xFF] ^ cards[cards[(cards[lastPlain] + cards[lastCipher] + cards[avalanche]) & 0xFF]];

            lastCipher = bVal;

            // Convert back to a byte
            // E.g. 240 becomes -16
            return (byte)lastPlain;
        }

        public void burn()
        {
            // Destroy the key and state information in RAM.
            for (int i = 0; i < 256; i++)
            {
                cards[i] = 0;
            }
            rotor = 0;
            ratchet = 0;
            avalanche = 0;
            lastPlain = 0;
            lastCipher = 0;
        }

        /**
         * @param hash
         */
        public void hashFinal(byte[] hash)
        { // Destination
            for (int i = 255; i >= 0; i--)
            {
                cipher((byte)i);
            }
            for (int i = 0; i < hash.Count(); i++)
            {
                hash[i] = cipher((byte)0);
            }
        }

        /**
         * Initializes the cards array to be deterministically random based upon the
         * key.
         * <p>
         * Key size may be up to 256 bytes. Pass phrases may be used directly, with
         * longer length compensating for the low entropy expected in such keys.
         * Alternatively, shorter keys hashed from a pass phrase or generated
         * randomly may be used. For random keys, lengths of from 4 to 16 bytes are
         * recommended, depending on how secure you want this to be.
         * </p>
         * 
         * @param key
         *            used to initialize the cipher engine.
         */
        private void initialize(byte[] key)
        {

            // Start with cards all in order, one of each.
            for (int i = 0; i < 256; i++)
            {
                cards[i] = i;
            }

            // Swap the card at each position with some other card.
            int swaptemp;
            int toswap = 0;
            keypos = 0; // Start with first byte of user key.
            rsum = 0;
            for (int i = 255; i >= 0; i--)
            {
                toswap = keyrand(i, key);
                swaptemp = cards[i];
                cards[i] = cards[toswap];
                cards[toswap] = swaptemp;
            }

            // Initialize the indices and data dependencies.
            // Indices are set to different values instead of all 0
            // to reduce what is known about the state of the cards
            // when the first byte is emitted.
            rotor = cards[1];
            ratchet = cards[3];
            avalanche = cards[5];
            lastPlain = cards[7];
            lastCipher = cards[rsum];

            // ensure that these have no useful values to those that snoop
            toswap = 0;
            swaptemp = toswap;
            rsum = swaptemp;
            keypos = rsum;
        }

        /**
         * Initialize non-keyed hash computation.
         */
        private void hashInit()
        {

            // Initialize the indices and data dependencies.
            rotor = 1;
            ratchet = 3;
            avalanche = 5;
            lastPlain = 7;
            lastCipher = 11;

            // Start with cards all in inverse order.

            int j = 255;
            for (int i = 0; i < 256; i++)
            {
                cards[i] = j--;
            }
        }

        private int keyrand(int limit, byte[] key)
        {
            int u; // Value from 0 to limit to return.

            if (limit == 0)
            {
                return 0; // Avoid divide by zero error.
            }

            int retry_limiter = 0; // No infinite loops allowed.

            // Fill mask with enough bits to cover the desired range.
            int mask = 1;
            while (mask < limit)
            {
                mask = (mask << 1) + 1;
            }

            do
            {
                // Convert a byte from the key to an int, but prevent sign
                // extension.
                // So -16 becomes 240
                // Also keep rsum in the range of 0-255
                // The C++ code relied upon overflow of an unsigned char
                rsum = (cards[rsum] + (key[keypos++] & 0xFF)) & 0xFF;

                if (keypos >= key.Count())
                {
                    keypos = 0; // Recycle the user key.
                    rsum += key.Count(); // key "aaaa" != key "aaaaaaaa"
                    rsum &= 0xFF;
                }

                u = mask & rsum;

                if (++retry_limiter > 11)
                {
                    u %= limit; // Prevent very rare long loops.
                }
            } while (u > limit);
            return u;
        }

        private int[] cards;
        private int rotor;
        private int ratchet;
        private int avalanche;
        private int lastPlain;
        private int lastCipher;
        private int keypos;
        private int rsum;
    }

}
