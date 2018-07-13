#项目说明 2018.7.12
1. 本项目是用于采集国控站点标准数据而开发。
2. 采集的数据将会存储在服务器本地。
3. 采集数据评率为每个小时2-3次。
4. 采集方式，获取LearnCloud已有的国控站点缓存数据。
5. 共享数据方式：WebApi
6. 数据格式为JSON，具体字段请见项目AirStandard.Model

#项目部署
1. 项目部署在树莓派服务器上。
2. 目前dotnet core 没有sdk可以安装在arm32主机上，需要使用特殊的[打包方式](https://github.com/dotnet/core/blob/master/samples/RaspberryPiInstructions.md)
3. 树莓派已经部署了ftp服务器，发布项目直接以FTP方式发布到服务器即可，发布流程请见[publish.ps1](/publish.ps1)
4. 新版发布方式，项目右键->属性->打包，修改项目版本号，项目右键**生成**即可直接将项目发布到树莓派服务器。**注意，项目发布后树莓派服务器可能需要重启服务**
5. 发布选项修改可以直接修改./Properties/PublishProfiles/文件夹中的文件，可以通过项目右键->发布->选择配置文件->配置。**目前树莓派服务器只支持ftp发布**