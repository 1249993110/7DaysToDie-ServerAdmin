namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary> 
    /// Inventory
    /// </summary> 
    public class InventoryDto
    {
        /// <summary> 
        /// Backpack 
        /// </summary> 
        public required List<InvItemDto> Bag { get; set; }

        /// <summary> 
        /// Belt 
        /// </summary> 
        public required List<InvItemDto> Belt { get; set; }

        /// <summary> 
        /// Equipment 
        /// </summary> 
        public required InvItemDto?[] Equipment { get; set; }
    }
}