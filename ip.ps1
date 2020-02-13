Get-NetIPAddress | Where-Object {('WLAN', '以太网', 'Ethernet') -contains $_.InterfaceAlias} | Format-Table InterfaceAlias, IPAddress
$ipapi = try {(Invoke-WebRequest -Uri https://ipapi.co/ip).Content} catch {'Net Error'}
Write-Host 'ipapi         ' $ipapi
$ip_api_ip = try {(ConvertFrom-Json (Invoke-WebRequest http://ip-api.com/json)).query} catch {'Net Error'}
Write-Host 'ip-api ip     ' $ip_api_ip
$ip_api_dns = try{(ConvertFrom-Json (Invoke-WebRequest http://edns.ip-api.com/json)).dns.ip} catch {'Net Error'}
Write-Host 'ip-api dns    ' $ip_api_dns
$ipgoji = try{(Invoke-WebRequest -Uri http://ip-goji.appspot.com/text).Content} catch{'Net Error'}
Write-Host 'ip-goji       ' $ipgoji