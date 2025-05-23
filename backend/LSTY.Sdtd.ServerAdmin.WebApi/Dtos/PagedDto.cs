﻿namespace LSTY.Sdtd.ServerAdmin.WebApi.Dtos
{
    /// <summary>
    /// Paged DTO
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PagedDto<TEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        public required long Total { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required IEnumerable<TEntity> Items { get; set; }
    }
}
