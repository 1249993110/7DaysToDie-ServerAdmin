

namespace LSTY.Sdtd.ServerAdmin.Extensions
{
    using LSTY.Sdtd.ServerAdmin.Shared.Constants;
    using InventoryDto = Shared.Dtos.InventoryDto;

    /// <summary>
    /// Extension methods for PlayerDataFile.
    /// </summary>
    internal static class PlayerDataFileExtension
    {
        /// <summary>
        /// Gets the total stack count of a specified item in the player's inventory.
        /// </summary>
        /// <param name="pdf">The PlayerDataFile instance.</param>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="language"></param>
        /// <returns>The total stack count of the item.</returns>
        public static int GetInventoryStackCount(this PlayerDataFile pdf, string itemName, Language language)
        {
            int count = 0;
            var bag = ProcessInv(pdf.bag, language);
            var belt = ProcessInv(pdf.inventory, language);

            foreach (var item in bag)
            {
                if (item.ItemName == itemName)
                {
                    count += item.Count;
                }
            }

            foreach (var item in belt)
            {
                if (item.ItemName == itemName)
                {
                    count += item.Count;
                }
            }

            return count;
        }

        /// <summary>
        /// Gets the player's inventory.
        /// </summary>
        /// <param name="pdf">The PlayerDataFile instance.</param>
        /// <param name="language"></param>
        /// <returns>The player's inventory.</returns>
        public static InventoryDto GetInventory(this PlayerDataFile pdf, Language language)
        {
            try
            {
                return new InventoryDto()
                {
                    Bag = ProcessInv(pdf.bag, language),
                    Belt = ProcessInv(pdf.inventory, language),
                    Equipment = ProcessEqu(pdf.equipment, language)
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Get player inventory from PlayerDataFile failed.", ex);
            }
        }

        private static List<InvItemDto> ProcessInv(ItemStack[] sourceFields, Language language)
        {
            var target = new List<InvItemDto>(sourceFields.Length);

            foreach (var field in sourceFields)
            {
                var invItem = CreateInvItem(field.itemValue, field.count, language);
                if (invItem != null)
                {
                    ProcessParts(field.itemValue.Modifications, invItem, language);
                    target.Add(invItem);
                }
            }

            return target;
        }

        private static InvItemDto?[] ProcessEqu(Equipment sourceEquipment, Language language)
        {
            int slotCount = sourceEquipment.GetSlotCount();
            var equipment = new InvItemDto?[slotCount];
            for (int i = 0; i < slotCount; i++)
            {
                var itemValue = sourceEquipment.GetSlotItem(i);
                var invItem = CreateInvItem(itemValue, 1, language);
                if (invItem != null)
                {
                    ProcessParts(itemValue.Modifications, invItem, language);
                }
                equipment[i] = invItem;
            }

            return equipment;
        }

        private static void ProcessParts(ItemValue[] parts, InvItemDto item, Language language)
        {
            int length = parts.Length;
            InvItemDto?[] itemParts = new InvItemDto[length];

            for (int i = 0; i < length; i++)
            {
                var partItem = CreateInvItem(parts[i], 1, language);
                if (partItem != null)
                {
                    ProcessParts(parts[i].Modifications, partItem, language);
                }

                itemParts[i] = partItem;
            }

            item.Parts = itemParts;
        }

        private static InvItemDto? CreateInvItem(ItemValue? itemValue, int count, Language language)
        {
            try
            {
                if (itemValue == null || count <= 0 || itemValue.Equals(ItemValue.None))
                {
                    return null;
                }

                var itemClass = ItemClass.list[itemValue.type];

                if (itemClass == null)
                {
                    return null;
                }

                string name = itemClass.GetItemName();

                int? quality = null;
                string? qualityColor = null;
                if (itemValue.HasQuality)
                {
                    quality = itemValue.Quality;
                    qualityColor = QualityInfo.GetQualityColorHex(itemValue.Quality);
                }

                UnityEngine.Color iconTint = itemClass.GetIconTint();
                InvItemDto item = new InvItemDto()
                {
                    ItemName = name,
                    IconName = itemClass.GetIconName(),
                    IconColor = iconTint == UnityEngine.Color.white ? null : iconTint.ToHex(),
                    LocalizationName = Utils.GetLocalization(name, language),
                    Count = count,
                    MaxStackAllowed = itemClass.Stacknumber.Value,
                    Quality = quality,
                    QualityColor = qualityColor,
                    UseTimes = itemValue.UseTimes,
                    MaxUseTimes = itemValue.MaxUseTimes,
                    IsMod = itemValue.IsMod,
                    IsBlock = itemClass.IsBlock(),
                    Parts = null
                };

                return item;
            }
            catch (Exception ex)
            {
                CustomLogger.Warn(ex, "Error in PlayerDataFileExtension.CreateInvItem");
                return null;
            }
        }
    }
}