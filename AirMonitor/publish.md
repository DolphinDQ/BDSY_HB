#发布框架
1. 软件使用ClickOnce发布。
2. 发布账号是ftpadmin。
3. 发布只能在内网发布。

#发布配置
1. 发布前请修改Properties/AssemblyInfo.cs 中的版本号。（AppVersion.VERSION）
2. 修改完版本好重新生成项目，vs会提示重新加载项目，加载完成后可以开始发布
3. 项目右键->发布->完成，输入发布账号密码即可发布。


#注意事项
1. 发布期间保持当前PC与FTP服务器的网络畅通。
2. 发布使用到的xxxxx_TemporaryKey.pfx为临时密钥，有效期一年（到期：2019.7.6）,到期后可以在属性->签名更新密钥。
3. 如果添加nuget库的引用，记得在属性->发布->应用程序文件，将引用的库文件选择为包括。（不然用户会安装失败）


#FTP配置
 1. 目前FTP搭建在临时服务器上
 2. AirMonitor和AirDetector登录的用户既是FTP管理用户。
 3. 如果需要添加别的客户端共享文件夹，即在ftp服务器中建立一个指定客户专用的文件夹，添加该文件夹的访问用户即可。
 > 基础步骤：
 >> 使用airadmin登录[ftp](ftp://192.168.1.180);
 >> 新建新客户文件夹（祖庙街道项目）如：zumiao;
 >> 用ssh登录ftp服务器，添加用户：user1 密码：123456，操作命令为 ：
 >> ``useradd user1 -d /home/ftp/air/zumiao/ -s /sbin/nologin ``
 >> ``passwd user1 123456``