--积分信息
CREATE TABLE IF NOT EXISTS T_PointsInfo(
	Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,	--Id
	CreatedAt TEXT NOT NULL,						--创建日期
	PlayerId TEXT NOT NULL,							--玩家跨平台Id
	PlayerName TEXT NOT NULL,						--玩家名称
	Points INTEGER NOT NULL,						--拥有积分
	LastSignInDays INTEGER NOT NULL					--上次签到天数
);

--创建索引
CREATE UNIQUE INDEX IF NOT EXISTS Index_PointsInfo_0 ON T_PointsInfo(PlayerId);
CREATE INDEX IF NOT EXISTS Index_PointsInfo_1 ON T_PointsInfo(PlayerName);
