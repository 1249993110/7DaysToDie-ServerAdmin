CREATE TABLE IF NOT EXISTS LogEntry(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[CreatedAt] TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
	[GameServerId] TEXT NOT NULL,
	[ServiceModule] INTEGER NOT NULL,
	[LogLevel] INTEGER NOT NULL,
	[Message] TEXT NOT NULL,
	[Exception] TEXT NULL,
	[CorrelationId] TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS Index_LogEntry_0 ON LogEntry([GameServerId], [CreatedAt] DESC, [ServiceModule], [LogLevel]);
CREATE UNIQUE INDEX IF NOT EXISTS Index_LogEntry_1 ON LogEntry([CorrelationId]);

