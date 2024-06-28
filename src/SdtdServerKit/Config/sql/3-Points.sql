--积分信息
CREATE TABLE IF NOT EXISTS T_PointsInfo(
	Id TEXT NOT NULL PRIMARY KEY,					--玩家跨平台Id
	CreatedAt TEXT NOT NULL,						--创建日期
	PlayerName TEXT NOT NULL,						--玩家名称
	Points INTEGER NOT NULL,						--拥有积分
	LastSignInDays INTEGER NOT NULL					--上次签到天数
);

--创建索引
CREATE INDEX IF NOT EXISTS Index_PointsInfo_0 ON T_PointsInfo(CreatedAt);
CREATE INDEX IF NOT EXISTS Index_PointsInfo_2 ON T_PointsInfo(PlayerName);
CREATE INDEX IF NOT EXISTS Index_PointsInfo_3 ON T_PointsInfo(Points);
