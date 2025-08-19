namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    public class DiskInfo
    {
        /// <summary>
        /// Disk name.
        /// <para>ex:<br />
        /// Windows: C:\<br />
        /// Linux:  /dev
        /// </para>
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets the drive type.
        /// </summary>
        /// <remarks>Gets the drive type, such as CD-ROM, removable, network, or fixed.</remarks>
        public required DriveType DriveType { get; set; }

        /// <summary>
        /// Drive Format (File system).
        /// <para>
        /// Windows:   NTFS, CDFS...<br />
        /// Linux:  rootfs, tmpfs, binfmt_misc...
        /// </para>
        /// </summary>
        public required string DriveFormat { get; set; }

        /// <summary>
        /// Available free space on the drive, in bytes.
        /// </summary>
        public required long FreeSpace { get; set; }

        /// <summary>
        /// Total size of storage space on the drive, in bytes.
        /// </summary>
        public required long TotalSize { get; set; }

        /// <summary>
        /// Used space on the drive, in bytes.
        /// </summary>
        public long UsedSize => TotalSize - FreeSpace;

        /// <summary>
        /// The root directory of the drive.
        /// </summary>
        public required string? RootPath { get; set; }
    }

}
