using MongoDB.Driver;

namespace LSTY.Sdtd.ServerAdmin.Data.Extensions
{
    public static class EnumExtension
    {
        public static SortDefinition<TEntity> ToSortDefinition<TEntity>(this Enum @enum, SortDefinitionBuilder<TEntity> build, bool desc)
        {
            return desc ? build.Descending(@enum.ToString()) : build.Ascending(@enum.ToString());
        }
    }
}
