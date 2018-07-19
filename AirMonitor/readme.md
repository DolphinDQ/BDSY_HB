# 项目说明 2018.7.9
## 项目简介
--------
1. 本项目使用MVVM框架搭建。（框架组件为：[Caliburn.Micro](https://caliburnmicro.com/documentation)）
2. ViewModel的NotifyPropertyChange接口使用了[Fody](https://github.com/Fody/PropertyChanged)
3. 数据接入为目前是用MQTT，接入框架为：[MQTTnet](https://github.com/chkr1011/MQTTnet)
4. 界面UI使用了框架：[MahApps.Metro](https://github.com/MahApps/MahApps.Metro)
5. 地图脚本开发使用了TypeScript语法，请见为MapJs项目。
6. 实时视频接入是直接调用第三方SDK，详情请见BVCU项目。

## 目录结构
--------
项目的目录说明如下:
>Config:主要是配置文件管理，以及配置文件的数据模型。
>Converter:数值转换器，用于界面数据优化。
>Core:主要是项目基础类，核心类的实现。
>Data:主要是项目数据采集管理，以及数据结构。
>Interface:主要是定义项目接口。
>Map:地图模块的实现。ts脚本的语法与框架,[语法](https://github.com/Microsoft/TypeScript/blob/master/doc/spec.md)、[ts框架](https://www.webpackjs.com/guides/typescript/)
>EventArgs:IEventAggregator回调的事件类型，可以通过IHandler<T>处理事件。可以参考[Caliburn.Micro](https://caliburnmicro.com/documentation)事件机制。
>ViewModels:视图模型。
>Views:视图
>libBVCU:视频组件。
>Camera:摄像头相关封装。
>Chart:图形报表封装。[文档](http://oxyplot.org/)

## Config
--------
1. 单例工厂模型实现IConfigManager。
2. ConfigManager实现了配置文件默认值生成，配置文件的调取与写入。
3. 配置文件写入成功后会发出EvtSetting.Command==SettingCommands.Changed事件。
4. 各个配置文件模型xxxSetting类。

## Core
--------
1. BackupManager是自动备份模块，实现的是将采样实时备份到ftp上，会导致项目对ftp的依赖程度变强，并且ftp负担变大，目前没有启用。
2. Logger程序日志输出模块，日志使用[system.diagnostics](https://msdn.microsoft.com/zh-cn/library/gg145030.aspx)日志追踪框架，
	可以用DebugView之类的软件查看软件日志，也可以在配置文件（app.config）配置，输出对应的日志。
3. ResourcesManager实现了xaml的资源调用，主要是文字资源和图片资源的整合。
4. Utils是常用工具的封装。

## Camera
--------
1. BVCUCameraManager单例模型，目前只封装了打开视频的相关业务流程。
2. 其他的是对象模型。

## Chart
--------
1. 单例工厂模型实现IChartManager。
2. ChartManager对接了 OxyPlot组件，以及其基本配置。

## Config
--------
1. 系统配置模块，主要管理系统想配置文件
2. 实现配置文件读写等功能。