## 0.1.11 (02-05-2023):

Added method to remove `HostingTopology` property.

## 0.1.10 (29-05-2022):

Added `HostingTopology` extension property.

## 0.1.9 (11-01-2022):

Added service discovery blacklist events setup.

## 0.1.8 (06-12-2021):

Added `net6.0` target.

## 0.1.6 (24-06-2021):

Added extension methods for working with ApplicationNode tags.

Added tag filter abstraction and realizations.

## 0.1.5 (20-04-2020):

ReplicaComparer moved to Vostok.Commons.Helpers

## 0.1.4 (26-02-2020):

ReplicaComparer is now public. It also considers only the first segment of URL hosts from now on.

## 0.1.3 (12-09-2019):

Added extensions to add replicas to application blacklist (see `AddToBlacklistAsync()`).
Added extensions to remove replicas from application blacklist (see `RemoveFromBlacklistAsync()`).
Added extensions to set application external url (see `SetExternalUrlAsync()`).

## 0.1.1 (20-07-2019):

Added an extension to extract replica weights from topology properties (see `GetReplicaWeights()`).

## 0.1.0 (15-05-2019): 

Initial prerelease.
