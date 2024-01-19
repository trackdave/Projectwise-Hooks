# Projectwise-Hooks

Implement Hooks for ProjectWise

You can use the Projectwise Custom Module Manager to test.

For Release:
Step 1: Place {NAME_OF_DLL}.dll here: C:\Program Files (x86)\Bentley\ProjectWise\bin

Step 2: Configure registry key:

[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Bentley\ProjectWise\10.00\CustomModules\GPI]
"Application Mask"=dword:00000001
"Function"="CustomInitialize"
"Library"="{NAME_OF_DLL}.dll"
