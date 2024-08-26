# CNFairBanAlliance
使用方法：下载压缩包把所有东西拖到%AppData%\SCP Secret Laboratory\PluginAPI\plugins\global下
待配置文件生成后进入%AppData%\SCP Secret Laboratory\PluginAPI\plugins\7777\中国公平封禁联盟系统找到config.yml更改本地列表缓存路径到你想要的地方
官网：https://www.cnfba.top/

说明：
1.插件里所含的数据库账户只有小范围读取权限和执行存储过程权限，什么也干不了，你一点数据都修改不了
2.后台数据库包含插入日志和删除日志用来记录哪个人插入了数据或删除了数据用来防止滥用，请放心

# 配置文件
# 联Ban玩家尝试加入Log输出
log: true
# 本地列表缓存路径
path: C:\Users\Administrator\Desktop\
# 检查数据更新时间（分钟）
check_interval: 5
# 检查玩家IP
check_player_i_p: false
