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

; --- Languages ---
!insertmacro MUI_LANGUAGE "English"

; --- Variables for Custom Checkboxes ---
Var CheckBoxDesktop
Var CheckBoxStartMenu
Var CheckBoxQuickLaunch
Var CheckBoxTaskbar

Name "${PRODUCT_NAME}"
OutFile "setup.exe"
InstallDir "C:\IE61"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

; --- Installer Initialization ---
Function .onInit
  ; Set shell context to 'all' so shortcuts go to Public/All Users folders
  SetShellVarContext all
FunctionEnd

Section "Internet Explorer 6.1 Base (Required)" SEC_BASE
  SectionIn RO
  SetOverwrite on
  
  ; Copy main application executable to installation root
  SetOutPath "$INSTDIR"
  File "iexplore61.exe"
  
  ; Bundle and extract the source folder directly into $INSTDIR\source
  SetOutPath "$INSTDIR\source"
  File /r "source\*.*"
  
  SetOutPath "$INSTDIR"
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

  ${NSD_CreateCheckbox} 10u 70u 290u 15u "Pin to Windows Taskbar (creates shortcut)"
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
  ; Match the shell context so uninstaller deletes from All Users folders too
  SetShellVarContext all

  Delete "$INSTDIR\uninst.exe"
  
  ; Clean up source folder contents and the directory itself
  RMDir /r "$INSTDIR\source"
  
  Delete "$INSTDIR\iexplore61.exe"
  Delete "$INSTDIR\history.txt"
  Delete "$INSTDIR\homepage.txt"
  Delete "$INSTDIR\default_check.txt"

  Delete "$DESKTOP\Internet Explorer 6.1.lnk"
  Delete "$SMPROGRAMS\Internet Explorer 6.1\Internet Explorer 6.1.lnk"
  Delete "$SMPROGRAMS\Internet Explorer 6.1\Uninstall.lnk"
  RMDir "$SMPROGRAMS\Internet Explorer 6.1"
  Delete "$QUICKLAUNCH\Internet Explorer 6.1.lnk"

  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  
  ; Refresh shell icons after uninstall
  System::Call 'shell32.dll::SHChangeNotify(i 0x08000000, i 0, i 0, i 0)'
  SetAutoClose true
SectionEnd