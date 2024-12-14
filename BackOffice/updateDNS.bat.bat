@echo off

REM Remove old DNS entries
netsh interface ipv4 delete dns "DeiVPN" 10.9.21.71
netsh interface ipv4 delete dns "DeiVPN" 192.168.62.8
netsh interface ipv4 delete dns "DeiVPN" 192.168.62.4
netsh interface ipv4 delete dns "DeiVPN" 192.168.62.32

REM Add the desired DNS server with the highest priority (metric 1)
netsh interface ipv4 add dns "DeiVPN" 10.9.21.71 index=1

REM Optional: Add other DNS servers with lower priority (metric 2)
netsh interface ipv4 add dns "DeiVPN" 192.168.62.8 index=2
netsh interface ipv4 add dns "DeiVPN" 192.168.62.4 index=3
netsh interface ipv4 add dns "DeiVPN" 192.168.62.32 index=4

REM Optionally, display the DNS configuration to verify the changes
netsh interface ipv4 show config
