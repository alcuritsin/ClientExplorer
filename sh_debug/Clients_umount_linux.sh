#!/bin/bash
share=Clients
echo "Unmount '"$share"' in linux..."
umount "/mnt/share/"$share -l
