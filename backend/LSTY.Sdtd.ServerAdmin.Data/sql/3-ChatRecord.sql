CREATE TABLE IF NOT EXISTS ChatMessage(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[CreatedAt] TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
	[GameServerId] TEXT NOT NULL,
	[EntityId] INTEGER NOT NULL,
	[PlayerId] TEXT NULL,
	[ChatType] TEXT NOT NULL,
	[SenderName] TEXT NOT NULL,
	[Message] TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS Index_ChatMessage_0 ON ChatMessage([GameServerId], [CreatedAt] DESC, [EntityId]);
CREATE INDEX IF NOT EXISTS Index_ChatMessage_1 ON ChatMessage([GameServerId], [CreatedAt] DESC, [PlayerId]);
CREATE INDEX IF NOT EXISTS Index_ChatMessage_2 ON ChatMessage([GameServerId], [CreatedAt] DESC, [ChatType]);
CREATE INDEX IF NOT EXISTS Index_ChatMessage_3 ON ChatMessage([GameServerId], [CreatedAt] DESC, [SenderName]);

