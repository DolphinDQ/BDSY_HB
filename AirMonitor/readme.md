##项目说明
===
###项目简介
--------
1. 本项目使用MVVM框架搭建。（框架组件为：[Caliburn.Micro](https://caliburnmicro.com/documentation)）
2. ViewModel的NotifyPropertyChange接口使用了[Fody](https://github.com/Fody/PropertyChanged)
3. 数据接入为目前是用MQTT，接入框架为：[MQTTnet](https://github.com/chkr1011/MQTTnet)
4. 界面UI使用了框架：[MahApps.Metro](https://github.com/MahApps/MahApps.Metro)

###目录结构
--------
项目的目录说明如下:
>Config:主要是配置文件管理，以及配置文件的数据模型。
>Converter:数值转换器，用于界面数据优化。
>Core:主要是项目基础类，核心类的实现。
>Data:主要是项目数据采集管理，以及数据结构。
>Interface:主要是定义项目接口。
>Map:地图模块的实现。
>EventArgs:IEventAggregator回调的事件类型，可以通过IHandler<T>处理事件。
>ViewModels:视图模型。
>Views:视图

