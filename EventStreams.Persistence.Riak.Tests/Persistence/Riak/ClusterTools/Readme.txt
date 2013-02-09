ClusterTools requires that each node of your cluster is configured in the following way:

	1. The "sudo" command must be available without a TTY session. (because plink.exe doesn't use TTY)
		$ sudo nano /etc/sudoers
		 ... then comment out the "Defaults    requiretty" line.

	2. Your user account must be explicitly allowed to use "sudo" command without entering a password.
		$ sudo nano /etc/sudoers
		 ... append a line to the VERY end of this file like this:

			administrator   ALL=(ALL)       NOPASSWD: ALL

		 replace the "administrator" word with the name of your user account.

	3. The LAN IP range must be using 10.0.0.0/8, which is the default on Azure VM's.

Additionally, you must configure your PuTTY and Pageant tools in the following ways:

	1. Setup each node in PuTTY as a "Saved Session", name each node as follows: riak01, riak02 and riak03.

	2. Ensure that Pageant is loaded and pre-authenticated with your SSH private key.

	3. Ensure that you have used PuTTY to connect to each node at least once, as you must click "Yes" to
	   the dialog concerning whether you trust the SSH certificate of the server.


That's everything for now.