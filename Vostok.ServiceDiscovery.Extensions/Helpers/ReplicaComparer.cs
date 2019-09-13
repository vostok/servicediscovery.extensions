using System;
using System.Collections.Generic;

namespace Vostok.ServiceDiscovery.Extensions.Helpers
{
    internal class ReplicaComparer : IEqualityComparer<Uri>
    {
        public static readonly ReplicaComparer Instance = new ReplicaComparer();

        public bool Equals(Uri r1, Uri r2) =>
            StringComparer.OrdinalIgnoreCase.Equals(r1?.DnsSafeHost, r2?.DnsSafeHost) && r1?.Port == r2?.Port;

        public int GetHashCode(Uri replica) =>
            (StringComparer.OrdinalIgnoreCase.GetHashCode(replica.DnsSafeHost) * 397) ^ replica.Port;
    }
}