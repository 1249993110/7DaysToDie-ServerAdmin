using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Data.Enums;

namespace LSTY.Sdtd.ServerAdmin.Data.Entities
{
    public class LogEntry : EntityBase
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public required LogLevel Level { get; set; }

        public required string Content { get; set; }

        public string? AdditionalData { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public required ServiceModule ServiceModule { get; set; }

        public required string GameServerId { get; set; }

        static LogEntry()
        {
            DB.Index<LogEntry>()
                .Key(e => e.CreatedOn, KeyType.Descending)
                .Key(e => e.Level, KeyType.Ascending)
                .Key(e => e.Content, KeyType.Text)
                .Key(e => e.ServiceModule, KeyType.Ascending)
                .Key(e => e.GameServerId, KeyType.Ascending)
                .CreateAsync();
        }
    }
}
