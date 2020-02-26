using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vostok.ServiceDiscovery.Extensions.Helpers
{
    [PublicAPI]
    public class ReplicaComparer : IEqualityComparer<Uri>
    {
        public static readonly ReplicaComparer Instance = new ReplicaComparer();

        public bool Equals(Uri r1, Uri r2)
        {
            if (ReferenceEquals(r1, r2))
                return true;

            if (r1 == null || r2 == null)
                return false;

            if (r1.Port != r2.Port)
                return false;

            var host1 = r1.DnsSafeHost;
            var host2 = r2.DnsSafeHost;

            LocateHostname(host1, out var length1);
            LocateHostname(host2, out var length2);

            return length1 == length2 && string.Compare(host1, 0, host2, 0, length1, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public int GetHashCode(Uri replica)
        {
            var host = replica.DnsSafeHost;

            LocateHostname(host, out var length);

            if (length < host.Length)
                host = host.Substring(0, length);

            return (StringComparer.OrdinalIgnoreCase.GetHashCode(host) * 397) ^ replica.Port;
        }

        private static void LocateHostname(string host, out int length)
        {
            length = host.Length;

            if (length <= 0)
                return;

            var dotIndex = host.IndexOf('.');
            if (dotIndex < 0)
                return;

            if (char.IsDigit(host[0]))
                return;

            length = dotIndex;
        }
    }
}