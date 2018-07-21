# PokemonGame_Entitas游戏介绍

## 游戏说明

说是游戏，实际上这个游戏并不完整，受限个人程序设计能力和工作量的问题，目前只实现了草地系统和不完整的精灵对战系统。

### 游戏玩法

玩家可以操控训练家进入草地，草地中会冒出精灵与玩家作战，玩家可以在战斗中选择使用道具，捕捉精灵，切换精灵，选择精灵技能进行攻击，或是离开战斗。这个游戏能让玩家充分享受精灵对战和收集的乐趣。

## 使用技术

该游戏基于Unity游戏引擎，主要使用Entitias-CSharp作为游戏实现的主要框架，基于TinyTeam.UI和UGUI完成游戏的UI部分

## 程序设计结构

个人编写的脚本主要在四个文件夹下，分别是Editor,Scripts,Sources,Shader

Editor下脚本是游戏开发过程我个人编写的编辑器扩展，主要用于读取xml里的精灵相关数据，编辑配置技能，图集拆分等等



Shader下脚本主要是草地编辑器和用于草地渲染的shader



Scripts下有四个文件夹，Common,Controllers,kbe-scripts,View

Common是一些我自定义的工具类，如自定义的事件系统，单例类，碰撞检测系统,对象池等等

kbe-scripts是原本设计用于与KBEngine服务器通信的脚本，不过受限工作量的问题，被我暂时延后了

View下是基于TinyTeam.UI和UGUI完成的UI，用于UI的显示和对外界输入的响应

Controllers下是一些挂上了Monobehavior的脚本，用于运行Entitas的GameSystem和TinyTeam.UI，以及对摄像头，玩家，触摸输入，资源加载，对象池等等的控制



Sources下是基于Entitas框架的脚本，分Data，Components，Systems，Services，Generated

Components是基于Entitas框架编写的组件，Systems是系统，Generated是Entitas根据Components自动生成的代码，Services是一些辅助的静态类，Data是为游戏专门定义的数据结构,游戏的主要逻辑在Systems里完成，目前主要实现了草地系统，对战系统，玩家数据的自动存储

