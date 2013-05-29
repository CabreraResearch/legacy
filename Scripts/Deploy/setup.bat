echo "This script must be run 'As Administrator', if you did not open the Command-Prompt using the 'As Administrator' override, please close this window and start again."
pause

@powershell -NoProfile -ExecutionPolicy unrestricted -File setup.ps1
pause
