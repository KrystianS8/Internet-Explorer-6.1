; Script generated with the HM VNIEdit Script Wizard, customized for Internet Explorer 6.1

!define PRODUCT_NAME "Internet Explorer 6.1"
!define PRODUCT_VERSION "1.2"
!define PRODUCT_PUBLISHER "Internet Surfers"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\iexplore61.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; Force administrator rights to allow writing to C:\IE61 and registering system paths
RequestExecutionLevel admin

SetCompressor lzma

; --- Modern UI Definitions ---
!include "MUI2.nsh"
!include "LogicLib.nsh"
!include "nsDialogs.nsh"

!define MUI_ABORTWARNING
!define MUI_ICON "source\ieicon.ico"
!define MUI_UNICON "source\ieicon.ico"

; --- Pages Configuration ---
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
Page custom ExtraShortcutOptionsPage ExtraShortcutOptionsPageLeave
!insertmacro MUI_PAGE_INSTFILES

; Finish page run option
!define MUI_FINISHPAGE_RUN "$INSTDIR\iexplore61.exe"
!define MUI_FINISHPAGE_RUN_NOTCHECKED
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; --- Languages (Inserted AFTER pages as required by MUI2) ---
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "Albanian"
!insertmacro MUI_LANGUAGE "Arabic"
!insertmacro MUI_LANGUAGE "Belarusian"
!insertmacro MUI_LANGUAGE "Bulgarian"
!insertmacro MUI_LANGUAGE "Catalan"
!insertmacro MUI_LANGUAGE "Croatian"
!insertmacro MUI_LANGUAGE "Czech"
!insertmacro MUI_LANGUAGE "Danish"
!insertmacro MUI_LANGUAGE "Dutch"
!insertmacro MUI_LANGUAGE "Estonian"
!insertmacro MUI_LANGUAGE "Farsi"
!insertmacro MUI_LANGUAGE "Finnish"
!insertmacro MUI_LANGUAGE "French"
!insertmacro MUI_LANGUAGE "German"
!insertmacro MUI_LANGUAGE "Greek"
!insertmacro MUI_LANGUAGE "Hebrew"
!insertmacro MUI_LANGUAGE "Hungarian"
!insertmacro MUI_LANGUAGE "Indonesian"
!insertmacro MUI_LANGUAGE "Irish"
!insertmacro MUI_LANGUAGE "Italian"
!insertmacro MUI_LANGUAGE "Japanese"
!insertmacro MUI_LANGUAGE "Korean"
!insertmacro MUI_LANGUAGE "Kurdish"
!insertmacro MUI_LANGUAGE "Latvian"
!insertmacro MUI_LANGUAGE "Lithuanian"
!insertmacro MUI_LANGUAGE "Luxembourgish"
!insertmacro MUI_LANGUAGE "Norwegian"
!insertmacro MUI_LANGUAGE "Polish"
!insertmacro MUI_LANGUAGE "Portuguese"
!insertmacro MUI_LANGUAGE "Romanian"
!insertmacro MUI_LANGUAGE "Russian"
!insertmacro MUI_LANGUAGE "Serbian"
!insertmacro MUI_LANGUAGE "Slovak"
!insertmacro MUI_LANGUAGE "Slovenian"
!insertmacro MUI_LANGUAGE "Spanish"
!insertmacro MUI_LANGUAGE "Swedish"
!insertmacro MUI_LANGUAGE "Thai"
!insertmacro MUI_LANGUAGE "Turkish"
!insertmacro MUI_LANGUAGE "Ukrainian"
!insertmacro MUI_LANGUAGE "Vietnamese"

; --- Variables for Custom Checkboxes ---
Var CheckBoxDesktop
Var CheckBoxStartMenu
Var CheckBoxQuickLaunch
Var CheckBoxTaskbar

Name "${PRODUCT_NAME}"
OutFile "IE61Setup.exe"                ; <--- Compiled installer filename
InstallDir "C:\IE61"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

; --- Installer Initialization ---
Function .onInit
  SetShellVarContext all
  
  ; Triggers the language selection dialog right before the Welcome screen
  !insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd

Section "Internet Explorer 6.1 Base (Required)" SEC_BASE
  SectionIn RO ; Mandatory base section
  SetOverwrite on
  
  ; Package root directory recursively while excluding the installer and script itself
  SetOutPath "$INSTDIR"
  File /r /x "IE61Setup.exe" /x "installer.nsi" *.*
SectionEnd

Section -Post
  ; Register App Paths so Windows knows it's a real installed application
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\iexplore61.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "Path" "$INSTDIR"

  ; Register Add/Remove Programs (Uninstall entry with proper display icon)
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\source\ieicon.ico"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "QuietUninstallString" "$INSTDIR\uninst.exe /S"
  WriteRegDWORD ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "NoModify" 1
  WriteRegDWORD ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "NoRepair" 1
  
  WriteUninstaller "$INSTDIR\uninst.exe"

  ; Create Desktop Shortcut (.lnk file) if checked
  ${If} $CheckBoxDesktop == 1
    CreateShortCut "$DESKTOP\Internet Explorer 6.1.lnk" "$INSTDIR\iexplore61.exe" "" "$INSTDIR\source\ieicon.ico" 0
  ${EndIf}

  ; Create Start Menu Folder & Shortcuts (.lnk files) if checked
  ${If} $CheckBoxStartMenu == 1
    CreateDirectory "$SMPROGRAMS\Internet Explorer 6.1"
    CreateShortCut "$SMPROGRAMS\Internet Explorer 6.1\Internet Explorer 6.1.lnk" "$INSTDIR\iexplore61.exe" "" "$INSTDIR\source\ieicon.ico" 0
    CreateShortCut "$SMPROGRAMS\Internet Explorer 6.1\Uninstall.lnk" "$INSTDIR\uninst.exe" "" "$INSTDIR\source\ieicon.ico" 0
  ${EndIf}

  ; Create Quick Launch Shortcut (.lnk file) if checked
  ${If} $CheckBoxQuickLaunch == 1
    CreateShortCut "$QUICKLAUNCH\Internet Explorer 6.1.lnk" "$INSTDIR\iexplore61.exe" "" "$INSTDIR\source\ieicon.ico" 0
  ${EndIf}

  ; Taskbar shortcut handling
  ${If} $CheckBoxTaskbar == 1
    CreateShortCut "$APPDATA\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar\Internet Explorer 6.1.lnk" "$INSTDIR\iexplore61.exe" "" "$INSTDIR\source\ieicon.ico" 0
  ${EndIf}

  ; Tell Windows shell to refresh icon caches so the new shortcuts show up instantly
  System::Call 'shell32.dll::SHChangeNotify(i 0x08000000, i 0, i 0, i 0)'
SectionEnd

; --- Custom Page for Shortcuts & Options ---
Function ExtraShortcutOptionsPage
  !insertmacro MUI_HEADER_TEXT "Choose Shortcuts and Options" "Select additional tasks you would like Setup to perform while installing Internet Explorer 6.1."
  
  nsDialogs::Create 1018
  Pop $0
  ${If} $0 == error
    Abort
  ${EndIf}

  ${NSD_CreateCheckbox} 10u 10u 290u 15u "Create a desktop icon"
  Pop $CheckBoxDesktop
  SendMessage $CheckBoxDesktop ${BM_SETCHECK} ${BST_CHECKED} 0

  ${NSD_CreateCheckbox} 10u 30u 290u 15u "Create a Start Menu folder"
  Pop $CheckBoxStartMenu
  SendMessage $CheckBoxStartMenu ${BM_SETCHECK} ${BST_CHECKED} 0

  ${NSD_CreateCheckbox} 10u 50u 290u 15u "Create a Quick Launch shortcut"
  Pop $CheckBoxQuickLaunch
  SendMessage $CheckBoxQuickLaunch ${BM_SETCHECK} ${BST_CHECKED} 0

  ${NSD_CreateCheckbox} 10u 70u 290u 15u "Pin to Windows Taskbar"
  Pop $CheckBoxTaskbar
  SendMessage $CheckBoxTaskbar ${BM_SETCHECK} ${BST_CHECKED} 0

  nsDialogs::Show
FunctionEnd

Function ExtraShortcutOptionsPageLeave
  ${NSD_GetState} $CheckBoxDesktop $CheckBoxDesktop
  ${NSD_GetState} $CheckBoxStartMenu $CheckBoxStartMenu
  ${NSD_GetState} $CheckBoxQuickLaunch $CheckBoxQuickLaunch
  ${NSD_GetState} $CheckBoxTaskbar $CheckBoxTaskbar
FunctionEnd

; --- Uninstaller Section ---
Section Uninstall
  SetShellVarContext all

  ; Recursively wipe out the entire installation folder contents
  RMDir /r "$INSTDIR"

  ; Remove shortcuts
  Delete "$DESKTOP\Internet Explorer 6.1.lnk"
  Delete "$SMPROGRAMS\Internet Explorer 6.1\Internet Explorer 6.1.lnk"
  Delete "$SMPROGRAMS\Internet Explorer 6.1\Uninstall.lnk"
  RMDir "$SMPROGRAMS\Internet Explorer 6.1"
  Delete "$QUICKLAUNCH\Internet Explorer 6.1.lnk"
  Delete "$APPDATA\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar\Internet Explorer 6.1.lnk"

  ; Clean registry keys
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  
  ; Refresh shell icons after uninstall
  System::Call 'shell32.dll::SHChangeNotify(i 0x08000000, i 0, i 0, i 0)'
  SetAutoClose true
SectionEnd