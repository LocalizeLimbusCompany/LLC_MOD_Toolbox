; ============================================
; exportInstaller.iss  (Inno Setup 6.x)
; ============================================
; 用法（示例）：
; iscc installer\exportInstaller.iss ^
;   /DInputDir="dist" ^
;   /DOutputDir="." ^
;   /DMyAppVersion="1.4.0"

; -------------------------
; 元信息与可传参开关
; -------------------------
#define MyAppName "LLC_MOD_Toolbox"
#define MyAppExeName "LLC_MOD_Toolbox.exe"
#define MyAppPublisher "LocalizeLimbusCompany"
#define MyAppURL "https://www.zeroasso.top/"

#ifndef MyAppVersion
  #define MyAppVersion "0.0.0-ci"
#endif

#ifndef InputDir
  #define InputDir "dist"
#endif

#ifndef OutputDir
  #define OutputDir "."
#endif

; -------------------------
; 安装器基础配置
; -------------------------
[Setup]
; 固定 AppId（请勿更改，以便升级/卸载识别为同一产品）
AppId={{3D5B3A12-2CDA-4E16-9F3D-3B9E4C8C2D17}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

; 安装目录（自动选择 64 位 Program Files 或 32 位）
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
AllowNoIcons=yes

; 输出设置
OutputDir={#OutputDir}
OutputBaseFilename=LLC_MOD_Toolbox_Installer

; 压缩优化
Compression=lzma2/ultra
SolidCompression=yes

; 64 位系统上以 64 位模式安装（若是 x86 程序也可照常运行）
ArchitecturesAllowed=x64 x86
ArchitecturesInstallIn64BitMode=x64

; 需要管理员权限安装到 Program Files
PrivilegesRequired=admin

; 安装/卸载显示图标：若 dist 下有图标则使用
#ifexist "{#InputDir}\favicon.ico"
SetupIconFile={#InputDir}\favicon.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
#else
UninstallDisplayIcon={app}\{#MyAppExeName}
#endif

; 可选：把版本信息写入安装器文件属性
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoProductName={#MyAppName}

; -------------------------
; 语言
; -------------------------
[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "chinesesimplified"; MessagesFile: "{#SourcePath}\Languages\ChineseSimplified.isl"

; -------------------------
; 自定义任务（桌面图标）
; -------------------------
[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

; -------------------------
; 要打进安装包的文件
; -------------------------
[Files]
; 先放主程序 exe
Source: "{#InputDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

; 再把 InputDir 其余内容全部打入（排除主 exe，避免重复）
Source: "{#InputDir}\*"; Excludes: "{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; -------------------------
; 开始菜单/桌面快捷方式
; -------------------------
[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

; -------------------------
; 安装完可选运行
; -------------------------
[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
