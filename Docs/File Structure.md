# File Structure #

#### /.events/aggregate-root-guid/.history ####

* Contains an immutable stream of events, in order.
* File has read-only flag set i.e. `FileAttributes.ReadOnly`.
* File is only ever written to using `FileMode.Append`. Which prevents modification of existing content. Also gains the benefit that sysadmins may be able to lock down permissions by only granting append access but not write access.
* Some sort of background process will occasionally update this file by appending recent events to it. So temporarily the read-only flag will be removed, and the file locked for exclusive access by this process. This process will use optomistic concurrency i.e. if it cannot get a exclusive lock on the file then it will give up and try again later.
* When opened for reading, file is opened using `FileAttributes.Temporary` (to improve OS caching) and `FileShare.Read` (to allow other readers but not writers).

#### /.events/aggregate-root-guid/event-id ####

* Contains a single event that is considered immutable (never modifiable, but eventually deletable).
* 