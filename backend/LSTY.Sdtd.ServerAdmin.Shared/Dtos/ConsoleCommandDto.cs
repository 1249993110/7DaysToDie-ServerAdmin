namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    public class ConsoleCommandDto
    {
        /// <summary>
        /// Command text
        /// </summary>
        [Required]
        public required string Command { get; set; }

        /// <summary>
        /// Execute console command in main thread
        /// </summary>
        [DefaultValue(true)]
        public bool InMainThread { get; set; } = true;
    }
}
