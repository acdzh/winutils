$do = Read-Host 'add tongji dns(1), del tongji dns(2)'
<#
Get-DnsClientServerAddress -AddressFamily IPv4 |
Out-GridView -PassThru |
foreach {
    if($do -eq 1){
        Set-DnsClientServerAddress -InterfaceIndex $_.InterfaceIndex -Addresses '202.120.190.208','202.96.209.5'
        }
    elseif($do -eq 2){
        Set-DnsClientServerAddress -InterfaceIndex $_.InterfaceIndex -Addresses '',''
        }
 }
 #>
if($do -eq 1){
    Set-DnsClientServerAddress -Addresses ('202.120.190.208','202.96.209.5') 'WLAN'
    }
elseif($do -eq 2){
    Set-DnsClientServerAddress -ResetServerAddresses 'WLAN'
}elseif($do -eq 3){
    Set-DnsClientServerAddress -Addresses ('202.45.84.58','202.45.84.58') 'WLAN'
}
Get-DnsClientServerAddress -AddressFamily IPv4 'WLAN'
$do = Read-Host '按任意键退出...'