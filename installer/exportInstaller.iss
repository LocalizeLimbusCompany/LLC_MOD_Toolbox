; ============================================
; exportInstaller.iss  (Inno Setup 6.x)
; ============================================
; �÷���ʾ������
; iscc installer\exportInstaller.iss ^
;   /DInputDir="dist" ^
;   /DOutputDir="." ^
;   /DMyAppVersion="1.4.0"

; -------------------------
; Ԫ��Ϣ��ɴ��ο���
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
; ��װ����������
; -------------------------
[Setup]
; �̶� AppId��������ģ��Ա�����/ж��ʶ��Ϊͬһ��Ʒ��
AppId={{3D5B3A12-2CDA-4E16-9F3D-3B9E4C8C2D17}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

; ��װĿ¼���Զ�ѡ�� 64 λ Program Files �� 32 λ��
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
AllowNoIcons=yes

; �������
OutputDir={#OutputDir}
OutputBaseFilename=LLC_MOD_Toolbox_Installer

; ѹ���Ż�
Compression=lzma2/ultra
SolidCompression=yes

; 64 λϵͳ���� 64 λģʽ��װ������ x86 ����Ҳ���ճ����У�
ArchitecturesAllowed=x64 x86
ArchitecturesInstallIn64BitMode=x64

; ��Ҫ����ԱȨ�ް�װ�� Program Files
PrivilegesRequired=admin

; ��װ/ж����ʾͼ�꣺�� dist ����ͼ����ʹ��
#ifexist "{#InputDir}\favicon.ico"
SetupIconFile={#InputDir}\favicon.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
#else
UninstallDisplayIcon={app}\{#MyAppExeName}
#endif

; ��ѡ���Ѱ汾��Ϣд�밲װ���ļ�����
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoProductName={#MyAppName}

; -------------------------
; ����
; -------------------------
[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "chinesesimplified"; MessagesFile: "{#SourcePath}\Languages\ChineseSimplified.isl"

; -------------------------
; �Զ�����������ͼ�꣩
; -------------------------
[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

; -------------------------
; Ҫ�����װ�����ļ�
; -------------------------
[Files]
; �ȷ������� exe
Source: "{#InputDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

; �ٰ� InputDir ��������ȫ�����루�ų��� exe�������ظ���
Source: "{#InputDir}\*"; Excludes: "{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; -------------------------
; ��ʼ�˵�/�����ݷ�ʽ
; -------------------------
[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

; -------------------------
; ��װ���ѡ����
; -------------------------
[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
