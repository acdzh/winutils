Get-NetIPAddress | Where-Object {('WLAN', '以太网', 'Ethernet') -contains $_.InterfaceAlias} | Format-Table InterfaceAlias, IPAddress
$ipapi = (Invoke-WebRequest -Uri https://ipapi.co/ip).Content
Write-Host 'ipapi         ' $ipapi
$ip_api_ip = (ConvertFrom-Json (Invoke-WebRequest http://ip-api.com/json)).query
Write-Host 'ip-api ip     ' $ip_api_ip
$ip_api_dns = (ConvertFrom-Json (Invoke-WebRequest http://edns.ip-api.com/json)).dns.ip
Write-Host 'ip-api dns    ' $ip_api_dns
$ipgoji = (Invoke-WebRequest -Uri http://ip-goji.appspot.com/text).Content
Write-Host 'ip-goji       ' $ipgoji