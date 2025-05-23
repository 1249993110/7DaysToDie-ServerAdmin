﻿using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Data.Enums;

namespace LSTY.Sdtd.ServerAdmin.Data.Entities
{
    public class LogEntry : EntityBase
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public required LogLevel LogLevel { get; set; }

        public required string Content { get; set; }

        public string? AdditionalData { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public required ServiceModule ServiceModule { get; set; }

        public required string GameServerId { get; set; }

        static LogEntry()
        {
            DB.Index<LogEntry>()
                .Key(e => e.GameServerId, KeyType.Ascending)
                .Key(e => e.CreatedOn, KeyType.Descending)
                .Key(e => e.ServiceModule, KeyType.Ascending)
                .Key(e => e.LogLevel, KeyType.Ascending)
                .CreateAsync();
        }
    }
}
