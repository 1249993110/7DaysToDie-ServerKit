--商品
CREATE TABLE IF NOT EXISTS T_Goods_v1(
	Id INTEGER NOT NULL PRIMARY KEY,				--Id
	CreatedAt TEXT NOT NULL,						--创建日期
	Name TEXT NOT NULL,								--商品名称
	Content TEXT NOT NULL,							--内容
	ContentType INTEGER NOT NULL,					--内容类型, 0: 物品 1: 自定义命令
	InMainThread INTEGER NOT NULL,					--在主线程执行
	Price INTEGER NOT NULL							--售价
);

-- content exp:
-- ["ItemName": "", "Count": "", "Quality": "", "Durability": "", "ItemIcon": "", "IconColor": ""]
-- ["", ""]