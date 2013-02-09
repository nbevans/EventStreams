# Simulates a network partition scenario by blocking all TCP LAN traffic for a node.
# This will prevent the node from talking to other nodes in the cluster.
# The rules are not persisted, so a restart of the iptables service (or indeed the whole box) will reset things to normal.

# First add special exemption to allow loopback traffic on the LAN interface.
# Without this, riak-admin gets confused and thinks the local node is down when it isn't.
sudo iptables -I OUTPUT -p tcp -d $(hostname -i) -j ACCEPT
sudo iptables -I INPUT -p tcp -s $(hostname -i) -j ACCEPT

# Now block all other LAN traffic.
sudo iptables -I OUTPUT 2 -p tcp -d 10.0.0.0/8 -j REJECT
sudo iptables -I INPUT 2 -p tcp -s 10.0.0.0/8 -j REJECT
