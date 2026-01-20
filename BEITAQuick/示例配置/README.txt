配置文件格式说明
==================

1. 根目录 config.ini
-------------------
文件位置：与 BEITAQuick.exe 同级目录
文件内容：应用程序名称（单行文本）

示例：
我的应用程序

或：
MyApplication 2.0


2. CONTENT_N/config.ini
-----------------------
文件位置：每个 CONTENT_* 文件夹内
文件内容：目标安装路径（单行文本）

示例：
C:\Program Files\MyApp

或：
C:\Users\Public\Documents\MyApp

或：
D:\Software\MyApp


3. LICENSE.txt（可选）
----------------------
文件位置：与 BEITAQuick.exe 同级目录
文件内容：许可协议文本（多行文本）

如果此文件存在，安装器会在安装前显示许可协议页面。


完整目录结构示例
================

App/
├── BEITAQuick.exe
├── config.ini                    ← 内容：我的应用程序
├── LICENSE.txt                   ← 可选的许可协议
├── CONTENT_1/
│   ├── config.ini                ← 内容：C:\Program Files\MyApp
│   └── CONTENT/
│       ├── FolderA/              ← 将被复制到 C:\Program Files\MyApp\FolderA
│       │   └── file1.txt
│       └── FolderB/              ← 将被复制到 C:\Program Files\MyApp\FolderB
│           └── file2.txt
└── CONTENT_2/
    ├── config.ini                ← 内容：C:\ProgramData\MyApp
    └── CONTENT/
        └── Data/                 ← 将被复制到 C:\ProgramData\MyApp\Data
            └── data.db


重要提示
========

1. 所有 config.ini 文件都是纯文本文件，只包含一行内容
2. 不要在路径末尾添加反斜杠 \
3. 支持中文路径和中文应用程序名称
4. 文件编码建议使用 UTF-8 或 GBK
5. CONTENT_* 文件夹的编号可以是任意数字（CONTENT_1, CONTENT_2, CONTENT_99 等）
