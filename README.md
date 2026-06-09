# CNFairBanAlliance
使用方法：下载压缩包把LabAPI文件夹拖到%AppData%\SCP Secret Laboratory下
待配置文件生成后进入%AppData%\SCP Secret Laboratory\LabAPI\configs\7777\中国公平封禁联盟系统找到config.yml去配置服务器标识和秘钥
数据库: https://gitee.com/XLittleLeft/cfba-public-data/tree/master

说明：
1.插件里所含的数据库账户只有小范围读取权限和执行存储过程权限，什么也干不了，你一点数据都修改不了
2.后台数据库包含插入日志和删除日志用来记录哪个人插入了数据或删除了数据用来防止滥用，请放心

# 配置文件
# 服务器标识
server_name: 我发你的服务器名称
# 服务器秘钥
server_key: 我发你的秘钥
# 联Ban玩家尝试加入Log输出
log: true
# 检查数据更新时间（分钟）
check_interval: 5
# 检查玩家IP
check_player_i_p: false
