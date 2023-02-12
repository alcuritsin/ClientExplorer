#!/bin/bash
server=192.168.0.88
share=Clients
credentials=/home/kuricyn/.smbuser
echo "Mount '"$share"' in linux..."
mount -t cifs -o credentials=$credentials "//"$server"/"$share "/mnt/share/"$share
