#!/bin/bash
server="192.168.0.88"
share="Clients"
credentials="/home/kuricyn/.smbuser"
options="iocharset=utf8,gid=kuricyn,uid=kuricyn,file_mode=0777,dir_mode=0777"
echo "Mount '"$share"' in linux..."
mount -t cifs -o credentials=$credentials,$options  "//"$server"/"$share "/mnt/share/"$share
