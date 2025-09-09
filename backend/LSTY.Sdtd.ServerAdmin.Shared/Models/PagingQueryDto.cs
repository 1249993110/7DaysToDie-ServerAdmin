namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Represents a pagination query with basic parameters.
    /// </summary>
    public class PagingQueryDto
    {
        /// <summary>
        /// The page number. Default value is 1.
        /// </summary>
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// The number of items per page. 
        /// If the value is less than 0, all records will be returned. Default value is 10.
        /// </summary>
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// The search keyword for filtering results.
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// The field by which the results should be ordered.
        /// </summary>
        public string? Order { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the results should be sorted in descending order.
        /// </summary>
        public bool Desc { get; set; }
    }

    /// <summary>
    /// Represents a pagination query with a generic type for the order field.
    /// </summary>
    /// <typeparam name="TOrder">The type of the order field, which must be an enumeration.</typeparam>
    public class PagingQueryDto<TOrder> where TOrder : struct, Enum
    {
        /// <summary>
        /// The page number. Default value is 1.
        /// </summary>
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// The number of items per page. 
        /// If the value is less than 0, all records will be returned. Default value is 10.
        /// </summary>
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// The search keyword for filtering results.
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// The field by which the results should be ordered.
        /// </summary>
        public TOrder? Order { get; set; }

        /// <summary>
        /// A value indicating whether the results should be sorted in descending order.
        /// </summary>
        public bool Desc { get; set; }
    }
}
