#项目说明
>1.本项目是地图显示模块的封装。
>2.封装主要使用了typescript语言，封装了百度地图webapi
>3.如果项目有更多需求，baidu地图无法提供足够功能，可以考虑高德地图（amap）。
>4.在index.ts中可以切换对应的封装。
>5.生成依赖npm管理器，生成时候请确定生成PC已经安装了npm。


#代码说明
>1.代码分为文件夹：dist和src以及相关配置文件。
>2.配置文件说明可以在[webpack说明](https://www.webpackjs.com/guides/typescript/)找到
>3.项目**重新生成**后，生成文件在dist文件中。（dist文件夹中的文件分别映射到AirDetector和AirMonitor项目中输出，所以修改后ts脚本后只需要**重新生成**项目即可更新到引用项目）
>4.src是主要的ts代码文件，剩下的就是看代码实现了。
