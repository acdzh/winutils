Get-ChildItem -Recurse -Force -Filter "._*" | Remove-Item -Force
Get-ChildItem -Recurse -Force -Filter "._.DS_Store" | Remove-Item -Force