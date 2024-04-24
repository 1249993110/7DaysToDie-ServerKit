--商品
CREATE TABLE IF NOT EXISTS T_Goods(
	Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,	--Id
	Name TEXT NOT NULL,								--商品名称
	ExecuteCommands TEXT NOT NULL,					--执行指令
	InMainThread INTEGER NOT NULL,					--在主线程执行指令
	Price INTEGER NOT NULL							--售价
);
--创建索引
CREATE UNIQUE INDEX IF NOT EXISTS Index_Goods_0 ON T_Goods(Name);