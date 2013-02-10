# Gets the readiness of the ring.
# Docs says "Checks whether all nodes in the cluster agree on the ring state. Prints “FALSE” if the nodes do not agree. This is useful after changing cluster membership to make sure that ring state has settled."

sudo riak-admin ringready
