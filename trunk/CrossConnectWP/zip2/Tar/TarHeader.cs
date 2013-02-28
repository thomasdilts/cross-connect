#region Header

/* The original Java file had this header:
 *
** Authored by Timothy Gerard Endres
** <mailto:time@gjt.org>  <http://www.trustice.com>
**
** This work has been placed into the public domain.
** You may use this work in any way and for any purpose you wish.
**
** THIS SOFTWARE IS PROVIDED AS-IS WITHOUT WARRANTY OF ANY KIND,
** NOT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY. THE AUTHOR
** OF THIS SOFTWARE, ASSUMES _NO_ RESPONSIBILITY FOR ANY
** CONSEQUENCE RESULTING FROM THE USE, MODIFICATION, OR
** REDISTRIBUTION OF THIS SOFTWARE.
**
*/
// TarHeader.cs
//
// Copyright (C) 2001 Mike Krueger
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// Linking this library statically or dynamically with other modules is
// making a combined work based on this library.  Thus, the terms and
// conditions of the GNU General Public License cover the whole
// combination.
//
// As a special exception, the copyright holders of this library give you
// permission to link this library with independent modules to produce an
// executable, regardless of the license terms of these independent
// modules, and to copy and distribute the resulting executable under
// terms of your choice, provided that you also meet, for each linked
// independent module, the terms and conditions of the license of that
// module.  An independent module is a module which is not derived from
// or based on this library.  If you modify this library, you may extend
// this exception to your version of the library, but you are not
// obligated to do so.  If you do not wish to do so, delete this
// exception statement from your version.
/* The tar format and its POSIX successor PAX have a long history which makes for compatability
   issues when creating and reading files.

   This is further complicated by a large number of programs with variations on formats
   One common issue is the handling of names longer than 100 characters.
   GNU style long names are currently supported.

This is the ustar (Posix 1003.1) header.

struct header
{
    char t_name[100];          //   0 Filename
    char t_mode[8];            // 100 Permissions
    char t_uid[8];             // 108 Numerical User ID
    char t_gid[8];             // 116 Numerical Group ID
    char t_size[12];           // 124 Filesize
    char t_mtime[12];          // 136 st_mtime
    char t_chksum[8];          // 148 Checksum
    char t_typeflag;           // 156 Type of File
    char t_linkname[100];      // 157 Target of Links
    char t_magic[6];           // 257 "ustar" or other...
    char t_version[2];         // 263 Version fixed to 00
    char t_uname[32];          // 265 User Name
    char t_gname[32];          // 297 Group Name
    char t_devmajor[8];        // 329 Major for devices
    char t_devminor[8];        // 337 Minor for devices
    char t_prefix[155];        // 345 Prefix for t_name
    char t_mfill[12];          // 500 Filler up to 512
};

*/

#endregion Header

namespace ICSharpCode.SharpZipLib.Tar
{
    using System;
    using System.Text;

    /// <summary>
    /// This class encapsulates the Tar Entry Header used in Tar Archives.
    /// The class also holds a number of tar constants, used mostly in headers.
    /// </summary>
    public class TarHeader : ICloneable
    {
        #region Fields

        /// <summary>
        /// The length of the checksum field in a header buffer.
        /// </summary>
        public const int CHKSUMLEN = 8;

        /// <summary>
        /// Offset of checksum in a header buffer.
        /// </summary>
        public const int CHKSUMOFS = 148;

        /// <summary>
        /// The length of the devices field in a header buffer.
        /// </summary>
        public const int DEVLEN = 8;

        /// <summary>
        /// The length of the group id field in a header buffer.
        /// </summary>
        public const int GIDLEN = 8;

        /// <summary>
        /// The length of the group name field in a header buffer.
        /// </summary>
        public const int GNAMELEN = 32;

        /// <summary>
        /// The magic tag representing an old GNU tar archive where version is included in magic and overwrites it
        /// </summary>
        public const string GNU_TMAGIC = "ustar  ";

        // POSIX allows for upper case ascii type as extensions
        /// <summary>
        /// Solaris access control list file type
        /// </summary>
        public const byte LF_ACL = (byte) 'A';

        /// <summary>
        /// Block device file type.
        /// </summary>
        public const byte LF_BLK = (byte) '4';

        /// <summary>
        /// Character device file type.
        /// </summary>
        public const byte LF_CHR = (byte) '3';

        /// <summary>
        /// Contiguous file type.
        /// </summary>
        public const byte LF_CONTIG = (byte) '7';

        /// <summary>
        /// Directory file type.
        /// </summary>
        public const byte LF_DIR = (byte) '5';

        /// <summary>
        /// Solaris Extended Attribute File
        /// </summary>
        public const byte LF_EXTATTR = (byte) 'E' ;

        /// <summary>
        /// FIFO (pipe) file type.
        /// </summary>
        public const byte LF_FIFO = (byte) '6';

        /// <summary>
        /// Posix.1 2001 global extended header
        /// </summary>
        public const byte LF_GHDR = (byte) 'g';

        /// <summary>
        /// GNU dir dump file type
        /// This is a dir entry that contains the names of files that were in the
        /// dir at the time the dump was made
        /// </summary>
        public const byte LF_GNU_DUMPDIR = (byte) 'D';

        /// <summary>
        /// Identifies the next file on the tape as having a long link name
        /// </summary>
        public const byte LF_GNU_LONGLINK = (byte) 'K';

        /// <summary>
        /// Identifies the next file on the tape as having a long name
        /// </summary>
        public const byte LF_GNU_LONGNAME = (byte) 'L';

        /// <summary>
        /// Continuation of a file that began on another volume
        /// </summary>
        public const byte LF_GNU_MULTIVOL = (byte) 'M';

        /// <summary>
        /// For storing filenames that dont fit in the main header (old GNU)
        /// </summary>
        public const byte LF_GNU_NAMES = (byte) 'N';

        /// <summary>
        /// GNU Sparse file
        /// </summary>
        public const byte LF_GNU_SPARSE = (byte) 'S';

        /// <summary>
        /// GNU Tape/volume header ignore on extraction
        /// </summary>
        public const byte LF_GNU_VOLHDR = (byte) 'V';

        /// <summary>
        /// Link file type.
        /// </summary>
        public const byte LF_LINK = (byte) '1';

        /// <summary>
        /// Inode (metadata only) no file content
        /// </summary>
        public const byte LF_META = (byte) 'I';

        /// <summary>
        /// Normal file type.
        /// </summary>
        public const byte LF_NORMAL = (byte) '0';

        //
        // LF_ constants represent the "type" of an entry
        //
        /// <summary>
        ///  The "old way" of indicating a normal file.
        /// </summary>
        public const byte LF_OLDNORM = 0;

        /// <summary>
        /// Symbolic link file type.
        /// </summary>
        public const byte LF_SYMLINK = (byte) '2';

        /// <summary>
        /// Posix.1 2001 extended header
        /// </summary>
        public const byte LF_XHDR = (byte) 'x';

        /// <summary>
        /// The length of the magic field in a header buffer.
        /// </summary>
        public const int MAGICLEN = 6;

        /// <summary>
        /// The length of the mode field in a header buffer.
        /// </summary>
        public const int MODELEN = 8;

        /// <summary>
        /// The length of the modification time field in a header buffer.
        /// </summary>
        public const int MODTIMELEN = 12;

        /// <summary>
        /// The length of the name field in a header buffer.
        /// </summary>
        public const int NAMELEN = 100;

        /// <summary>
        /// The length of the size field in a header buffer.
        /// </summary>
        public const int SIZELEN = 12;

        /// <summary>
        /// The magic tag representing a POSIX tar archive.  (includes trailing NULL)
        /// </summary>
        public const string TMAGIC = "ustar ";

        /// <summary>
        /// The length of the user id field in a header buffer.
        /// </summary>
        public const int UIDLEN = 8;

        /// <summary>
        /// The length of the user name field in a header buffer.
        /// </summary>
        public const int UNAMELEN = 32;

        /// <summary>
        /// The length of the version field in a header buffer.
        /// </summary>
        public const int VERSIONLEN = 2;

        internal static int defaultGroupId;
        internal static string defaultGroupName = "None";
        internal static string defaultUser;
        internal static int defaultUserId;
        internal static int groupIdAsSet;
        internal static string groupNameAsSet = "None";

        // Values used during recursive operations.
        internal static int userIdAsSet;
        internal static string userNameAsSet;

        const long timeConversionFactor = 10000000L; // 1 tick == 100 nanoseconds

        static readonly DateTime dateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        int checksum;
        int devMajor;
        int devMinor;
        int groupId;
        string groupName;
        bool isChecksumValid;
        string linkName;
        string magic;
        int mode;
        DateTime modTime;
        string name;
        long size;
        byte typeFlag;
        int userId;
        string userName;
        string version;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise a default TarHeader instance
        /// </summary>
        public TarHeader()
        {
            Magic = TMAGIC;
            Version = " ";

            Name     = string.Empty;
            LinkName = string.Empty;

            UserId    = defaultUserId;
            GroupId   = defaultGroupId;
            UserName  = defaultUser;
            GroupName = defaultGroupName;
            Size      = 0;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get the entry's checksum.  This is only valid/updated after writing or reading an entry.
        /// </summary>
        public int Checksum
        {
            get { return checksum; }
        }

        /// <summary>
        /// Get/set the entry's major device number.
        /// </summary>
        public int DevMajor
        {
            get { return devMajor; }
            set { devMajor = value; }
        }

        /// <summary>
        /// Get/set the entry's minor device number.
        /// </summary>
        public int DevMinor
        {
            get { return devMinor; }
            set { devMinor = value; }
        }

        /// <summary>
        /// Get/set the entry's group id.
        /// </summary>
        /// <remarks>
        /// This is only directly relevant to linux/unix systems.
        /// The default value is zero.
        /// </remarks>
        public int GroupId
        {
            get { return groupId; }
            set { groupId = value; }
        }

        /// <summary>
        /// Get/set the entry's group name.
        /// </summary>
        /// <remarks>
        /// This is only directly relevant to unix systems.
        /// </remarks>
        public string GroupName
        {
            get { return groupName; }
            set {
                if ( value == null ) {
                    groupName = "None";
                }
                else {
                    groupName = value;
                }
            }
        }

        /// <summary>
        /// Get value of true if the header checksum is valid, false otherwise.
        /// </summary>
        public bool IsChecksumValid
        {
            get { return isChecksumValid; }
        }

        /// <summary>
        /// The entry's link name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when attempting to set LinkName to null.</exception>
        public string LinkName
        {
            get { return linkName; }
            set {
                if ( value == null ) {
                    throw new ArgumentNullException("value");
                }
                linkName = value;
            }
        }

        /// <summary>
        /// Get/set the entry's magic tag.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when attempting to set Magic to null.</exception>
        public string Magic
        {
            get { return magic; }
            set {
                if ( value == null ) {
                    throw new ArgumentNullException("value");
                }
                magic = value;
            }
        }

        /// <summary>
        /// Get/set the entry's Unix style permission mode.
        /// </summary>
        public int Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// Get/set the entry's modification time.
        /// </summary>
        /// <remarks>
        /// The modification time is only accurate to within a second.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting the date time to less than 1/1/1970.</exception>
        public DateTime ModTime
        {
            get { return modTime; }
            set {
                if ( value < dateTime1970 )
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                modTime = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        /// <summary>
        /// Get/set the name for this tar entry.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when attempting to set the property to null.</exception>
        public string Name
        {
            get { return name; }
            set {
                if ( value == null ) {
                    throw new ArgumentNullException("value");
                }
                name = value;
            }
        }

        /// <summary>
        /// Get/set the entry's size.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting the size to less than zero.</exception>
        public long Size
        {
            get { return size; }
            set {
                if ( value < 0 ) {
                    throw new ArgumentOutOfRangeException("value");
                }
                size = value;
            }
        }

        /// <summary>
        /// Get/set the entry's type flag.
        /// </summary>
        public byte TypeFlag
        {
            get { return typeFlag; }
            set { typeFlag = value; }
        }

        /// <summary>
        /// The entry's user id.
        /// </summary>
        /// <remarks>
        /// This is only directly relevant to unix systems.
        /// The default is zero.
        /// </remarks>
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        /// <summary>
        /// The entry's user name.
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set {
                if (value != null) {
                    userName = value.Substring(0, Math.Min(UNAMELEN, value.Length));
                }
                else {
                    string currentUser = "PocketPC";

                    if (currentUser.Length > UNAMELEN) {
                        currentUser = currentUser.Substring(0, UNAMELEN);
                    }
                    userName = currentUser;
                }
            }
        }

        /// <summary>
        /// The entry's version.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when attempting to set Version to null.</exception>
        public string Version
        {
            get {
                return version;
            }

            set {
                if ( value == null ) {
                    throw new ArgumentNullException("value");
                }
                version = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Add a string to a buffer as a collection of ascii bytes.
        /// </summary>
        /// <param name="toAdd">The string to add</param>
        /// <param name="nameOffset">The offset of the first character to add.</param>
        /// <param name="buffer">The buffer to add to.</param>
        /// <param name="bufferOffset">The offset to start adding at.</param>
        /// <param name="length">The number of ascii characters to add.</param>
        /// <returns>The next free index in the buffer.</returns>
        public static int GetAsciiBytes(string toAdd, int nameOffset, byte[] buffer, int bufferOffset, int length )
        {
            if ( toAdd == null ) {
                throw new ArgumentNullException("toAdd");
            }

            if ( buffer == null ) {
                throw new ArgumentNullException("buffer");
            }

            for (int i = 0 ; i < length && nameOffset + i < toAdd.Length; ++i)
            {
                buffer[bufferOffset + i] = (byte)toAdd[nameOffset + i];
            }
            return bufferOffset + length;
        }

        /// <summary>
        /// Put an octal representation of a value into a buffer
        /// </summary>
        /// <param name = "value">Value to be convert to octal</param>
        /// <param name = "buffer">The buffer to update</param>
        /// <param name = "offset">The offset into the buffer to store the value</param>
        /// <param name = "length">The length of the octal string</param>
        /// <returns>Index of next byte</returns>
        public static int GetLongOctalBytes(long value, byte[] buffer, int offset, int length)
        {
            return GetOctalBytes(value, buffer, offset, length);
        }

        /// <summary>
        /// Add <paramref name="name">name</paramref> to the buffer as a collection of bytes
        /// </summary>
        /// <param name="name">The name to add</param>
        /// <param name="nameOffset">The offset of the first character</param>
        /// <param name="buffer">The buffer to add to</param>
        /// <param name="bufferOffset">The index of the first byte to add</param>
        /// <param name="length">The number of characters/bytes to add</param>
        /// <returns>The next free index in the <paramref name="buffer"/></returns>
        public static int GetNameBytes(StringBuilder name, int nameOffset, byte[] buffer, int bufferOffset, int length)
        {
            if ( name == null ) {
                throw new ArgumentNullException("name");
            }

            if ( buffer == null ) {
                throw new ArgumentNullException("buffer");
            }

            return GetNameBytes(name.ToString(), nameOffset, buffer, bufferOffset, length);
        }

        /// <summary>
        /// Add <paramref name="name">name</paramref> to the buffer as a collection of bytes
        /// </summary>
        /// <param name="name">The name to add</param>
        /// <param name="nameOffset">The offset of the first character</param>
        /// <param name="buffer">The buffer to add to</param>
        /// <param name="bufferOffset">The index of the first byte to add</param>
        /// <param name="length">The number of characters/bytes to add</param>
        /// <returns>The next free index in the <paramref name="buffer"/></returns>
        public static int GetNameBytes(string name, int nameOffset, byte[] buffer, int bufferOffset, int length)
        {
            if ( name == null )
            {
                throw new ArgumentNullException("name");
            }

            if ( buffer == null )
            {
                throw new ArgumentNullException("buffer");
            }

            int i;

            for (i = 0 ; i < length - 1 && nameOffset + i < name.Length; ++i) {
                buffer[bufferOffset + i] = (byte)name[nameOffset + i];
            }

            for (; i < length ; ++i) {
                buffer[bufferOffset + i] = 0;
            }

            return bufferOffset + length;
        }

        /// <summary>
        /// Add an entry name to the buffer
        /// </summary>
        /// <param name="name">
        /// The name to add
        /// </param>
        /// <param name="buffer">
        /// The buffer to add to
        /// </param>
        /// <param name="offset">
        /// The offset into the buffer from which to start adding
        /// </param>
        /// <param name="length">
        /// The number of header bytes to add
        /// </param>
        /// <returns>
        /// The index of the next free byte in the buffer
        /// </returns>
        public static int GetNameBytes(StringBuilder name, byte[] buffer, int offset, int length)
        {
            if ( name == null ) {
                throw new ArgumentNullException("name");
            }

            if ( buffer == null ) {
                throw new ArgumentNullException("buffer");
            }

            return GetNameBytes(name.ToString(), 0, buffer, offset, length);
        }

        /// <summary>
        /// Add an entry name to the buffer
        /// </summary>
        /// <param name="name">The name to add</param>
        /// <param name="buffer">The buffer to add to</param>
        /// <param name="offset">The offset into the buffer from which to start adding</param>
        /// <param name="length">The number of header bytes to add</param>
        /// <returns>The index of the next free byte in the buffer</returns>
        public static int GetNameBytes(string name, byte[] buffer, int offset, int length)
        {
            if ( name == null ) {
                throw new ArgumentNullException("name");
            }

            if ( buffer == null )
            {
                throw new ArgumentNullException("buffer");
            }

            return GetNameBytes(name, 0, buffer, offset, length);
        }

        /// <summary>
        /// Put an octal representation of a value into a buffer
        /// </summary>
        /// <param name = "value">
        /// the value to be converted to octal
        /// </param>
        /// <param name = "buffer">
        /// buffer to store the octal string
        /// </param>
        /// <param name = "offset">
        /// The offset into the buffer where the value starts
        /// </param>
        /// <param name = "length">
        /// The length of the octal string to create
        /// </param>
        /// <returns>
        /// The offset of the character next byte after the octal string
        /// </returns>
        public static int GetOctalBytes(long value, byte[] buffer, int offset, int length)
        {
            if ( buffer == null ) {
                throw new ArgumentNullException("buffer");
            }

            int localIndex = length - 1;

            // Either a space or null is valid here.  We use NULL as per GNUTar
            buffer[offset + localIndex] = 0;
            --localIndex;

            if (value > 0) {
                for ( long v = value; (localIndex >= 0) && (v > 0); --localIndex ) {
                    buffer[offset + localIndex] = (byte)((byte)'0' + (byte)(v & 7));
                    v >>= 3;
                }
            }

            for ( ; localIndex >= 0; --localIndex ) {
                buffer[offset + localIndex] = (byte)'0';
            }

            return offset + length;
        }

        /// <summary>
        /// Parse a name from a header buffer.
        /// </summary>
        /// <param name="header">
        /// The header buffer from which to parse.
        /// </param>
        /// <param name="offset">
        /// The offset into the buffer from which to parse.
        /// </param>
        /// <param name="length">
        /// The number of header bytes to parse.
        /// </param>
        /// <returns>
        /// The name parsed.
        /// </returns>
        public static StringBuilder ParseName(byte[] header, int offset, int length)
        {
            if ( header == null ) {
                throw new ArgumentNullException("header");
            }

            if ( offset < 0 ) {

                throw new ArgumentOutOfRangeException("offset");

            }

            if ( length < 0 )
            {

                throw new ArgumentOutOfRangeException("length");
            }

            if ( offset + length > header.Length )
            {
                throw new ArgumentException("Exceeds header size", "length");
            }

            StringBuilder result = new StringBuilder(length);

            for (int i = offset; i < offset + length; ++i) {
                if (header[i] == 0) {
                    break;
                }
                result.Append((char)header[i]);
            }

            return result;
        }

        /// <summary>
        /// Parse an octal string from a header buffer.
        /// </summary>
        /// <param name = "header">The header buffer from which to parse.</param>
        /// <param name = "offset">The offset into the buffer from which to parse.</param>
        /// <param name = "length">The number of header bytes to parse.</param>
        /// <returns>The long equivalent of the octal string.</returns>
        public static long ParseOctal(byte[] header, int offset, int length)
        {
            if ( header == null ) {
                throw new ArgumentNullException("header");
            }

            long result = 0;
            bool stillPadding = true;

            int end = offset + length;
            for (int i = offset; i < end ; ++i) {
                if (header[i] == 0) {
                    break;
                }

                if (header[i] == (byte)' ' || header[i] == '0') {
                    if (stillPadding) {
                        continue;
                    }

                    if (header[i] == (byte)' ') {
                        break;
                    }
                }

                stillPadding = false;

                result = (result << 3) + (header[i] - '0');
            }

            return result;
        }

        /// <summary>
        /// Create a new <see cref="TarHeader"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="Object"/> that is a copy of the current instance.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Determines if this instance is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>true if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            TarHeader localHeader = obj as TarHeader;

            bool result;
            if ( localHeader != null )
            {
                result = (name == localHeader.name)
                    && (mode == localHeader.mode)
                    && (UserId == localHeader.UserId)
                    && (GroupId == localHeader.GroupId)
                    && (Size == localHeader.Size)
                    && (ModTime == localHeader.ModTime)
                    && (Checksum == localHeader.Checksum)
                    && (TypeFlag == localHeader.TypeFlag)
                    && (LinkName == localHeader.LinkName)
                    && (Magic == localHeader.Magic)
                    && (Version == localHeader.Version)
                    && (UserName == localHeader.UserName)
                    && (GroupName == localHeader.GroupName)
                    && (DevMajor == localHeader.DevMajor)
                    && (DevMinor == localHeader.DevMinor);
            }
            else
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Get a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// Get the name of this entry.
        /// </summary>
        /// <returns>The entry's name.</returns>
        [Obsolete("Use the Name property instead", true)]
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Parse TarHeader information from a header buffer.
        /// </summary>
        /// <param name = "header">
        /// The tar entry header buffer to get information from.
        /// </param>
        public void ParseBuffer(byte[] header)
        {
            if ( header == null )
            {
                throw new ArgumentNullException("header");
            }

            int offset = 0;

            name = TarHeader.ParseName(header, offset, TarHeader.NAMELEN).ToString();
            offset += TarHeader.NAMELEN;

            mode = (int)TarHeader.ParseOctal(header, offset, TarHeader.MODELEN);
            offset += TarHeader.MODELEN;

            UserId = (int)TarHeader.ParseOctal(header, offset, TarHeader.UIDLEN);
            offset += TarHeader.UIDLEN;

            GroupId = (int)TarHeader.ParseOctal(header, offset, TarHeader.GIDLEN);
            offset += TarHeader.GIDLEN;

            Size = TarHeader.ParseOctal(header, offset, TarHeader.SIZELEN);
            offset += TarHeader.SIZELEN;

            ModTime = GetDateTimeFromCTime(TarHeader.ParseOctal(header, offset, TarHeader.MODTIMELEN));
            offset += TarHeader.MODTIMELEN;

            checksum = (int)TarHeader.ParseOctal(header, offset, TarHeader.CHKSUMLEN);
            offset += TarHeader.CHKSUMLEN;

            TypeFlag = header[ offset++ ];

            LinkName = TarHeader.ParseName(header, offset, TarHeader.NAMELEN).ToString();
            offset += TarHeader.NAMELEN;

            Magic = TarHeader.ParseName(header, offset, TarHeader.MAGICLEN).ToString();
            offset += TarHeader.MAGICLEN;

            Version = TarHeader.ParseName(header, offset, TarHeader.VERSIONLEN).ToString();
            offset += TarHeader.VERSIONLEN;

            UserName = TarHeader.ParseName(header, offset, TarHeader.UNAMELEN).ToString();
            offset += TarHeader.UNAMELEN;

            GroupName = TarHeader.ParseName(header, offset, TarHeader.GNAMELEN).ToString();
            offset += TarHeader.GNAMELEN;

            DevMajor = (int)TarHeader.ParseOctal(header, offset, TarHeader.DEVLEN);
            offset += TarHeader.DEVLEN;

            DevMinor = (int)TarHeader.ParseOctal(header, offset, TarHeader.DEVLEN);

            // Fields past this point not currently parsed or used...

            isChecksumValid = Checksum == TarHeader.MakeCheckSum(header);
        }

        /// <summary>
        /// 'Write' header information to buffer provided, updating the <see cref="Checksum">check sum</see>.
        /// </summary>
        /// <param name="outBuffer">output buffer for header information</param>
        public void WriteHeader(byte[] outBuffer)
        {
            if ( outBuffer == null )
            {
                throw new ArgumentNullException("outBuffer");
            }

            int offset = 0;

            offset = GetNameBytes(Name, outBuffer, offset, NAMELEN);
            offset = GetOctalBytes(mode, outBuffer, offset, MODELEN);
            offset = GetOctalBytes(UserId, outBuffer, offset, UIDLEN);
            offset = GetOctalBytes(GroupId, outBuffer, offset, GIDLEN);

            offset = GetLongOctalBytes(Size, outBuffer, offset, SIZELEN);
            offset = GetLongOctalBytes(GetCTime(ModTime), outBuffer, offset, MODTIMELEN);

            int csOffset = offset;
            for (int c = 0; c < CHKSUMLEN; ++c)
            {
                outBuffer[offset++] = (byte)' ';
            }

            outBuffer[offset++] = TypeFlag;

            offset = GetNameBytes(LinkName, outBuffer, offset, NAMELEN);
            offset = GetAsciiBytes(Magic, 0, outBuffer, offset, MAGICLEN);
            offset = GetNameBytes(Version, outBuffer, offset, VERSIONLEN);
            offset = GetNameBytes(UserName, outBuffer, offset, UNAMELEN);
            offset = GetNameBytes(GroupName, outBuffer, offset, GNAMELEN);

            if ((TypeFlag == LF_CHR) || (TypeFlag == LF_BLK))
            {
                offset = GetOctalBytes(DevMajor, outBuffer, offset, DEVLEN);
                offset = GetOctalBytes(DevMinor, outBuffer, offset, DEVLEN);
            }

            for ( ; offset < outBuffer.Length; )
            {
                outBuffer[offset++] = 0;
            }

            checksum = ComputeCheckSum(outBuffer);

            GetCheckSumOctalBytes(checksum, outBuffer, csOffset, CHKSUMLEN);
            isChecksumValid = true;
        }

        internal static void RestoreSetValues()
        {
            defaultUserId = userIdAsSet;
            defaultUser = userNameAsSet;
            defaultGroupId = groupIdAsSet;
            defaultGroupName = groupNameAsSet;
        }

        /// <summary>
        /// Set defaults for values used when constructing a TarHeader instance.
        /// </summary>
        /// <param name="userId">Value to apply as a default for userId.</param>
        /// <param name="userName">Value to apply as a default for userName.</param>
        /// <param name="groupId">Value to apply as a default for groupId.</param>
        /// <param name="groupName">Value to apply as a default for groupName.</param>
        internal static void SetValueDefaults(int userId, string userName, int groupId, string groupName)
        {
            defaultUserId = userIdAsSet = userId;
            defaultUser = userNameAsSet = userName;
            defaultGroupId = groupIdAsSet = groupId;
            defaultGroupName = groupNameAsSet = groupName;
        }

        /// <summary>
        /// Compute the checksum for a tar entry header.  
        /// The checksum field must be all spaces prior to this happening
        /// </summary>
        /// <param name = "buffer">The tar entry's header buffer.</param>
        /// <returns>The computed checksum.</returns>
        static int ComputeCheckSum(byte[] buffer)
        {
            int sum = 0;
            for (int i = 0; i < buffer.Length; ++i) {
                sum += buffer[i];
            }
            return sum;
        }

        /// <summary>
        /// Add the checksum integer to header buffer.
        /// </summary>
        /// <param name = "value"></param>
        /// <param name = "buffer">The header buffer to set the checksum for</param>
        /// <param name = "offset">The offset into the buffer for the checksum</param>
        /// <param name = "length">The number of header bytes to update.
        /// It's formatted differently from the other fields: it has 6 digits, a
        /// null, then a space -- rather than digits, a space, then a null.
        /// The final space is already there, from checksumming
        /// </param>
        /// <returns>The modified buffer offset</returns>
        static int GetCheckSumOctalBytes(long value, byte[] buffer, int offset, int length)
        {
            TarHeader.GetOctalBytes(value, buffer, offset, length - 1);
            return offset + length;
        }

        static int GetCTime(System.DateTime dateTime)
        {
            return unchecked((int)((dateTime.Ticks - dateTime1970.Ticks) / timeConversionFactor));
        }

        static DateTime GetDateTimeFromCTime(long ticks)
        {
            DateTime result;

            try {
                result = new DateTime(dateTime1970.Ticks + ticks * timeConversionFactor);
            }
            catch(ArgumentOutOfRangeException) {
                result = dateTime1970;
            }
            return result;
        }

        /// <summary>
        /// Make a checksum for a tar entry ignoring the checksum contents.
        /// </summary>
        /// <param name = "buffer">The tar entry's header buffer.</param>
        /// <returns>The checksum for the buffer</returns>
        static int MakeCheckSum(byte[] buffer)
        {
            int sum = 0;
            for ( int i = 0; i < CHKSUMOFS; ++i )
            {
                sum += buffer[i];
            }

            for ( int i = 0; i < TarHeader.CHKSUMLEN; ++i)
            {
                sum += (byte)' ';
            }

            for (int i = CHKSUMOFS + CHKSUMLEN; i < buffer.Length; ++i)
            {
                sum += buffer[i];
            }
            return sum;
        }

        #endregion Methods
    }
}