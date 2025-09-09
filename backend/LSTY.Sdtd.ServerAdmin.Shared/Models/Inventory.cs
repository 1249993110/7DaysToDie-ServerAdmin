namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary> 
    /// Inventory
    /// </summary> 
    public class Inventory
    {
        /// <summary> 
        /// Backpack 
        /// </summary> 
        public required List<InvItem> Bag { get; set; }

        /// <summary> 
        /// Belt 
        /// </summary> 
        public required List<InvItem> Belt { get; set; }

        /// <summary> 
        /// Equipment 
        /// </summary> 
        public required InvItem?[] Equipment { get; set; }
    }
}