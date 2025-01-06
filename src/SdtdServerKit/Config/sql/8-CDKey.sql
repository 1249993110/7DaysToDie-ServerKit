PRAGMA FOREIGN_KEYS = ON;

--CDKey
CREATE TABLE IF NOT EXISTS T_CDKey(
	Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,	--Id
	CreatedAt TEXT NOT NULL,						--Created At
	CDKey TEXT NOT NULL UNIQUE,						--CDKey
	RedeemCount INTEGER NOT NULL,					--Redeem Count
	MaxRedeemCount INTEGER NOT NULL,				--Max Redeem Count
	ExpiryAt TEXT NULL,								--Expiry At
	Description TEXT NULL							--Description
);
CREATE UNIQUE INDEX IF NOT EXISTS Index_CDKey_0 ON T_CDKey(CDKey);

--CDKey Redeem Record
CREATE TABLE IF NOT EXISTS T_CDKeyRedeemRecord(
	Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,	--Id
	CreatedAt TEXT NOT NULL,						--Created At
	CDKey TEXT NOT NULL,							--CDKey
	PlayerId TEXT NOT NULL,							--Player Id
	PlayerName TEXT NOT NULL,						--Player Name
	FOREIGN KEY (CDKey) REFERENCES T_CDKey(CDKey) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IF NOT EXISTS Index_CDKeyRedeemRecord_0 ON T_CDKeyRedeemRecord(CDKey, PlayerId);

CREATE TABLE IF NOT EXISTS T_CDKeyItem(
	CDKeyId TEXT NOT NULL,						
	ItemId INTEGER NOT NULL,						
	PRIMARY KEY (CDKeyId, ItemId),
	FOREIGN KEY (CDKeyId) REFERENCES T_CDKey(Id) ON DELETE CASCADE,
	FOREIGN KEY (ItemId) REFERENCES T_ItemList(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS T_CDKeyCommand(
	CDKeyId TEXT NOT NULL,						
	CommandId INTEGER NOT NULL,						
	PRIMARY KEY (CDKeyId, CommandId),
	FOREIGN KEY (CDKeyId) REFERENCES T_CDKey(Id) ON DELETE CASCADE,
	FOREIGN KEY (CommandId) REFERENCES T_CommandList(Id) ON DELETE CASCADE
);