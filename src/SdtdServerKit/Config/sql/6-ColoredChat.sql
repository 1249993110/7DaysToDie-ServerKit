--Colored chat
CREATE TABLE IF NOT EXISTS T_ColoredChat(
	Id TEXT NOT NULL PRIMARY KEY,					--Player Id
	CreatedAt TEXT NOT NULL,						--Created At
	NameColor TEXT,									--Name Color
	TextColor TEXT,									--Text Color
	Description TEXT								--Description
);
